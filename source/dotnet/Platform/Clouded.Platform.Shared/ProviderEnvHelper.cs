using System.Text.Json;
using Clouded.Function.Shared;
using Clouded.Platform.Database.Entities.Clouded;
using Clouded.Platform.Models.Dtos.Provider.Admin;
using Clouded.Platform.Models.Enums;
using Clouded.Shared;
using Clouded.Shared.Enums;

namespace Clouded.Platform.Shared;

public static class ProviderEnvHelper
{
    public static Dictionary<string, string> ComposeAuthEnvs(
        AuthProviderEntity? authProvider,
        string domain,
        string databaseEncryptionKey
    )
    {
        if (authProvider == null)
            return new Dictionary<string, string>();

        var authConfig = authProvider.Configuration;
        var providerEnvs = new Dictionary<string, string>
        {
            ["Clouded__Auth__ApiKey"] = authConfig.ApiKey,
            ["Clouded__Auth__Name"] = authProvider.Name,
            ["Clouded__Auth__Token__ValidIssuer"] = authConfig.Token.ValidIssuer,
            ["Clouded__Auth__Token__ValidateIssuer"] = authConfig.Token.ValidateIssuer.ToString(),
            ["Clouded__Auth__Token__ValidateAudience"] =
                authConfig.Token.ValidateAudience.ToString(),
            ["Clouded__Auth__Token__ValidateIssuerSigningKey"] =
                authConfig.Token.ValidateIssuerSigningKey.ToString(),
            ["Clouded__Auth__Token__Secret"] = authConfig.Token.Secret,
            ["Clouded__Auth__Token__AccessTokenExpiration"] =
                authConfig.Token.AccessTokenExpiration.ToString(),
            ["Clouded__Auth__Token__RefreshTokenExpiration"] =
                authConfig.Token.RefreshTokenExpiration.ToString(),
            ["Clouded__Auth__Management__Enabled"] = authConfig.Management.ToString(),
            ["Clouded__Auth__Management__TokenKey"] = authConfig.ManagementTokenKey.Decrypt(
                databaseEncryptionKey
            ),
            ["Clouded__Auth__Management__IdentityKey"] = authConfig.ManagementIdentityKey.Decrypt(
                databaseEncryptionKey
            ),
            ["Clouded__Auth__Management__PasswordKey"] = authConfig.ManagementPasswordKey.Decrypt(
                databaseEncryptionKey
            )
        };

        var usrAccess = authConfig.UserAccess.ToList();
        for (var index = 0; index < usrAccess.Count; index++)
        {
            var authUserAccessEntity = usrAccess[index];
            providerEnvs[$"Clouded__Auth__Management__Users__{index}__Identity"] =
                authUserAccessEntity.Identity;
            providerEnvs[$"Clouded__Auth__Management__Users__{index}__Password"] =
                authUserAccessEntity.Password.Decrypt(databaseEncryptionKey);
        }

        switch (authConfig.Hash)
        {
            case AuthHashArgon2ConfigurationEntity hashArgon2Config:
                providerEnvs["Clouded__Auth__Hash__Argon2__Secret"] = hashArgon2Config.Secret;
                providerEnvs["Clouded__Auth__Hash__Argon2__Type"] = hashArgon2Config.Type;
                providerEnvs["Clouded__Auth__Hash__Argon2__Version"] = hashArgon2Config.Version;
                providerEnvs["Clouded__Auth__Hash__Argon2__DegreeOfParallelism"] =
                    hashArgon2Config.DegreeOfParallelism.ToString();
                providerEnvs["Clouded__Auth__Hash__Argon2__MemorySize"] =
                    hashArgon2Config.MemorySize.ToString();
                providerEnvs["Clouded__Auth__Hash__Argon2__Iterations"] =
                    hashArgon2Config.Iterations.ToString();
                providerEnvs["Clouded__Auth__Hash__Argon2__ReturnBytes"] =
                    hashArgon2Config.ReturnBytes.ToString();
                break;
        }

        providerEnvs["Clouded__Auth__Cors__MaxAge"] = authConfig.Cors.MaxAge.ToString();
        providerEnvs["Clouded__Auth__Cors__SupportsCredentials"] =
            authConfig.Cors.SupportsCredentials.ToString();
        var allowedMethods = authConfig.Cors.AllowedMethods.Split(",");
        for (var index = 0; index < allowedMethods.Length; index++)
        {
            var allowedMethod = allowedMethods[index];
            providerEnvs[$"Clouded__Auth__Cors__AllowedMethods__{index}"] = allowedMethod;
        }

        var allowedOrigins = authConfig.Cors.AllowedOrigins.Split(",");
        for (var index = 0; index < allowedOrigins.Length; index++)
        {
            var allowedOrigin = allowedOrigins[index];
            providerEnvs[$"Clouded__Auth__Cors__AllowedOrigins__{index}"] = allowedOrigin;
        }

        var allowedHeaders = authConfig.Cors.AllowedHeaders.Split(",");
        for (var index = 0; index < allowedHeaders.Length; index++)
        {
            var allowedHeader = allowedHeaders[index];
            providerEnvs[$"Clouded__Auth__Cors__AllowedHeaders__{index}"] = allowedHeader;
        }

        var exposedHeaders = authConfig.Cors.ExposedHeaders?.Split(",") ?? Array.Empty<string>();
        for (var index = 0; index < exposedHeaders.Length; index++)
        {
            var exposedHeader = exposedHeaders[index];
            providerEnvs[$"Clouded__Auth__Cors__ExposedHeaders__{index}"] = exposedHeader;
        }

        // DataSource
        var datasourceConfig = authProvider.DataSource.Configuration;
        providerEnvs["Clouded__DataSource__Type"] = datasourceConfig.Type.ToString();
        providerEnvs["Clouded__DataSource__Port"] = datasourceConfig.Port.ToString();
        providerEnvs["Clouded__DataSource__Server"] = datasourceConfig.Server.Decrypt(
            databaseEncryptionKey
        );
        providerEnvs["Clouded__DataSource__Username"] = datasourceConfig.Username.Decrypt(
            databaseEncryptionKey
        );
        providerEnvs["Clouded__DataSource__Password"] = datasourceConfig.Password.Decrypt(
            databaseEncryptionKey
        );
        providerEnvs["Clouded__DataSource__Database"] = datasourceConfig.Database.Decrypt(
            databaseEncryptionKey
        );

        if (authConfig.IdentityOrganization != null)
        {
            providerEnvs["Clouded__Auth__Identity__Organization__Schema"] = authConfig
                .IdentityOrganization
                .Schema;
            providerEnvs["Clouded__Auth__Identity__Organization__Table"] = authConfig
                .IdentityOrganization
                .Table;
            providerEnvs["Clouded__Auth__Identity__Organization__ColumnId"] = authConfig
                .IdentityOrganization
                .ColumnId;
            providerEnvs["Clouded__Auth__Identity__Organization__ColumnIdentity"] = authConfig
                .IdentityOrganization
                .ColumnIdentity;
        }

        if (authConfig.IdentityUser != null)
        {
            providerEnvs["Clouded__Auth__Identity__User__Schema"] = authConfig.IdentityUser.Schema;
            providerEnvs["Clouded__Auth__Identity__User__Table"] = authConfig.IdentityUser.Table;
            providerEnvs["Clouded__Auth__Identity__User__ColumnId"] = authConfig
                .IdentityUser
                .ColumnId;
            providerEnvs["Clouded__Auth__Identity__User__ColumnIdentity"] = authConfig
                .IdentityUser
                .ColumnIdentity;
            providerEnvs["Clouded__Auth__Identity__User__ColumnPassword"] = authConfig
                .IdentityUser
                .ColumnPassword;
        }

        if (authConfig.IdentityRole != null)
        {
            providerEnvs["Clouded__Auth__Identity__Role__Schema"] = authConfig.IdentityRole.Schema;
            providerEnvs["Clouded__Auth__Identity__Role__Table"] = authConfig.IdentityRole.Table;
            providerEnvs["Clouded__Auth__Identity__Role__ColumnId"] = authConfig
                .IdentityRole
                .ColumnId;
            providerEnvs["Clouded__Auth__Identity__Role__ColumnIdentity"] = authConfig
                .IdentityRole
                .ColumnIdentity;
        }

        if (authConfig.IdentityPermission != null)
        {
            providerEnvs["Clouded__Auth__Identity__Permission__Schema"] = authConfig
                .IdentityPermission
                .Schema;
            providerEnvs["Clouded__Auth__Identity__Permission__Table"] = authConfig
                .IdentityPermission
                .Table;
            providerEnvs["Clouded__Auth__Identity__Permission__ColumnId"] = authConfig
                .IdentityPermission
                .ColumnId;
            providerEnvs["Clouded__Auth__Identity__Permission__ColumnIdentity"] = authConfig
                .IdentityPermission
                .ColumnIdentity;
        }

        providerEnvs["Clouded__Auth__Identity__Domain__Schema"] = authConfig.IdentityUser.Schema;
        providerEnvs["Clouded__Auth__Identity__Machine__Schema"] = authConfig.IdentityUser.Schema;

        var fbConfig = authConfig.SocialConfiguration.FirstOrDefault(
            i => i.Type == ESocialAuthType.Facebook
        );
        var googleConfig = authConfig.SocialConfiguration.FirstOrDefault(
            i => i.Type == ESocialAuthType.Google
        );
        if (fbConfig != null)
        {
            providerEnvs["Clouded__Auth__Social__Facebook__Key"] = fbConfig.Key;
            providerEnvs["Clouded__Auth__Social__Facebook__Secret"] = fbConfig.Secret;
            providerEnvs["Clouded__Auth__Social__Facebook__RedirectUrl"] = fbConfig.RedirectUrl;
            providerEnvs["Clouded__Auth__Social__Facebook__DeniedRedirectUrl"] =
                fbConfig.DeniedRedirectUrl;
        }

        if (googleConfig != null)
        {
            providerEnvs["Clouded__Auth__Social__Google__Key"] = googleConfig.Key;
            providerEnvs["Clouded__Auth__Social__Google__Secret"] = googleConfig.Secret;
            providerEnvs["Clouded__Auth__Social__Google__RedirectUrl"] = googleConfig.RedirectUrl;
            providerEnvs["Clouded__Auth__Social__Google__DeniedRedirectUrl"] =
                googleConfig.DeniedRedirectUrl;
        }

        var mailConfig = authConfig.Mail;
        if (mailConfig != null)
        {
            providerEnvs["Clouded__Mail__FromEmailDefault"] = mailConfig.From;
            providerEnvs["Clouded__Mail__Client__Server"] = mailConfig.Host.Decrypt(
                databaseEncryptionKey
            );
            providerEnvs["Clouded__Mail__Client__Port"] = mailConfig.Port.ToString();
            providerEnvs["Clouded__Mail__Client__User"] = mailConfig.User?.Decrypt(
                databaseEncryptionKey
            );
            providerEnvs["Clouded__Mail__Client__Password"] = mailConfig.Password?.Decrypt(
                databaseEncryptionKey
            );
            providerEnvs["Clouded__Mail__Client__UseSsl"] = mailConfig.UseSsl.ToString();
            providerEnvs["Clouded__Mail__Client__RequiresAuthentication"] =
                mailConfig.Authentication.ToString();
            providerEnvs["Clouded__Mail__Client__SocketOptions"] = mailConfig.SocketOptions;
            providerEnvs["Clouded__Mail__Drafts__PasswordReset__Subject"] =
                $"{authProvider.Name} - Password reset";
            providerEnvs["Clouded__Mail__Drafts__PasswordReset__Template"] =
                "<a href=\"{{ Context[\"Link\"] }}/{{ PasswordResetToken }}\">Reset Password</a>";
            providerEnvs["Clouded__Mail__Drafts__PasswordReset__Context__Link"] =
                mailConfig.ResetPasswordReturnUrl;
        }

        // Functions
        var functionProviderIndex = 0;
        foreach (var hooksGroup in authProvider.Hooks.AsQueryable().GroupBy(x => x.Provider))
        {
            var hookFunctionProvider = hooksGroup.Key;
            providerEnvs[$"Clouded__Function__Providers__{functionProviderIndex}__Id"] =
                functionProviderIndex.ToString();
            providerEnvs[$"Clouded__Function__Providers__{functionProviderIndex}__Url"] =
                $"https://{hookFunctionProvider.Code}.{domain}";
            providerEnvs[$"Clouded__Function__Providers__{functionProviderIndex}__ApiKey"] =
                hookFunctionProvider.Configuration.ApiKey;

            var functionHookIndex = 0;
            foreach (var hook in hooksGroup)
            {
                providerEnvs[$"Clouded__Function__Hooks__{functionHookIndex}__ProviderId"] =
                    functionProviderIndex.ToString();
                providerEnvs[$"Clouded__Function__Hooks__{functionHookIndex}__Name"] = hook.Name;
                providerEnvs[$"Clouded__Function__Hooks__{functionHookIndex}__Type"] =
                    hook.Type.GetEnumName();
                functionHookIndex++;
            }

            functionProviderIndex++;
        }

        return providerEnvs;
    }

    public static Dictionary<string, string> ComposeAdminEnvs(
        AdminProviderEntity? adminProvider,
        string databaseEncryptionKey
    )
    {
        if (adminProvider == null)
            return new Dictionary<string, string>();

        var authConfig = adminProvider.Configuration;
        var usersConfig = adminProvider.UserAccess;
        var dataSourcesConfig = adminProvider.DataSources;
        var tablesConfig = adminProvider.Tables;
        var navGroupsConfig = adminProvider.NavGroups;
        var providerEnvs = new Dictionary<string, string>
        {
            ["Clouded__Admin__Name"] = adminProvider.Name,
        };

        var dataSourceIndex = 0;
        foreach (var dataSourceEntity in dataSourcesConfig)
        {
            providerEnvs[$"Clouded__DataSources__{dataSourceIndex}__Id"] =
                dataSourceEntity.Id.ToString();
            providerEnvs[$"Clouded__DataSources__{dataSourceIndex}__Type"] =
                dataSourceEntity.Configuration.Type.ToString();
            providerEnvs[$"Clouded__DataSources__{dataSourceIndex}__Server"] =
                dataSourceEntity.Configuration.Server.Decrypt(databaseEncryptionKey);
            providerEnvs[$"Clouded__DataSources__{dataSourceIndex}__Port"] =
                dataSourceEntity.Configuration.Port.ToString();
            providerEnvs[$"Clouded__DataSources__{dataSourceIndex}__Username"] =
                dataSourceEntity.Configuration.Username.Decrypt(databaseEncryptionKey);
            providerEnvs[$"Clouded__DataSources__{dataSourceIndex}__Password"] =
                dataSourceEntity.Configuration.Password.Decrypt(databaseEncryptionKey);
            providerEnvs[$"Clouded__DataSources__{dataSourceIndex}__Database"] =
                dataSourceEntity.Configuration.Database.Decrypt(databaseEncryptionKey);
            dataSourceIndex++;
        }

        var functionProviderIndex = 0;
        foreach (
            var functionProviderId in adminProvider.Functions
                .Select(x => x.ProviderId)
                .Distinct()
                .ToList()
        )
        {
            providerEnvs[$"Clouded__Admin__FunctionProviders__{functionProviderIndex}__Id"] =
                functionProviderId.ToString();
            // todo build execute cmd
            // providerEnvs[$"Clouded__Admin__FunctionProviders__{functionProviderIndex}__ExecuteCmd"] = ;
            functionProviderIndex++;
        }

        ConfigOfGlobalFunction(providerEnvs, EAdminProviderFunctionTrigger.Create);
        ConfigOfGlobalFunction(providerEnvs, EAdminProviderFunctionTrigger.Update);
        ConfigOfGlobalFunction(providerEnvs, EAdminProviderFunctionTrigger.Delete);

        var userSettings = adminProvider.DataSourcesRelation.First(i => i.HasUserSettingsTable);
        providerEnvs["Clouded__Admin__Auth__UserSettings__DataSourceId"] =
            userSettings.DataSourceId.ToString();
        providerEnvs["Clouded__Admin__Auth__UserSettings__Schema"] =
            userSettings.UserSettingsSchema!;
        providerEnvs["Clouded__Admin__Auth__UserSettings__Table"] = "clouded_admin_users_settings";
        providerEnvs["Clouded__Admin__Auth__UserSettings__ColumnId"] = "user_id";
        providerEnvs["Clouded__Admin__Auth__UserSettings__ColumnSettings"] = "settings";

        providerEnvs["Clouded__Admin__Auth__PasswordKey"] = authConfig.PasswordKey.Decrypt(
            databaseEncryptionKey
        );
        providerEnvs["Clouded__Admin__Auth__TokenKey"] = authConfig.TokenKey.Decrypt(
            databaseEncryptionKey
        );
        providerEnvs["Clouded__Admin__Auth__IdentityKey"] = authConfig.IdentityKey.Decrypt(
            databaseEncryptionKey
        );

        var userIndex = 0;
        foreach (var userEntity in usersConfig)
        {
            providerEnvs[$"Clouded__Admin__Auth__Users__{userIndex}__Id"] =
                userEntity.Id.ToString();
            providerEnvs[$"Clouded__Admin__Auth__Users__{userIndex}__Identity"] =
                userEntity.Identity;
            providerEnvs[$"Clouded__Admin__Auth__Users__{userIndex}__Password"] =
                userEntity.Password.Decrypt(databaseEncryptionKey);
            userIndex++;
        }

        var navGroupIndex = 0;
        foreach (var navGroupEntity in navGroupsConfig)
        {
            providerEnvs[$"Clouded__Admin__NavGroups__{navGroupIndex}__Key"] = navGroupEntity.Key;
            providerEnvs[$"Clouded__Admin__NavGroups__{navGroupIndex}__Label"] =
                navGroupEntity.Label;
            if (navGroupEntity.Icon != null)
            {
                providerEnvs[$"Clouded__Admin__NavGroups__{navGroupIndex}__Icon"] =
                    navGroupEntity.Icon;
            }

            providerEnvs[$"Clouded__Admin__NavGroups__{navGroupIndex}__Order"] =
                navGroupEntity.Order.ToString();
            navGroupIndex++;
        }

        var tableIndex = 0;
        foreach (var tableEntity in tablesConfig)
        {
            providerEnvs[$"Clouded__Admin__Tables__{tableIndex}__DataSourceId"] =
                tableEntity.DataSourceId.ToString();
            providerEnvs[$"Clouded__Admin__Tables__{tableIndex}__Schema"] = tableEntity.Schema;
            providerEnvs[$"Clouded__Admin__Tables__{tableIndex}__Table"] = tableEntity.Table;
            providerEnvs[$"Clouded__Admin__Tables__{tableIndex}__Name"] = tableEntity.Name;
            providerEnvs[$"Clouded__Admin__Tables__{tableIndex}__InMenu"] =
                tableEntity.InMenu.ToString();

            if (tableEntity.SingularName != null)
            {
                providerEnvs[$"Clouded__Admin__Tables__{tableIndex}__SingularName"] =
                    tableEntity.SingularName;
            }

            if (tableEntity.NavGroup != null)
            {
                providerEnvs[$"Clouded__Admin__Tables__{tableIndex}__NavGroup"] =
                    tableEntity.NavGroup;
            }

            if (tableEntity.Icon != null)
            {
                providerEnvs[$"Clouded__Admin__Tables__{tableIndex}__Icon"] = tableEntity.Icon;
            }

            var colIndex = 0;
            var columns = JsonSerializer
                .Deserialize<List<AdminProviderTableInput.AdminProviderTableColumnInput>>(
                    tableEntity.Columns
                )!
                .Where(i => i.Enabled)
                .ToList();
            foreach (var column in columns)
            {
                providerEnvs[$"Clouded__Admin__Tables__{tableIndex}__Columns__{colIndex}__Column"] =
                    column.Column;
                providerEnvs[$"Clouded__Admin__Tables__{tableIndex}__Columns__{colIndex}__Name"] =
                    column.Name;
                providerEnvs[$"Clouded__Admin__Tables__{tableIndex}__Columns__{colIndex}__Order"] =
                    column.Order.ToString();
                providerEnvs[$"Clouded__Admin__Tables__{tableIndex}__Columns__{colIndex}__Type"] =
                    column.Type.ToString();
                providerEnvs[
                    $"Clouded__Admin__Tables__{tableIndex}__Columns__{colIndex}__Filterable"
                ] = column.Filterable.ToString();
                providerEnvs[
                    $"Clouded__Admin__Tables__{tableIndex}__Columns__{colIndex}__List__Visible"
                ] = column.List.Visible.ToString();
                providerEnvs[
                    $"Clouded__Admin__Tables__{tableIndex}__Columns__{colIndex}__List__Readonly"
                ] = column.List.Readonly.ToString();
                providerEnvs[
                    $"Clouded__Admin__Tables__{tableIndex}__Columns__{colIndex}__Create__Visible"
                ] = column.Create.Visible.ToString();
                providerEnvs[
                    $"Clouded__Admin__Tables__{tableIndex}__Columns__{colIndex}__Create__Readonly"
                ] = column.Create.Readonly.ToString();
                providerEnvs[
                    $"Clouded__Admin__Tables__{tableIndex}__Columns__{colIndex}__Detail__Visible"
                ] = column.Detail.Visible.ToString();
                providerEnvs[
                    $"Clouded__Admin__Tables__{tableIndex}__Columns__{colIndex}__Detail__Readonly"
                ] = column.Detail.Readonly.ToString();

                colIndex++;
            }

            var virtualColumns = JsonSerializer
                .Deserialize<List<AdminProviderTableInput.AdminProviderTableColumnInput>>(
                    tableEntity.VirtualColumns
                )!
                .Where(i => i.Enabled)
                .ToList();
            foreach (var column in virtualColumns)
            {
                providerEnvs[$"Clouded__Admin__Tables__{tableIndex}__Columns__{colIndex}__Column"] =
                    column.Column;
                providerEnvs[$"Clouded__Admin__Tables__{tableIndex}__Columns__{colIndex}__Name"] =
                    column.Name;
                providerEnvs[$"Clouded__Admin__Tables__{tableIndex}__Columns__{colIndex}__Order"] =
                    column.Order.ToString();
                providerEnvs[$"Clouded__Admin__Tables__{tableIndex}__Columns__{colIndex}__Type"] =
                    column.Type.ToString();
                providerEnvs[
                    $"Clouded__Admin__Tables__{tableIndex}__Columns__{colIndex}__VirtualValue"
                ] = column.VirtualValue;
                providerEnvs[
                    $"Clouded__Admin__Tables__{tableIndex}__Columns__{colIndex}__VirtualType"
                ] = column.VirtualType.ToString();
                providerEnvs[
                    $"Clouded__Admin__Tables__{tableIndex}__Columns__{colIndex}__List__Visible"
                ] = column.List.Visible.ToString();
                providerEnvs[
                    $"Clouded__Admin__Tables__{tableIndex}__Columns__{colIndex}__List__Readonly"
                ] = column.List.Readonly.ToString();
                providerEnvs[
                    $"Clouded__Admin__Tables__{tableIndex}__Columns__{colIndex}__Create__Visible"
                ] = column.Create.Visible.ToString();
                providerEnvs[
                    $"Clouded__Admin__Tables__{tableIndex}__Columns__{colIndex}__Create__Readonly"
                ] = column.Create.Readonly.ToString();
                providerEnvs[
                    $"Clouded__Admin__Tables__{tableIndex}__Columns__{colIndex}__Detail__Visible"
                ] = column.Detail.Visible.ToString();
                providerEnvs[
                    $"Clouded__Admin__Tables__{tableIndex}__Columns__{colIndex}__Detail__Readonly"
                ] = column.Detail.Readonly.ToString();

                colIndex++;
            }

            ConfigOfTableFunction(providerEnvs, tableEntity, EAdminProviderFunctionTrigger.Create);
            ConfigOfTableFunction(providerEnvs, tableEntity, EAdminProviderFunctionTrigger.Update);
            ConfigOfTableFunction(providerEnvs, tableEntity, EAdminProviderFunctionTrigger.Delete);

            tableIndex++;
        }

        return providerEnvs;

        void ConfigOfTableFunction(
            Dictionary<string, string> dictionary,
            AdminTablesConfigurationEntity tableEntity,
            EAdminProviderFunctionTrigger trigger
        )
        {
            var functionIndex = 0;
            foreach (
                var adminProviderFunctionRelationEntity in tableEntity.FunctionsRelation
                    .Where(
                        i =>
                            i.OperationType == trigger
                            && i.Function.Type == EFunctionType.BeforeHook
                    )
                    .ToList()
            )
            {
                dictionary[
                    $"Clouded__Admin__Tables__{tableIndex}__{trigger.GetEnumName()}OperationFunctions__BeforeHooks__{functionIndex}__SourceId"
                ] = adminProviderFunctionRelationEntity.Function.ProviderId.ToString();
                dictionary[
                    $"Clouded__Admin__Tables__{tableIndex}__{trigger.GetEnumName()}OperationFunctions__BeforeHooks__{functionIndex}__Name"
                ] = adminProviderFunctionRelationEntity.Function.Name;
                functionIndex++;
            }

            functionIndex = 0;
            foreach (
                var adminProviderFunctionRelationEntity in tableEntity.FunctionsRelation
                    .Where(
                        i =>
                            i.OperationType == trigger && i.Function.Type == EFunctionType.InputHook
                    )
                    .ToList()
            )
            {
                dictionary[
                    $"Clouded__Admin__Tables__{tableIndex}__{trigger.GetEnumName()}OperationFunctions__InputHooks__{functionIndex}__SourceId"
                ] = adminProviderFunctionRelationEntity.Function.ProviderId.ToString();
                dictionary[
                    $"Clouded__Admin__Tables__{tableIndex}__{trigger.GetEnumName()}OperationFunctions__InputHooks__{functionIndex}__Name"
                ] = adminProviderFunctionRelationEntity.Function.Name;
                functionIndex++;
            }

            functionIndex = 0;
            foreach (
                var adminProviderFunctionRelationEntity in tableEntity.FunctionsRelation
                    .Where(
                        i =>
                            i.OperationType == trigger
                            && i.Function.Type == EFunctionType.ValidationHook
                    )
                    .ToList()
            )
            {
                dictionary[
                    $"Clouded__Admin__Tables__{tableIndex}__{trigger.GetEnumName()}OperationFunctions__ValidationHooks__{functionIndex}__SourceId"
                ] = adminProviderFunctionRelationEntity.Function.ProviderId.ToString();
                dictionary[
                    $"Clouded__Admin__Tables__{tableIndex}__{trigger.GetEnumName()}OperationFunctions__ValidationHooks__{functionIndex}__Name"
                ] = adminProviderFunctionRelationEntity.Function.Name;
                functionIndex++;
            }

            functionIndex = 0;
            foreach (
                var adminProviderFunctionRelationEntity in tableEntity.FunctionsRelation
                    .Where(
                        i =>
                            i.OperationType == trigger && i.Function.Type == EFunctionType.AfterHook
                    )
                    .ToList()
            )
            {
                dictionary[
                    $"Clouded__Admin__Tables__{tableIndex}__{trigger.GetEnumName()}OperationFunctions__AfterHooks__{functionIndex}__SourceId"
                ] = adminProviderFunctionRelationEntity.Function.ProviderId.ToString();
                dictionary[
                    $"Clouded__Admin__Tables__{tableIndex}__{trigger.GetEnumName()}OperationFunctions__AfterHooks__{functionIndex}__Name"
                ] = adminProviderFunctionRelationEntity.Function.Name;
                functionIndex++;
            }
        }

        void ConfigOfGlobalFunction(
            Dictionary<string, string> dictionary,
            EAdminProviderFunctionTrigger trigger
        )
        {
            var functionIndex = 0;
            foreach (
                var adminProviderFunctionRelationEntity in adminProvider.FunctionsRelation
                    .Where(
                        i =>
                            i.OperationType == trigger
                            && i.Function.Type == EFunctionType.BeforeHook
                    )
                    .ToList()
            )
            {
                dictionary[
                    $"Clouded__Admin__Global{trigger.GetEnumName()}OperationFunctions__BeforeHooks__{functionIndex}__SourceId"
                ] = adminProviderFunctionRelationEntity.Function.ProviderId.ToString();
                dictionary[
                    $"Clouded__Admin__Global{trigger.GetEnumName()}OperationFunctions__BeforeHooks__{functionIndex}__Name"
                ] = adminProviderFunctionRelationEntity.Function.Name;
                functionIndex++;
            }

            functionIndex = 0;
            foreach (
                var adminProviderFunctionRelationEntity in adminProvider.FunctionsRelation
                    .Where(
                        i =>
                            i.OperationType == trigger && i.Function.Type == EFunctionType.InputHook
                    )
                    .ToList()
            )
            {
                dictionary[
                    $"Clouded__Admin__Global{trigger.GetEnumName()}OperationFunctions__InputHooks__{functionIndex}__SourceId"
                ] = adminProviderFunctionRelationEntity.Function.ProviderId.ToString();
                dictionary[
                    $"Clouded__Admin__Global{trigger.GetEnumName()}OperationFunctions__InputHooks__{functionIndex}__Name"
                ] = adminProviderFunctionRelationEntity.Function.Name;
                functionIndex++;
            }

            functionIndex = 0;
            foreach (
                var adminProviderFunctionRelationEntity in adminProvider.FunctionsRelation
                    .Where(
                        i =>
                            i.OperationType == trigger
                            && i.Function.Type == EFunctionType.ValidationHook
                    )
                    .ToList()
            )
            {
                dictionary[
                    $"Clouded__Admin__Global{trigger.GetEnumName()}OperationFunctions__ValidationHooks__{functionIndex}__SourceId"
                ] = adminProviderFunctionRelationEntity.Function.ProviderId.ToString();
                dictionary[
                    $"Clouded__Admin__Global{trigger.GetEnumName()}OperationFunctions__ValidationHooks__{functionIndex}__Name"
                ] = adminProviderFunctionRelationEntity.Function.Name;
                functionIndex++;
            }

            functionIndex = 0;
            foreach (
                var adminProviderFunctionRelationEntity in adminProvider.FunctionsRelation
                    .Where(
                        i =>
                            i.OperationType == trigger && i.Function.Type == EFunctionType.AfterHook
                    )
                    .ToList()
            )
            {
                dictionary[
                    $"Clouded__Admin__Global{trigger.GetEnumName()}OperationFunctions__AfterHooks__{functionIndex}__SourceId"
                ] = adminProviderFunctionRelationEntity.Function.ProviderId.ToString();
                dictionary[
                    $"Clouded__Admin__Global{trigger.GetEnumName()}OperationFunctions__AfterHooks__{functionIndex}__Name"
                ] = adminProviderFunctionRelationEntity.Function.Name;
                functionIndex++;
            }
        }
    }

    public static Dictionary<string, string> ComposeFunctionEnvs(
        FunctionProviderEntity? functionProvider
    )
    {
        if (functionProvider == null)
            return new Dictionary<string, string>();

        var functionConfig = functionProvider.Configuration;
        var providerEnvs = new Dictionary<string, string>
        {
            ["Clouded__Function__ApiKey"] = functionConfig.ApiKey
        };

        var groupIndex = 0;
        foreach (var groupHooks in functionProvider.Functions.GroupBy(x => x.ExecutableName))
        {
            providerEnvs.Add(
                $"Clouded__Function__Hooks__{groupIndex}__Path",
                $"/executables/{groupHooks.Key}/CloudedFunctionEPRN.dll"
            );

            var hookIndex = 0;
            foreach (var hook in groupHooks)
            {
                providerEnvs.Add(
                    $"Clouded__Function__Hooks__{groupIndex}__Map__{hookIndex}__Name",
                    hook.Name
                );

                providerEnvs.Add(
                    $"Clouded__Function__Hooks__{groupIndex}__Map__{hookIndex}__Type",
                    hook.Type.GetEnumName()
                );

                hookIndex++;
            }

            groupIndex++;
        }

        return providerEnvs;
    }
}
