@echo off

REM Install Dapr
dapr init --kubernetes

REM Deploy Redis
helm repo add stable https://kubernetes-charts.storage.googleapis.com/
helm repo update
helm install redis stable/redis --set rbac.create=true --set password=DcfhWwl0hZ

REM Deploy Dapr Components
kubectl apply -f k8s-dapr

REM Deploy SQL Server
helm install sql stable/mssql-linux --set acceptEula.value=Y --set sapassword=Pass@word1 --set edition.value=Developer