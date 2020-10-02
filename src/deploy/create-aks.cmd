SET RG=dotnet2020
set AKS=dotnet2020
set LOCATION=westeurope

call az group create -n %RG% --location %LOCATION%
call az aks create -g %RG% -n %AKS% --node-count 2 --enable-managed-identity --location %LOCATION%

call az aks get-credentials -n %AKS% -g %RG%

REM Deploy traefik  ingress controller
call helm repo add stable https://kubernetes-charts.storage.googleapis.com/
call helm repo update
call helm install traefik-ingress stable/traefik --namespace traefik-ingress --create-namespace --set kubernetes.ingressClass=traefik --set rbac.enabled=true --set kubernetes.ingressEndpoint.useDefaultPublishedService=true --version 1.85.0