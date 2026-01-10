# 🏗️ Architecture Technique - PONT PESÉE WebApp

## Vue d'ensemble

Application full-stack moderne pour la gestion des pesées sur deux sites (SOPRECI GUTRI et OLODIO).

---

## 📐 Schéma d'Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    NAVIGATEUR WEB                            │
│  ┌────────────────────────────────────────────────────────┐ │
│  │         Vue.js 3 Frontend (Port 5173)                  │ │
│  │  ┌─────────────┬─────────────┬─────────────────────┐  │ │
│  │  │  Dashboard  │    Stats    │     Reedition       │  │ │
│  │  │   (Menu)    │  (Pesées)   │     (Tickets)       │  │ │
│  │  └─────────────┴─────────────┴─────────────────────┘  │ │
│  │  ┌──────────────────────────────────────────────────┐  │ │
│  │  │  Vuetify 3 Components + Pinia Store              │  │ │
│  │  └──────────────────────────────────────────────────┘  │ │
│  └────────────────────────────────────────────────────────┘ │
└──────────────────────┬──────────────────────────────────────┘
                       │ HTTP/REST + JWT
                       │ (Axios)
┌──────────────────────▼──────────────────────────────────────┐
│           ASP.NET Core 8 Web API (Port 5000)                │
│  ┌────────────────────────────────────────────────────────┐ │
│  │              API Controllers                            │ │
│  │  ┌─────────────┬──────────────┬──────────────────────┐ │ │
│  │  │    Auth     │    Pesee     │    (Extensible)      │ │ │
│  │  │ Controller  │  Controller  │                       │ │ │
│  │  └─────────────┴──────────────┴──────────────────────┘ │ │
│  ├────────────────────────────────────────────────────────┤ │
│  │              Business Logic Layer                       │ │
│  │  ┌─────────────┬──────────────┐                        │ │
│  │  │ AuthService │ PeseeService │                        │ │
│  │  └─────────────┴──────────────┘                        │ │
│  ├────────────────────────────────────────────────────────┤ │
│  │           Data Access Layer (EF Core)                   │ │
│  │  ┌─────────────────────────────────────────────────┐   │ │
│  │  │     ApplicationDbContext                         │   │ │
│  │  └─────────────────────────────────────────────────┘   │ │
│  └────────────────────────────────────────────────────────┘ │
└──────────────────────┬──────────────────────────────────────┘
                       │ MySQL Connector
                       │ Entity Framework Core
┌──────────────────────▼──────────────────────────────────────┐
│              MySQL Database Server                           │
│  ┌────────────────────────────────────────────────────────┐ │
│  │  Tables:                                                │ │
│  │  • pesee (SOPRECI GUTRI)                               │ │
│  │  • peseep2 (SOPRECI OLODIO)                            │ │
│  │  • user (Authentification)                             │ │
│  └────────────────────────────────────────────────────────┘ │
└──────────────────────────────────────────────────────────────┘
```

---

## 🔐 Flux d'Authentification

```
┌──────────┐              ┌──────────┐              ┌──────────┐
│          │   1. Login   │          │  2. Verify   │          │
│  Client  │─────────────>│   API    │─────────────>│  MySQL   │
│ (Vue.js) │              │(ASP.NET) │              │  (user)  │
│          │<─────────────│          │<─────────────│          │
└──────────┘  4. JWT Token└──────────┘ 3. User Found└──────────┘
     │
     │ 5. Store token in localStorage
     │
     ▼
┌──────────────────────────────────┐
│  Subsequent requests include:    │
│  Authorization: Bearer {token}   │
└──────────────────────────────────┘
```

---

## 📊 Flux de Récupération des Données

```
┌────────────────────────────────────────────────────────────────┐
│  1. User sélectionne un site (GUTRI ou OLODIO)                │
└────────────────┬───────────────────────────────────────────────┘
                 │
                 ▼
┌────────────────────────────────────────────────────────────────┐
│  2. Frontend envoie une requête avec tableName                │
│     POST /api/Pesee/search                                     │
│     { "tableName": "peseep2", "dateDebut": "...", ... }       │
└────────────────┬───────────────────────────────────────────────┘
                 │
                 ▼
┌────────────────────────────────────────────────────────────────┐
│  3. PeseeService détermine la table à interroger              │
│     - pesee (GUTRI) ou peseep2 (OLODIO)                       │
│     - Applique les filtres (dates, mouvement, etc.)           │
└────────────────┬───────────────────────────────────────────────┘
                 │
                 ▼
┌────────────────────────────────────────────────────────────────┐
│  4. Entity Framework exécute la requête SQL                    │
│     SELECT * FROM pesee WHERE imprime=true AND ...             │
└────────────────┬───────────────────────────────────────────────┘
                 │
                 ▼
┌────────────────────────────────────────────────────────────────┐
│  5. API retourne les résultats + statistiques                 │
│     { nombrePesees: 150, poidsBrutTotal: 50000, ... }        │
└────────────────┬───────────────────────────────────────────────┘
                 │
                 ▼
┌────────────────────────────────────────────────────────────────┐
│  6. Frontend affiche les données dans la DataTable            │
└────────────────────────────────────────────────────────────────┘
```

---

## 🗂️ Structure Backend

### Couches de l'application

#### 1. **Controllers Layer** (Point d'entrée API)
```
AuthController.cs
├── POST /api/Auth/login      → Authentification
└── POST /api/Auth/logout     → Déconnexion

PeseeController.cs
├── POST /api/Pesee/search    → Recherche avec filtres
├── GET  /api/Pesee/ticket/{id} → Récupérer un ticket
├── GET  /api/Pesee/provenances → Liste des provenances
├── GET  /api/Pesee/destinations → Liste des destinations
├── GET  /api/Pesee/fournisseurs → Liste des fournisseurs
├── GET  /api/Pesee/clients    → Liste des clients
├── GET  /api/Pesee/produits   → Liste des produits
└── GET  /api/Pesee/vehicules  → Liste des véhicules
```

#### 2. **Services Layer** (Logique métier)
```
IAuthService / AuthService
├── AuthenticateAsync()       → Vérifie credentials
└── GenerateJwtToken()        → Génère le token JWT

IPeseeService / PeseeService
├── GetPeseesAsync()          → Récupère pesées avec filtres
├── GetPeseeByTicketAsync()   → Récupère un ticket
├── GetProvenancesAsync()     → Récupère liste provenances
└── ... (autres méthodes de lookup)
```

#### 3. **Models Layer** (Entités de la base)
```
Pesee.cs         → Correspond à tables pesee/peseep2
User.cs          → Correspond à table user
```

#### 4. **DTOs Layer** (Objets de transfert)
```
LoginDto.cs           → Request/Response de login
PeseeDto.cs           → Representation simplifiée d'une pesée
PeseeFilterDto.cs     → Filtres de recherche
PeseeStatsDto.cs      → Résultats avec statistiques
```

#### 5. **Data Layer** (Accès données)
```
ApplicationDbContext.cs
├── DbSet<Pesee>              → Accès table pesee
├── DbSet<User>               → Accès table user
└── GetPeseesByTable()        → Méthode dynamique pesee/peseep2
```

---

## 🎨 Structure Frontend

### Organisation Vue.js

```
src/
├── main.ts                    → Point d'entrée
├── App.vue                    → Composant racine
│
├── router/
│   └── index.ts               → Configuration des routes
│       ├── /                  → Dashboard
│       ├── /login             → Page de connexion
│       ├── /stats             → Page des statistiques
│       └── /reedition         → Page de réédition
│
├── stores/
│   ├── auth.ts                → Store Pinia pour l'auth
│   └── pesee.ts               → Store Pinia pour les pesées
│
├── services/
│   └── api.ts                 → Service HTTP (Axios)
│       ├── authAPI            → Méthodes d'authentification
│       └── peseeAPI           → Méthodes de gestion pesées
│
├── views/                     → Pages principales
│   ├── LoginView.vue
│   ├── DashboardView.vue
│   ├── StatsView.vue
│   └── ReeditionView.vue
│
└── components/                → Composants réutilisables
    ├── SiteSelector.vue       → Sélecteur GUTRI/OLODIO
    ├── PeseeDataTable.vue     → Grille de données
    ├── FilterPanel.vue        → Panneau de filtres
    └── StatsCards.vue         → Cartes de statistiques
```

### Stores Pinia

#### **authStore**
```typescript
State:
  - token: string | null
  - username: string | null
  - profil: string | null

Getters:
  - isAuthenticated: boolean
  - isPeseur: boolean
  - isSuperviseur: boolean

Actions:
  - login(credentials)
  - logout()
```

#### **peseeStore** (à créer)
```typescript
State:
  - currentSite: 'pesee' | 'peseep2'
  - filters: PeseeFilter
  - pesees: Pesee[]
  - stats: Stats

Actions:
  - setSite(tableName)
  - searchPesees(filters)
  - loadLookupData()
```

---

## 🔄 Gestion Multi-Sites

### Approche technique

Le système gère deux sites (GUTRI et OLODIO) en utilisant **deux tables distinctes** :
- `pesee` → SOPRECI GUTRI
- `peseep2` → SOPRECI OLODIO

#### Implémentation

**Frontend** :
```typescript
// User sélectionne le site
const currentTable = ref<string>('pesee')

function changeSite(site: 'GUTRI' | 'OLODIO') {
  currentTable.value = site === 'GUTRI' ? 'pesee' : 'peseep2'
  // Recharger les données
  loadPesees()
}
```

**Backend** :
```csharp
// Dans PeseeService
var query = tableName.ToLower() == "peseep2"
    ? _context.Set<Pesee>().FromSqlRaw("SELECT * FROM peseep2")
    : _context.Pesees.AsQueryable();
```

**Avantages** :
- ✅ Pas de modification de schéma
- ✅ Performance optimale
- ✅ Isolation des données par site
- ✅ Flexibilité pour règles métier spécifiques

---

## 🔒 Sécurité

### 1. Authentification JWT

```
Workflow:
1. User envoie username + password
2. API vérifie dans table `user`
3. Si OK → Génère JWT token (durée 8h)
4. Client stocke token dans localStorage
5. Chaque requête inclut: Authorization: Bearer {token}
6. API valide le token avant chaque action
```

### 2. Autorisation par rôle

```csharp
[Authorize] // Nécessite un token valide
public class PeseeController : ControllerBase
{
    [HttpPost("search")]
    public async Task<ActionResult> Search()
    {
        // Accessible à tous les utilisateurs authentifiés
    }
}
```

**Future** : Ajouter vérification par rôle
```csharp
[Authorize(Roles = "SUPERVISEUR")]
public async Task<ActionResult> Delete()
{
    // Accessible uniquement aux superviseurs
}
```

### 3. CORS (Cross-Origin Resource Sharing)

Le backend autorise uniquement les origines configurées :
```json
"Cors": {
  "AllowedOrigins": [
    "http://localhost:5173"   // Frontend dev
  ]
}
```

---

## 📈 Performance

### Optimisations Backend

1. **Index de base de données** :
```sql
CREATE INDEX idx_pesee_imprime ON pesee(imprime);
CREATE INDEX idx_pesee_dmv ON pesee(dmv);
```

2. **Pagination** :
```csharp
query.Skip((page - 1) * pageSize).Take(pageSize)
```

3. **Filtres SQL côté serveur** :
```csharp
// Pas de chargement complet en mémoire
query.Where(p => p.Imprime && p.DatePesee >= dateDebut)
```

### Optimisations Frontend

1. **Lazy loading des routes** :
```typescript
{
  path: '/stats',
  component: () => import('@/views/StatsView.vue')
}
```

2. **Debounce sur les recherches** :
```typescript
const debouncedSearch = debounce(search, 300)
```

3. **Cache des données lookup** :
```typescript
// Charger une seule fois les listes
const provenances = await peseeAPI.getProvenances()
```

---

## 🔧 Extensibilité

### Ajouter un nouvel endpoint

**1. Backend** :
```csharp
// PeseeController.cs
[HttpPost("export-excel")]
public async Task<IActionResult> ExportExcel([FromBody] PeseeFilter filter)
{
    var data = await _peseeService.GetPeseesAsync(filter);
    // Générer Excel
    return File(excelBytes, "application/vnd.openxmlformats...", "pesees.xlsx");
}
```

**2. Frontend** :
```typescript
// api.ts
export const peseeAPI = {
  exportExcel: (filter: PeseeFilter) =>
    apiClient.post('/Pesee/export-excel', filter, { responseType: 'blob' })
}
```

### Ajouter une nouvelle page

**1. Créer la vue** : `src/views/MyNewView.vue`
**2. Ajouter la route** : `router/index.ts`
**3. Ajouter un lien** : Dans le Dashboard

---

## 📦 Déploiement

### Backend (IIS / Kestrel)

```bash
# Build release
dotnet publish -c Release -o ./publish

# Copier le dossier publish sur le serveur
# Configurer IIS ou exécuter directement
dotnet PontPesee.API.dll
```

### Frontend (Nginx / IIS)

```bash
# Build production
npm run build

# Le dossier dist/ contient les fichiers statiques
# Servir avec Nginx, IIS, ou Apache
```

---

## 📚 Technologies détaillées

### Backend Stack
- **ASP.NET Core 8** : Framework web
- **Entity Framework Core 8** : ORM
- **MySql.EntityFrameworkCore** : Provider MySQL
- **JWT Bearer** : Authentification
- **Swagger** : Documentation API

### Frontend Stack
- **Vue.js 3** : Framework progressif
- **TypeScript** : Typage statique
- **Vuetify 3** : Components Material Design
- **Pinia** : State management
- **Vue Router 4** : Routing
- **Axios** : HTTP client
- **Vite** : Build tool

---

**Version** : 1.0.0
**Dernière mise à jour** : Janvier 2025
