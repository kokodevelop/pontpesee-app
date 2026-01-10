# 🚀 PONT PESÉE - Application Web

Application web moderne pour la gestion des pesées sur les sites SOPRECI GUTRI et SOPRECI OLODIO.

## 📋 Table des matières

- [Architecture](#architecture)
- [Technologies](#technologies)
- [Installation](#installation)
- [Configuration](#configuration)
- [Démarrage](#démarrage)
- [API Documentation](#api-documentation)
- [Frontend](#frontend)
- [Base de données](#base-de-données)

---

## 🏗️ Architecture

```
PONT_WEBAPP/
├── backend/                     # ASP.NET Core 8 Web API
│   └── PontPesee.API/
│       ├── Controllers/         # API Controllers
│       ├── Services/            # Business Logic
│       ├── Models/              # Entity Models
│       ├── DTOs/                # Data Transfer Objects
│       ├── Data/                # DbContext
│       └── appsettings.json     # Configuration
│
└── frontend/                    # Vue.js 3 + Vuetify
    └── pont-pesee-app/
        ├── src/
        │   ├── views/           # Pages principales
        │   ├── components/      # Composants réutilisables
        │   ├── services/        # API Service Layer
        │   ├── stores/          # Pinia State Management
        │   └── router/          # Vue Router
        └── package.json
```

---

## 🛠️ Technologies

### Backend
- **Framework**: ASP.NET Core 8 Web API
- **Base de données**: MySQL 8.0+
- **ORM**: Entity Framework Core 8
- **Authentification**: JWT Bearer Token
- **Documentation**: Swagger/OpenAPI

### Frontend
- **Framework**: Vue.js 3 (Composition API)
- **UI Library**: Vuetify 3
- **State Management**: Pinia
- **Routing**: Vue Router 4
- **HTTP Client**: Axios
- **Langage**: TypeScript

---

## 📦 Installation

### Prérequis

- **.NET SDK 8.0** ou supérieur
- **Node.js 18** ou supérieur
- **npm** ou **yarn**
- **MySQL Server 8.0+**

### 1. Cloner le projet

```bash
cd D:\PROJETVBNET2015\PONT VBPP2 CAM2ADMIN\PONT_WEBAPP
```

### 2. Installation Backend

```bash
cd backend/PontPesee.API
dotnet restore
dotnet build
```

### 3. Installation Frontend

```bash
cd ../../frontend/pont-pesee-app
npm install
```

---

## ⚙️ Configuration

### Backend - appsettings.json

Ouvrez `backend/PontPesee.API/appsettings.json` et configurez :

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=VOTRE_SERVEUR;Port=3306;Database=VOTRE_BASE;User=VOTRE_USER;Password=VOTRE_MDP;SslMode=none;"
  },
  "Jwt": {
    "Key": "VotreCleSecretePourJWT_MinimumTrenteDeuxCaracteres2025",
    "Issuer": "PontPeseeAPI",
    "Audience": "PontPeseeClient"
  },
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:5173",
      "http://localhost:3000"
    ]
  }
}
```

**Configuration de la connexion MySQL :**

Remplacez les valeurs suivantes :
- `VOTRE_SERVEUR` : Adresse de votre serveur MySQL (ex: `localhost` ou IP distante)
- `VOTRE_BASE` : Nom de votre base de données (celle qui contient `pesee` et `peseep2`)
- `VOTRE_USER` : Nom d'utilisateur MySQL
- `VOTRE_MDP` : Mot de passe MySQL

**Exemple pour connexion locale :**
```json
"DefaultConnection": "Server=localhost;Port=3306;Database=gestpeseedb;User=root;Password=monmotdepasse;SslMode=none;"
```

**Exemple pour connexion distante :**
```json
"DefaultConnection": "Server=54.36.224.114;Port=3306;Database=gestpeseedb;User=pontuser;Password=Soprol@2022;SslMode=none;"
```

### Frontend - Variables d'environnement

Créez un fichier `.env` dans `frontend/pont-pesee-app/` :

```bash
VITE_API_URL=http://localhost:5000/api
```

Pour la production, utilisez `.env.production` :

```bash
VITE_API_URL=https://votre-api.com/api
```

---

## 🚀 Démarrage

### 1. Démarrer le Backend

```bash
cd backend/PontPesee.API
dotnet run
```

L'API sera accessible sur :
- **HTTP**: http://localhost:5000
- **HTTPS**: https://localhost:5001
- **Swagger UI**: http://localhost:5000 (documentation interactive)

### 2. Démarrer le Frontend

Dans un nouveau terminal :

```bash
cd frontend/pont-pesee-app
npm run dev
```

L'application sera accessible sur : **http://localhost:5173**

---

## 📚 API Documentation

### Endpoints principaux

#### Authentification

**POST** `/api/Auth/login`
```json
// Request
{
  "username": "admin",
  "password": "password123"
}

// Response
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "username": "admin",
  "profil": "SUPERVISEUR",
  "expiresAt": "2025-01-10T12:00:00Z"
}
```

#### Pesées

**POST** `/api/Pesee/search`
```json
// Request
{
  "tableName": "pesee",
  "dateDebut": "2025-01-01",
  "dateFin": "2025-01-10",
  "mouvement": "ENTREE",
  "acceptes": true,
  "page": 1,
  "pageSize": 50
}

// Response
{
  "nombrePesees": 150,
  "poidsBrutTotal": 50000.5,
  "poidsNetTotal": 48500.2,
  "pesees": [...],
  "totalPages": 3,
  "currentPage": 1
}
```

**GET** `/api/Pesee/ticket/{numTicket}?tableName=pesee`

**GET** `/api/Pesee/provenances?tableName=pesee`

**GET** `/api/Pesee/destinations?tableName=pesee`

**GET** `/api/Pesee/fournisseurs?tableName=pesee`

**GET** `/api/Pesee/clients?tableName=pesee`

**GET** `/api/Pesee/produits?tableName=pesee`

**GET** `/api/Pesee/vehicules?tableName=pesee`

### Authentification JWT

Toutes les routes (sauf `/api/Auth/login`) nécessitent un token JWT dans le header :

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

---

## 🎨 Frontend

### Structure des pages

1. **Login** (`/login`)
   - Authentification des utilisateurs
   - Stockage du token JWT

2. **Dashboard** (`/`)
   - Menu principal avec tuiles
   - Accès rapide aux fonctionnalités

3. **Stats** (`/stats`)
   - Consultation et filtrage des pesées
   - Sélecteur de site (GUTRI/OLODIO)
   - Export Excel
   - Génération de rapports PDF

4. **Réédition** (`/reedition`)
   - Réimpression de tickets
   - Recherche par numéro de ticket

### Composants principaux

- **SiteSelector**: Sélection du site (pesee/peseep2)
- **PeseeDataTable**: Grille de données avec filtres
- **FilterPanel**: Panneau de filtres avancés
- **StatsCards**: Cartes de statistiques

---

## 🗄️ Base de données

### Tables utilisées

#### Table `user`
```sql
CREATE TABLE `user` (
  `id` INT PRIMARY KEY AUTO_INCREMENT,
  `NomUt` VARCHAR(100) NOT NULL,
  `motpasse` VARCHAR(255) NOT NULL,
  `LeGroupe` VARCHAR(50),
  `Actif` BOOLEAN
);
```

#### Table `pesee` (SOPRECI GUTRI)
Structure existante de votre base de données

#### Table `peseep2` (SOPRECI OLODIO)
Structure identique à `pesee`

### Index recommandés

Pour optimiser les performances, assurez-vous que ces index existent :

```sql
-- Table pesee
CREATE INDEX idx_pesee_imprime ON pesee(imprime);
CREATE INDEX idx_pesee_numticket ON pesee(NumTicket);
CREATE INDEX idx_pesee_dmv ON pesee(dmv);
CREATE INDEX idx_pesee_codesite ON pesee(CodeSite);

-- Table peseep2
CREATE INDEX idx_peseep2_imprime ON peseep2(imprime);
CREATE INDEX idx_peseep2_numticket ON peseep2(NumTicket);
CREATE INDEX idx_peseep2_dmv ON peseep2(dmv);
CREATE INDEX idx_peseep2_codesite ON peseep2(CodeSite);
```

---

## 🔐 Sécurité

### Bonnes pratiques

1. **JWT Secret Key**: Changez la clé dans `appsettings.json` en production
2. **HTTPS**: Utilisez HTTPS en production
3. **CORS**: Limitez les origines autorisées
4. **Mot de passe**: Implémentez le hashage (BCrypt recommandé)
5. **Variables d'environnement**: Ne commitez jamais les fichiers de config avec des secrets

### Gestion des rôles

- **PESEUR**: Consultation limitée
- **SUPERVISEUR**: Accès complet

---

## 📝 Développement

### Commandes utiles

**Backend:**
```bash
# Compiler
dotnet build

# Exécuter
dotnet run

# Créer une migration EF
dotnet ef migrations add InitialCreate

# Appliquer les migrations
dotnet ef database update

# Publier pour production
dotnet publish -c Release
```

**Frontend:**
```bash
# Développement
npm run dev

# Build production
npm run build

# Preview build
npm run preview

# Linter
npm run lint

# Tests
npm run test
```

---

## 🐛 Dépannage

### Backend ne démarre pas

1. Vérifiez la connexion MySQL :
```bash
mysql -h SERVEUR -u USER -p
```

2. Vérifiez les logs dans la console

3. Testez la connexion dans `appsettings.json`

### Frontend ne se connecte pas au backend

1. Vérifiez que le backend est démarré
2. Vérifiez l'URL dans `.env`
3. Vérifiez les CORS dans `appsettings.json`
4. Ouvrez la console du navigateur (F12)

### Erreur "Unauthorized" (401)

- Token expiré : reconnectez-vous
- Token invalide : vérifiez la clé JWT côté backend

---

## 📞 Support

Pour toute question ou problème :

1. Consultez la documentation Swagger : `http://localhost:5000`
2. Vérifiez les logs du backend
3. Consultez la console du navigateur pour le frontend

---

## 📄 Licence

Propriétaire : SOPRECI - Tous droits réservés

---

## ✨ Améliorations futures

- [ ] Export PDF des rapports
- [ ] Notifications en temps réel
- [ ] Dashboard avec graphiques
- [ ] Import/Export Excel avancé
- [ ] Gestion avancée des utilisateurs
- [ ] Audit trail complet
- [ ] Mode hors ligne (PWA)
- [ ] Multi-langue (i18n)

---

**Version**: 1.0.0
**Date**: Janvier 2025
**Développé avec** ❤️ **pour SOPRECI**
