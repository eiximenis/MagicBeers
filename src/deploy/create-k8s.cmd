SET K8S=dotnet2020-kind
kind create cluster --config kind-k8s.yaml --name %K8S%

REM Deploy nginx ingress controller (I am using Kind K8s)
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/master/deploy/static/provider/kind/deploy.yaml
kubectl delete -A ValidatingWebhookConfiguration ingress-nginx-admission