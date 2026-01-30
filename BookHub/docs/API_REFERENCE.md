# API Reference

## Authentification

L'authentification repose sur des **JWT (JSON Web Tokens)**.

* Les tokens sont obtenus via le **User Service** (`/api/users/login`)
* Les endpoints protégés nécessitent l'en-tête HTTP suivant :

```
Authorization: Bearer {token}
```

---

## Catalog Service

Base URL : `/api/books`

### GET /api/books

Récupère la liste complète des livres.

**Réponse**

* `200 OK`

---

### GET /api/books/{id}

Récupère les détails d’un livre par son identifiant.

**Paramètres**

* `id` (GUID) : identifiant du livre

**Réponses**

* `200 OK`
* `404 Not Found`

---

### GET /api/books/search?term=

Recherche des livres par mot-clé.

**Paramètres**

* `term` (string) : terme de recherche

**Réponse**

* `200 OK`

---

### GET /api/books/category/{category}

Récupère les livres par catégorie.

**Paramètres**

* `category` (string)

**Réponse**

* `200 OK`

---

### POST /api/books

Crée un nouveau livre.

**Corps de la requête** : `CreateBookDto`

**Réponses**

* `201 Created`

---

### PUT /api/books/{id}

Met à jour un livre existant.

**Réponses**

* `200 OK`
* `404 Not Found`

---

### DELETE /api/books/{id}

Supprime un livre.

**Réponses**

* `204 No Content`
* `404 Not Found`

---

### POST /api/books/{id}/decrement-availability

Décrémente la disponibilité d’un livre (emprunt).

**Réponses**

* `200 OK`
* `400 Bad Request`

---

### POST /api/books/{id}/increment-availability

Incrémente la disponibilité d’un livre (retour).

**Réponses**

* `200 OK`
* `400 Bad Request`

---

## User Service

Base URL : `/api/users`

### GET /api/users

Récupère la liste des utilisateurs.

**Sécurité**

* Rôles requis : `Admin`, `Librarian`

**Réponse**

* `200 OK`

---

### GET /api/users/{id}

Récupère le profil d’un utilisateur.

**Sécurité**

* Authentification requise

**Réponses**

* `200 OK`
* `404 Not Found`

---

### POST /api/users/register

Inscription d’un nouvel utilisateur.

**Corps de la requête** : `CreateUserDto`

**Réponses**

* `201 Created`
* `400 Bad Request`

---

### POST /api/users/login

Connexion utilisateur.

**Corps de la requête** : `LoginDto`

**Réponses**

* `200 OK` (JWT retourné)
* `401 Unauthorized`

---

### PUT /api/users/{id}

Met à jour un utilisateur.

**Sécurité**

* Authentification requise

**Réponses**

* `200 OK`
* `404 Not Found`

---

### DELETE /api/users/{id}

Supprime un utilisateur.

**Sécurité**

* Rôle requis : `Admin`

**Réponses**

* `204 No Content`
* `404 Not Found`

---

## Loan Service

Base URL : `/api/loans`

### GET /api/loans

Récupère la liste de tous les emprunts.

**Réponse**

* `200 OK`

---

### GET /api/loans/{id}

Récupère les détails d’un emprunt.

**Paramètres**

* `id` (GUID)

**Réponses**

* `200 OK`
* `404 Not Found`

---

### GET /api/loans/user/{userId}

Récupère les emprunts d’un utilisateur.

**Paramètres**

* `userId` (GUID)

**Réponse**

* `200 OK`

---

### GET /api/loans/overdue

Récupère la liste des emprunts en retard.

**Statut**

* Non implémenté

**Réponse**

* `501 Not Implemented`

---

### POST /api/loans

Crée un nouvel emprunt.

**Corps de la requête** : `CreateLoanDto`

**Règles métier**

* Maximum 5 emprunts simultanés par utilisateur
* Durée maximale : 21 jours
* Un livre déjà emprunté ne peut pas l’être à nouveau

**Statut**

* Non implémenté

---

### PUT /api/loans/{id}/return

Retourne un livre emprunté.

**Paramètres**

* `id` (GUID)

**Statut**

* Non implémenté

