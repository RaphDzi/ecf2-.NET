# 📚 BookHub

BookHub est une application de gestion de prêts de livres permettant aux utilisateurs de consulter les livres disponibles, gérer leurs prêts et suivre les retours. Le projet utilise **.NET 8**, **Blazor WebAssembly** pour le front-end, et **Docker** pour l’infrastructure (RabbitMQ, base de données, etc.).

---

## 🗂️ Structure du projet

- `BookHub.LoanService` : Service de gestion des prêts.
- `BookHub.UserService` : Service de gestion des utilisateurs.
- `BookHub.Shared` : DTOs et modèles partagés.
- `BookHub.Web` : Application Blazor WebAssembly.
- `docker-compose.yml` : Définition des services Docker pour RabbitMQ et la base de données.

---

## ⚙️ Prérequis

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Docker](https://www.docker.com/) en cours d’exécution
- [Visual Studio 2022+](https://visualstudio.microsoft.com/) ou VS Code

---

## 🚀 Lancer le projet

### 1 Cloner le dépôt

```bash
git clone <URL_DU_DEPOT>
cd BookHub
```

### 2 Lancer l'application
Lancer l'application via Docker et vérifier le bon fonctionnement des conteneurs 
```bash
docker compose up -d
docker ps
```

### 3 Lancer le service web
Accéder à l'adresse : http://localhost:8080/