using System.Runtime.CompilerServices;
using Clouded.Platform.Models.Enums;
using Clouded.Platform.Provider.Options;
using Clouded.Platform.Provider.Services.Interfaces;
using k8s;
using k8s.Autorest;
using k8s.KubeConfigModels;
using k8s.Models;

namespace Clouded.Platform.Provider.Services;

public class KubernetesService : IKubernetesService
{
    public const string ProviderIdLabel = "clouded.provider/id";
    public const string ProviderTypeLabel = "clouded.provider/type";
    public const string UserIdLabel = "clouded.provider/metadata.user.id";
    public const string DeployAtLabel = "clouded.provider/metadata.deploy_at";
    public const string HubRegionCodeLabel = "clouded.provider/metadata.hub.region.code";

    private readonly string _domain;
    private readonly ProviderKubernetesOptions _k8SOptions;
    private readonly IKubernetes _k8S;

    public KubernetesService(ApplicationOptions options)
    {
        _domain = options.Clouded.Domain;
        _k8SOptions = options.Clouded.Provider.Kubernetes;

        var config = KubernetesClientConfiguration.BuildConfigFromConfigObject(
            new K8SConfiguration
            {
                CurrentContext = _k8SOptions.CurrentContext,
                Preferences = _k8SOptions.Preferences,
                Clusters = _k8SOptions.Clusters.Select(
                    clusterOptions =>
                        new Cluster
                        {
                            Name = clusterOptions.Name,
                            ClusterEndpoint = new ClusterEndpoint
                            {
                                Server = clusterOptions.Server,
                                CertificateAuthorityData = clusterOptions.CertificateAuthorityData
                            }
                        }
                ),
                Contexts = _k8SOptions.Contexts.Select(
                    contextOptions =>
                        new Context
                        {
                            Name = contextOptions.Name,
                            ContextDetails = new ContextDetails
                            {
                                Cluster = contextOptions.Cluster,
                                User = contextOptions.User
                            }
                        }
                ),
                Users = _k8SOptions.Users.Select(
                    userOptions =>
                        new User
                        {
                            Name = userOptions.Name,
                            UserCredentials = new UserCredentials
                            {
                                ClientCertificateData = userOptions.ClientCertificateData,
                                ClientKeyData = userOptions.ClientKeyData
                            }
                        }
                )
            }
        );

        _k8S = new Kubernetes(config);
    }

    public async Task<V1Namespace?> GetNamespace(
        string @namespace,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            return await _k8S.CoreV1.ReadNamespaceAsync(
                @namespace,
                cancellationToken: cancellationToken
            );
        }
        catch
        {
            return null;
        }
    }

    public async Task<V1Deployment?> GetDeployment(
        string @namespace,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            return await _k8S.AppsV1.ReadNamespacedDeploymentAsync(
                @namespace,
                @namespace,
                cancellationToken: cancellationToken
            );
        }
        catch
        {
            return null;
        }
    }

    public async Task<V1Pod?> GetPod(
        string @namespace,
        CancellationToken cancellationToken = default
    )
    {
        var pods = await _k8S.CoreV1.ListNamespacedPodAsync(
            @namespace,
            cancellationToken: cancellationToken
        );
        return pods.Items.FirstOrDefault();
    }

    public async Task<EProviderStatus> GetPodStatus(
        string @namespace,
        CancellationToken cancellationToken = default
    )
    {
        var pod = await GetPod(@namespace, cancellationToken);

        return pod == null ? EProviderStatus.Stopped : EProviderStatus.Running;
    }

    public async IAsyncEnumerable<(
        WatchEventType EventType,
        V1Namespace Namespace
    )> WatchProviderNamespacesAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        var watchNamespaces = _k8S.CoreV1
            .ListNamespaceWithHttpMessagesAsync(
                labelSelector: ProviderTypeLabel,
                watch: true,
                cancellationToken: cancellationToken
            )
            .WatchAsync<V1Namespace, V1NamespaceList>()
            .WithCancellation(cancellationToken);

        await foreach (var (type, @namespace) in watchNamespaces)
            yield return (EventType: type, Namespace: @namespace);
    }

    public async IAsyncEnumerable<(WatchEventType EventType, V1Pod Pod)> WatchNamespacedPodsAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        var watchPods = _k8S.CoreV1
            .ListPodForAllNamespacesWithHttpMessagesAsync(
                labelSelector: ProviderTypeLabel,
                watch: true,
                cancellationToken: cancellationToken
            )
            .WatchAsync<V1Pod, V1PodList>()
            .WithCancellation(cancellationToken);

        await foreach (var (type, pod) in watchPods)
            yield return (EventType: type, Pod: pod);
    }

    public async Task CreateAsync(
        string name,
        string providerId,
        string providerType,
        string userId,
        string hubRegionCode,
        string image,
        DateTime deployAt,
        string livenessUrl,
        string readinessUrl,
        IEnumerable<V1EnvVar> envs,
        CancellationToken cancellationToken = default
    )
    {
        const int port = 8080;
        const string repositorySecretName = "docker-repository-secret";
        var kubernetesObjects = new IKubernetesObject[]
        {
            NamespaceDefinition(name, providerId, providerType, userId, hubRegionCode),
            SecretDefinition(
                name,
                repositorySecretName,
                "kubernetes.io/dockerconfigjson",
                new Dictionary<string, string>
                {
                    [".dockerconfigjson"] = _k8SOptions.RepositoryAuth
                }
            ),
            ServiceAccountDefinition(name),
            DeploymentDefinition(
                name,
                image,
                port,
                providerType,
                repositorySecretName,
                name,
                deployAt,
                livenessUrl,
                readinessUrl,
                envs.Concat(
                    new V1EnvVar[]
                    {
                        new("DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE", "false"),
                        new("ASPNETCORE_ENVIRONMENT", "Production"),
                        new("ASPNETCORE_URLS", $"http://*:{port}"),
                    }
                )
            ),
            ServiceDefinition(name),
            IngressDefinition(name, port)
        };

        foreach (var kubernetesObject in kubernetesObjects)
            await CreateAsync(kubernetesObject, cancellationToken);
    }

    public async Task StartAsync(string name, CancellationToken cancellationToken)
    {
        await _k8S.AppsV1.PatchNamespacedDeploymentScaleAsync(
            new V1Patch(
                new V1Deployment(spec: new V1DeploymentSpec { Replicas = 1 }),
                V1Patch.PatchType.MergePatch
            ),
            name,
            name,
            cancellationToken: cancellationToken
        );
    }

    public async Task StopAsync(string name, CancellationToken cancellationToken = default)
    {
        await _k8S.AppsV1.PatchNamespacedDeploymentScaleAsync(
            new V1Patch(
                new V1Deployment(spec: new V1DeploymentSpec { Replicas = 0 }),
                V1Patch.PatchType.MergePatch
            ),
            name,
            name,
            cancellationToken: cancellationToken
        );
    }

    public async Task UpdateDeploymentAsync(
        string name,
        int? replicas = null,
        IEnumerable<V1EnvVar>? containerEnvs = null,
        CancellationToken cancellationToken = default
    )
    {
        var deployment = new V1Deployment(spec: new V1DeploymentSpec());

        if (replicas.HasValue)
            deployment.Spec.Replicas = replicas.Value;

        if (containerEnvs != null)
        {
            deployment.Spec.Template ??= new V1PodTemplateSpec();
            deployment.Spec.Template.Spec ??= new V1PodSpec();

            var envArray = containerEnvs.ToArray();
            deployment.Spec.Template.Spec.Containers = new List<V1Container>
            {
                new() { Name = name, Env = envArray }
            };
        }

        await _k8S.AppsV1.PatchNamespacedDeploymentAsync(
            new V1Patch(deployment, V1Patch.PatchType.StrategicMergePatch),
            name,
            name,
            cancellationToken: cancellationToken
        );
    }

    public Task DeleteAsync(string name, CancellationToken cancellationToken) =>
        _k8S.CoreV1.DeleteNamespaceAsync(name, cancellationToken: cancellationToken);

    private V1Namespace NamespaceDefinition(
        string name,
        string providerId,
        string providerType,
        string userId,
        string hubRegionCode
    ) =>
        new()
        {
            ApiVersion = $"{V1Namespace.KubeGroup}/{V1Namespace.KubeApiVersion}",
            Kind = V1Namespace.KubeKind,
            Metadata = new V1ObjectMeta
            {
                Name = name,
                Labels = new Dictionary<string, string>
                {
                    [ProviderIdLabel] = providerId,
                    [ProviderTypeLabel] = providerType,
                    [UserIdLabel] = userId,
                    [HubRegionCodeLabel] = hubRegionCode
                }
            },
        };

    private V1Secret SecretDefinition(
        string name,
        string secretName,
        string secretType,
        IDictionary<string, string> data
    ) =>
        new()
        {
            ApiVersion = $"{V1Secret.KubeGroup}/{V1Secret.KubeApiVersion}",
            Kind = V1Secret.KubeKind,
            Metadata = new V1ObjectMeta
            {
                Name = secretName,
                NamespaceProperty = name,
                Labels = new Dictionary<string, string>
                {
                    ["app.kubernetes.io/instance"] = name,
                    ["app.kubernetes.io/name"] = name,
                    ["app.kubernetes.io/version"] = $"{_k8SOptions.Version}_{name}",
                },
                Annotations = new Dictionary<string, string>
                {
                    ["meta.clouded.provider/release-name"] = name,
                    ["meta.clouded.provider/release-namespace"] = name
                }
            },
            Type = secretType,
            StringData = data
        };

    private V1Service ServiceDefinition(string name) =>
        new()
        {
            ApiVersion = $"{V1Service.KubeGroup}/{V1Service.KubeApiVersion}",
            Kind = V1Service.KubeKind,
            Metadata = new V1ObjectMeta
            {
                Name = name,
                NamespaceProperty = name,
                Labels = new Dictionary<string, string>
                {
                    ["app.kubernetes.io/instance"] = name,
                    ["app.kubernetes.io/name"] = name,
                    ["app.kubernetes.io/version"] = $"{_k8SOptions.Version}_{name}"
                },
                Annotations = new Dictionary<string, string>
                {
                    ["meta.clouded.provider/release-name"] = name,
                    ["meta.clouded.provider/release-namespace"] = name
                },
            },
            Spec = new V1ServiceSpec
            {
                Selector = new Dictionary<string, string>
                {
                    ["app.kubernetes.io/instance"] = name,
                    ["app.kubernetes.io/name"] = name,
                },
                Type = "ClusterIP",
                Ports = new[]
                {
                    new V1ServicePort
                    {
                        Name = "http",
                        Protocol = "TCP",
                        Port = 8080,
                        TargetPort = 8080
                    }
                },
            }
        };

    private V1ServiceAccount ServiceAccountDefinition(string name) =>
        new()
        {
            ApiVersion = $"{V1ServiceAccount.KubeGroup}/{V1ServiceAccount.KubeApiVersion}",
            Kind = V1ServiceAccount.KubeKind,
            Metadata = new V1ObjectMeta
            {
                Name = name,
                NamespaceProperty = name,
                Labels = new Dictionary<string, string>
                {
                    ["app.kubernetes.io/instance"] = name,
                    ["app.kubernetes.io/name"] = name,
                    ["app.kubernetes.io/version"] = $"{_k8SOptions.Version}_{name}",
                },
                Annotations = new Dictionary<string, string>
                {
                    ["meta.clouded.provider/release-name"] = name,
                    ["meta.clouded.provider/release-namespace"] = name
                }
            },
            Secrets = new[] { new V1ObjectReference { Name = $"{name}-account-token" } }
        };

    private V1Ingress IngressDefinition(string name, int port) =>
        new()
        {
            ApiVersion = $"{V1Ingress.KubeGroup}/{V1Ingress.KubeApiVersion}",
            Kind = V1Ingress.KubeKind,
            Metadata = new V1ObjectMeta
            {
                Name = name,
                NamespaceProperty = name,
                Labels = new Dictionary<string, string>
                {
                    ["app.kubernetes.io/instance"] = name,
                    ["app.kubernetes.io/name"] = name,
                    ["app.kubernetes.io/version"] = $"{_k8SOptions.Version}_{name}",
                },
                Annotations = new Dictionary<string, string>
                {
                    ["meta.clouded.provider/release-name"] = name,
                    ["meta.clouded.provider/release-namespace"] = name,
                    ["cert-manager.io/cluster-issuer"] = "letsencrypt-production",
                    ["kubernetes.io/ingress.class"] = "nginx",
                    ["nginx.ingress.kubernetes.io/force-ssl-redirect"] = "true",
                    ["nginx.ingress.kubernetes.io/ssl-redirect"] = "true"
                }
            },
            Spec = new V1IngressSpec
            {
                Tls = new V1IngressTLS[]
                {
                    new()
                    {
                        Hosts = new[] { $"{name}.{_domain}", },
                        SecretName = $"{name}-certificate"
                    }
                },
                Rules = new V1IngressRule[]
                {
                    new()
                    {
                        Host = $"{name}.{_domain}",
                        Http = new V1HTTPIngressRuleValue
                        {
                            Paths = new V1HTTPIngressPath[]
                            {
                                new()
                                {
                                    Path = "/",
                                    PathType = "Prefix",
                                    Backend = new V1IngressBackend
                                    {
                                        Service = new V1IngressServiceBackend
                                        {
                                            Name = name,
                                            Port = new V1ServiceBackendPort { Number = port }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

    private V1Deployment DeploymentDefinition(
        string name,
        string image,
        int port,
        string providerType,
        string repositorySecretName,
        string serviceAccountName,
        DateTime deployAt,
        string livenessUrl,
        string readinessUrl,
        IEnumerable<V1EnvVar> envs
    ) =>
        new()
        {
            ApiVersion = $"{V1Deployment.KubeGroup}/{V1Deployment.KubeApiVersion}",
            Kind = V1Deployment.KubeKind,
            Metadata = new V1ObjectMeta
            {
                Name = name,
                NamespaceProperty = name,
                Labels = new Dictionary<string, string>
                {
                    ["app.kubernetes.io/instance"] = name,
                    ["app.kubernetes.io/name"] = name,
                    ["app.kubernetes.io/version"] = $"{_k8SOptions.Version}_{name}"
                },
                Annotations = new Dictionary<string, string>
                {
                    ["meta.clouded.provider/release-name"] = name,
                    ["meta.clouded.provider/release-namespace"] = name,
                    [DeployAtLabel] = deployAt.ToString("O"),
                    ["timestamp"] = DateTime.UtcNow.ToString("O")
                }
            },
            Spec = new V1DeploymentSpec
            {
                Replicas = 0,
                Selector = new V1LabelSelector
                {
                    MatchLabels = new Dictionary<string, string>
                    {
                        ["app.kubernetes.io/instance"] = name,
                        ["app.kubernetes.io/name"] = name,
                    }
                },
                Template = new V1PodTemplateSpec
                {
                    Metadata = new V1ObjectMeta
                    {
                        CreationTimestamp = null,
                        Labels = new Dictionary<string, string>
                        {
                            [ProviderTypeLabel] = providerType,
                            ["app.kubernetes.io/instance"] = name,
                            ["app.kubernetes.io/name"] = name,
                        },
                    },
                    Spec = new V1PodSpec
                    {
                        ServiceAccount = serviceAccountName,
                        ServiceAccountName = serviceAccountName,
                        ImagePullSecrets = new[]
                        {
                            new V1LocalObjectReference(repositorySecretName)
                        },
                        Containers = new List<V1Container>
                        {
                            new()
                            {
                                Name = name,
                                Image = image,
                                ImagePullPolicy = "Always",
                                Ports = new List<V1ContainerPort>
                                {
                                    new()
                                    {
                                        Name = "http",
                                        ContainerPort = port,
                                        Protocol = "TCP"
                                    }
                                },
                                LivenessProbe = new V1Probe
                                {
                                    HttpGet = new V1HTTPGetAction
                                    {
                                        Path = livenessUrl,
                                        Port = port,
                                        Scheme = "HTTP"
                                    },
                                    InitialDelaySeconds = 600,
                                    TimeoutSeconds = 5,
                                    PeriodSeconds = 10,
                                    SuccessThreshold = 1,
                                    FailureThreshold = 3
                                },
                                ReadinessProbe = new V1Probe
                                {
                                    HttpGet = new V1HTTPGetAction
                                    {
                                        Path = readinessUrl,
                                        Port = port,
                                        Scheme = "HTTP"
                                    },
                                    InitialDelaySeconds = 3,
                                    TimeoutSeconds = 1,
                                    PeriodSeconds = 10,
                                    SuccessThreshold = 1,
                                    FailureThreshold = 3
                                },
                                // Resources = new V1ResourceRequirements
                                // {
                                //     Limits = new Dictionary<string, ResourceQuantity>
                                //     {
                                //         ["memory"] = new("200Mi"),
                                //         ["cpu"] = new("100m"),
                                //     },
                                //     Requests = new Dictionary<string, ResourceQuantity>
                                //     {
                                //         ["memory"] = new("100Mi"),
                                //         ["cpu"] = new("50m"),
                                //     },
                                // },
                                Env = envs.ToArray()
                            }
                        }
                    }
                }
            }
        };

    private async Task<IKubernetesObject?> CreateAsync(
        IKubernetesObject definition,
        CancellationToken cancellationToken
    )
    {
        var k8SObject = await GetAsync(definition, cancellationToken);

        if (k8SObject != null)
        {
            Console.WriteLine($"{k8SObject.Kind} already exists");

            if (k8SObject is V1Deployment k8SDeployment)
            {
                ((V1Deployment)definition).Spec.Replicas = k8SDeployment.Spec.Replicas;
            }

            k8SObject = await ActionAsync(
                async () =>
                    definition.Kind switch
                    {
                        V1Ingress.KubeKind
                            => await _k8S.NetworkingV1.PatchNamespacedIngressAsync(
                                new V1Patch(
                                    definition,
                                    V1Patch.PatchType.MergePatch //todo apply patch
                                ),
                                ((V1Ingress)definition).Name(),
                                ((V1Ingress)definition).Namespace(),
                                cancellationToken: cancellationToken
                            ),
                        V1Deployment.KubeKind
                            => await _k8S.AppsV1.PatchNamespacedDeploymentAsync(
                                new V1Patch(
                                    definition,
                                    V1Patch.PatchType.MergePatch //todo apply patch
                                ),
                                ((V1Deployment)definition).Name(),
                                ((V1Deployment)definition).Namespace(),
                                cancellationToken: cancellationToken
                            ),
                        _ => k8SObject
                    }
            );
        }
        else
        {
            k8SObject = await ActionAsync(
                async () =>
                    definition.Kind switch
                    {
                        V1Namespace.KubeKind
                            => await _k8S.CoreV1.CreateNamespaceAsync(
                                (V1Namespace)definition,
                                cancellationToken: cancellationToken
                            ),
                        V1Secret.KubeKind
                            => await _k8S.CoreV1.CreateNamespacedSecretAsync(
                                (V1Secret)definition,
                                ((V1Secret)definition).Namespace(),
                                cancellationToken: cancellationToken
                            ),
                        V1ServiceAccount.KubeKind
                            => await _k8S.CoreV1.CreateNamespacedServiceAccountAsync(
                                (V1ServiceAccount)definition,
                                ((V1ServiceAccount)definition).Namespace(),
                                cancellationToken: cancellationToken
                            ),
                        V1Service.KubeKind
                            => await _k8S.CoreV1.CreateNamespacedServiceAsync(
                                (V1Service)definition,
                                ((V1Service)definition).Namespace(),
                                cancellationToken: cancellationToken
                            ),
                        V1Ingress.KubeKind
                            => await _k8S.NetworkingV1.CreateNamespacedIngressAsync(
                                (V1Ingress)definition,
                                ((V1Ingress)definition).Namespace(),
                                cancellationToken: cancellationToken
                            ),
                        V1Deployment.KubeKind
                            => await _k8S.AppsV1.CreateNamespacedDeploymentAsync(
                                (V1Deployment)definition,
                                ((V1Deployment)definition).Namespace(),
                                cancellationToken: cancellationToken
                            ),
                        V1Pod.KubeKind
                            => await _k8S.CoreV1.CreateNamespacedPodAsync(
                                (V1Pod)definition,
                                ((V1Pod)definition).Namespace(),
                                cancellationToken: cancellationToken
                            ),
                        _ => throw new NotImplementedException()
                    }
            );
        }

        if (k8SObject != null)
            Console.WriteLine($"{k8SObject.Kind} created!");

        return k8SObject;
    }

    private async Task<IKubernetesObject?> GetAsync(
        IKubernetesObject definition,
        CancellationToken cancellationToken
    ) =>
        await ActionAsync(
            async () =>
                definition.Kind switch
                {
                    V1Namespace.KubeKind
                        => await _k8S.CoreV1.ReadNamespaceAsync(
                            ((V1Namespace)definition).Name(),
                            cancellationToken: cancellationToken
                        ),
                    V1Secret.KubeKind
                        => await _k8S.CoreV1.ReadNamespacedSecretAsync(
                            ((V1Secret)definition).Name(),
                            ((V1Secret)definition).Namespace(),
                            cancellationToken: cancellationToken
                        ),
                    V1ServiceAccount.KubeKind
                        => await _k8S.CoreV1.ReadNamespacedServiceAccountAsync(
                            ((V1ServiceAccount)definition).Name(),
                            ((V1ServiceAccount)definition).Namespace(),
                            cancellationToken: cancellationToken
                        ),
                    V1Service.KubeKind
                        => await _k8S.CoreV1.ReadNamespacedServiceAsync(
                            ((V1Service)definition).Name(),
                            ((V1Service)definition).Namespace(),
                            cancellationToken: cancellationToken
                        ),
                    V1Ingress.KubeKind
                        => await _k8S.NetworkingV1.ReadNamespacedIngressAsync(
                            ((V1Ingress)definition).Name(),
                            ((V1Ingress)definition).Namespace(),
                            cancellationToken: cancellationToken
                        ),
                    V1Deployment.KubeKind
                        => await _k8S.AppsV1.ReadNamespacedDeploymentAsync(
                            ((V1Deployment)definition).Name(),
                            ((V1Deployment)definition).Namespace(),
                            cancellationToken: cancellationToken
                        ),
                    V1Pod.KubeKind
                        => await _k8S.CoreV1.ReadNamespacedPodAsync(
                            ((V1Pod)definition).Name(),
                            ((V1Pod)definition).Namespace(),
                            cancellationToken: cancellationToken
                        ),
                    _ => throw new NotImplementedException()
                }
        );

    private async Task<IKubernetesObject?> ActionAsync(Func<Task<IKubernetesObject>> k8SAction)
    {
        try
        {
            return await k8SAction();
        }
        catch (HttpOperationException e)
        {
            await Console.Error.WriteLineAsync(e.Response.Content);
        }
        catch (Exception e)
        {
            await Console.Error.WriteLineAsync(
                $"An error occured while trying to create resource: {e}"
            );
        }

        return null;
    }
}
