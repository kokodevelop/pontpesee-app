# 🚀 Installation et Démarrage du Frontend

## ✅ Pages Vue.js Créées

1. ✅ **LoginView.vue** - Page de connexion avec authentification
2. ✅ **DashboardView.vue** - Menu principal avec tuiles de navigation
3. ✅ **StatsView.vue** - Tableau de pesées avec filtres avancés
4. ✅ **ReeditionView.vue** - Recherche et réédition de tickets

## 📦 Installation des Dépendances

### 1. Installer Vuetify et Material Design Icons

```bash
cd frontend/pont-pesee-app
npm install vuetify @mdi/font
```

### 2. Vérifier que toutes les dépendances sont installées

```bash
npm install
```

## 🎨 Configuration

Tous les fichiers de configuration ont été créés:
- ✅ `src/plugins/vuetify.ts` - Configuration Vuetify
- ✅ `src/main.ts` - Point d'entrée mis à jour
- ✅ `src/router/index.ts` - Routes avec guards d'authentification
- ✅ `src/App.vue` - Application principale simplifiée

## 🔧 Fichiers Existants à Vérifier

### `src/services/api.ts`
Le fichier existe déjà et contient les appels API.

### `src/stores/auth.ts`
Le store Pinia pour l'authentification existe déjà.

### `.env`
Vérifiez que le fichier contient:
```
VITE_API_URL=http://localhost:5059/api
```

## 🚀 Démarrage

### 1. Démarrer le Backend (dans un terminal)

```bash
cd backend/PontPesee.API
dotnet run
```

Le backend démarrera sur http://localhost:5059

### 2. Démarrer le Frontend (dans un autre terminal)

```bash
cd frontend/pont-pesee-app
npm run dev
```

Le frontend démarrera sur http://localhost:5173

## 🧪 Test de l'Application

1. **Ouvrez le navigateur** : http://localhost:5173

2. **Vous devriez voir la page de login**

3. **Connectez-vous** avec vos identifiants:
   - Username: admin
   - Password: 0101

4. **Vous arriverez sur le Dashboard** avec 3 tuiles:
   - Statistiques
   - Réédition
   - Export Excel (bientôt disponible)

5. **Testez la page Statistiques**:
   - Sélectionnez un site (GUTRI ou OLODIO)
   - Choisissez une plage de dates
   - Filtrez par fournisseur, client, produit
   - Cliquez sur "Rechercher"

6. **Testez la Réédition**:
   - Sélectionnez un site
   - Entrez un numéro de ticket (ex: 68175/PBS/5/2025)
   - Cliquez sur "Rechercher"

## 📂 Structure des Pages

### LoginView.vue
- Formulaire de connexion avec validation
- Gestion des erreurs
- Design Material avec Vuetify

### DashboardView.vue
- 3 tuiles de navigation animées
- 4 cartes de statistiques rapides
- Menu de déconnexion

### StatsView.vue
- Filtres avancés (site, dates, mouvement, fournisseur, client, produit)
- 3 cartes de statistiques (nombre, poids brut, poids net)
- Tableau de données avec tri
- Chips colorés pour mouvement et statut
- Bouton d'impression par ligne

### ReeditionView.vue
- Recherche par numéro de ticket
- Affichage complet des informations
- Bouton d'impression du ticket

## 🎨 Thème et Design

- **Framework UI** : Vuetify 3
- **Icons** : Material Design Icons
- **Thème** : Light avec couleurs personnalisées
  - Primary: #1976D2 (Bleu)
  - Success: #4CAF50 (Vert)
  - Warning: #FFC107 (Orange)
  - Error: #FF5252 (Rouge)

## 🔐 Authentification

- Store Pinia pour gérer l'état d'authentification
- Token JWT stocké dans localStorage
- Navigation guards pour protéger les routes
- Redirection automatique vers login si non authentifié

## 📱 Responsive

Toutes les pages sont responsive et s'adaptent aux différentes tailles d'écran:
- Mobile (< 600px)
- Tablet (600px - 960px)
- Desktop (> 960px)

## 🐛 Dépannage

### Erreur: Module 'vuetify' not found
```bash
npm install vuetify @mdi/font
```

### Erreur: CORS
Vérifiez que le backend est configuré pour accepter http://localhost:5173

### Erreur 401 sur les API
- Vérifiez que vous êtes connecté
- Le token est peut-être expiré, reconnectez-vous

### Le frontend ne démarre pas
```bash
# Supprimer node_modules et réinstaller
rm -rf node_modules
npm install
npm run dev
```

## 🎯 Prochaines Étapes

1. ✅ Tester toutes les pages
2. ⏳ Implémenter l'impression de tickets (PDF?)
3. ⏳ Implémenter l'export Excel
4. ⏳ Ajouter plus de statistiques au Dashboard
5. ⏳ Déploiement en production

## 📞 Support

Si vous rencontrez des problèmes, vérifiez:
1. Le backend tourne sur http://localhost:5059
2. Le frontend tourne sur http://localhost:5173
3. Les logs du navigateur (F12 → Console)
4. Les logs du backend (terminal PowerShell)

---

**🎉 L'application est prête à être testée!**
