# Dns Setup
## Install
```
brew install dnsmasq
```

## Setup

### Create config directory
```
mkdir -pv $(brew --prefix)/etc/
```

### Setup *.local
```
echo 'address=/.local/127.0.0.1' >> $(brew --prefix)/etc/dnsmasq.conf
```
## Autostart - now and after reboot
```
sudo brew services start dnsmasq
```

## Add to resolvers

### Create resolver directory
```
sudo mkdir -v /etc/resolver
```

### Add your nameserver to resolvers
```
sudo bash -c 'echo "nameserver 127.0.0.1" > /etc/resolver/local'
```
## Autostart - now and after reboot
```
sudo brew services restart dnsmasq
```


# Kubernetes Setup
## Helm install
```
brew install helm
helm plugin install https://github.com/databus23/helm-diff
brew install helmfile
```

## Inside kubernetes folder run
```
helmfile apply
```