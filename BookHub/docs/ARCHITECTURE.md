# Architecture

## 1.1 Analyse de l'Architecture Existante

### 1.1.1 Identifier les **patterns** utilises (Repository, Hexagonal Architecture, etc.)

#### Architecture Hexagonale
- Chaque microservices est structuré en hexagonal, selona la règle suivante : 
Domain, Application, Infrastructure, API.
Cela permet de faciliter les tests, le développement en équipe car chaque service est indépendant.
#### Repository Pattern
- Il est utilisé dans la couche Infrastructure et Persistence, Il permet de séparer la logique métier de la gestion des données.
#### API Gateway Pattern
- Permet un point d'entrée unique pour les appel entrants, simplifiant l'ajout de features éventuelles, les outils n'utilisant qu'une seule API

#### JWT Authentification
- L'authentification repose sur les tokens JWT qui sont générés par UserService

#### Docker conteuneurisation
- Chaque service possède son propre conteneur, cela permet un déploiement facilité et reproductible


### 1.1.2 Lister les **dependances** entre les services
- L'API Gateway dépend de tous les services (routage)
- Tous les services dépendent de PostgreSQL
- Client Blazor dépent de l'API Gateway (EntryPoint)
- LoanService dépend du CatalogService (stock disponible ?) et UserService (utilisateur existant ?)


### 1.1.3 Analyser les **choix techniques** et leur pertinence

- Microservices : 
Permet la scalabilité, la séparation du développement, permet de faciliter le travail à plusieurs sur le même projet.

- .NET 8 :
Encore très utilisé dans les entreprises, avec un LTS (Long time support) 

- Blazor
Intégration simple, utilisation de DTO, frontend simplifié avec utilisation de framework CSS possible.