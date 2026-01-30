# Deployment


Pour exécuter l'application, il suffit de lancer les conteneurs docker connectés via la gateway.
Pour cela, ouvrir un cmd dans le répertoire de l'application et entrer la commande
```cmd
docker compose up -d
```
puis vérifier que tous les conteneurs sont bien lancés.
```cmd
docker ps
```
Le site est désormais accessible à l'adresse : http://localhost:8080/
