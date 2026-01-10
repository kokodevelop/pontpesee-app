# Guide de Déploiement - PontPesee sur Azure (Gratuit)

## 🎯 Architecture de Déploiement

- **Backend (API ASP.NET Core)** → **Azure App Service** (Plan gratuit F1)
- **Frontend (Vue.js)** → **Azure Static Web Apps** (Gratuit)
- **Base de données MySQL** → Serveur existant `fga.sitsci.com:3306`

---

## 📋 Prérequis

1. **Compte Azure** (gratuit)
   - Créez un compte sur : https://azure.microsoft.com/free/
   - Vous obtenez $200 de crédit pour 30 jours + services gratuits permanents

2. **Azure CLI** (optionnel mais recommandé)
   - Téléchargez : https://aka.ms/installazurecli
   - Ou utilisez le portail web Azure

3. **Visual Studio** ou **Visual Studio Code** avec l'extension Azure

---

## 🚀 PARTIE 1 : Déployer le Backend (API)

### Méthode 1 : Depuis Visual Studio (Recommandé - Plus Simple)

#### Étape 1 : Préparer le projet

1. Ouvrez votre projet dans Visual Studio
2. Clic droit sur le projet **PontPesee.API**
3. Sélectionnez **Publish...**

#### Étape 2 : Configurer la publication Azure

1. Choisissez **Azure** comme cible
2. Cliquez sur **Next**
3. Sélectionnez **Azure App Service (Windows)** ou **(Linux)** - **Windows recommandé**
4. Cliquez sur **Next**

#### Étape 3 : Créer un nouveau App Service

1. Cliquez sur **Create New**
2. Remplissez les informations :
   - **Name** : `pontpesee-api` (doit être unique globalement)
   - **Subscription** : Votre abonnement Azure
   - **Resource Group** : Créez nouveau → `pontpesee-rg`
   - **Hosting Plan** : Créez nouveau
     - **Name** : `pontpesee-plan`
     - **Location** : `West Europe` (proche de la France)
     - **Size** : **F1 (Free)** ⚠️ Important : Sélectionnez le plan gratuit !

3. Cliquez sur **Create**

#### Étape 4 : Publier

1. Une fois l'App Service créé, cliquez sur **Publish**
2. Visual Studio va compiler et déployer automatiquement
3. Attendez la fin du déploiement (2-5 minutes)
4. L'URL sera : `https://pontpesee-api.azurewebsites.net`

---

### Méthode 2 : Via Azure CLI (Alternative)

```powershell
# Se connecter à Azure
az login

# Créer un groupe de ressources
az group create --name pontpesee-rg --location westeurope

# Créer un plan App Service gratuit
az appservice plan create --name pontpesee-plan --resource-group pontpesee-rg --sku F1

# Créer l'App Service
az webapp create --name pontpesee-api --resource-group pontpesee-rg --plan pontpesee-plan --runtime "DOTNET|8.0"

# Déployer depuis le dossier publish
cd "D:\PROJETVBNET2015\PONT VBPP2 CAM2ADMIN\PONT_WEBAPP\backend\PontPesee.API"
dotnet publish -c Release -o ./publish
cd publish
Compress-Archive -Path * -DestinationPath ../deploy.zip -Force
az webapp deployment source config-zip --resource-group pontpesee-rg --name pontpesee-api --src ../deploy.zip
```

---

### Configuration de l'App Service sur Azure Portal

#### Étape 5 : Configurer les Variables d'Environnement

1. Allez sur le **Portail Azure** : https://portal.azure.com
2. Recherchez votre App Service : `pontpesee-api`
3. Dans le menu de gauche : **Settings** → **Configuration**
4. Sous **Application settings**, ajoutez :

   | Nom | Valeur |
   |-----|--------|
   | `ASPNETCORE_ENVIRONMENT` | `Production` |
   | `ConnectionStrings__DefaultConnection` | `Server=fga.sitsci.com;Port=3306;Database=gestpeseedb;User=pontuser;Password=Soprol@2022;connect timeout=300;SslMode=none;` |
   | `Jwt__Key` | Votre clé JWT sécurisée (32+ caractères) |
   | `Jwt__Issuer` | `PontPeseeAPI` |
   | `Jwt__Audience` | `PontPeseeClient` |

5. Cliquez sur **Save** puis **Continue**

⚠️ **Note** : Pour les chaînes de connexion, utilisez `ConnectionStrings__DefaultConnection` (double underscore `__`)

#### Étape 6 : Configurer CORS (après déploiement du frontend)

1. Dans **Configuration** → **Application settings**, ajoutez :

   | Nom | Valeur |
   |-----|--------|
   | `Cors__AllowedOrigins__0` | `https://votre-app.azurestaticapps.net` |
   | `Cors__AllowedOrigins__1` | `https://fga.sitsci.com` |

   Ou modifiez `appsettings.Production.json` avant publication.

#### Étape 7 : Activer les Logs (pour debug)

1. Dans le menu : **Monitoring** → **App Service logs**
2. Activez **Application Logging (Filesystem)** → Level: **Error**
3. Cliquez sur **Save**

#### Étape 8 : Vérifier le déploiement

1. Allez sur : `https://pontpesee-api.azurewebsites.net`
2. Si Swagger est activé : `https://pontpesee-api.azurewebsites.net/swagger`
3. Testez un endpoint : `https://pontpesee-api.azurewebsites.net/api/test`

---

## 🎨 PARTIE 2 : Déployer le Frontend (Vue.js)

### Méthode 1 : Via Visual Studio Code + Extension Azure (Recommandé)

#### Étape 1 : Installer l'extension

1. Dans VS Code, installez **Azure Static Web Apps** extension
2. Connectez-vous à votre compte Azure (Ctrl+Shift+P → "Azure: Sign In")

#### Étape 2 : Créer une Static Web App

1. Ouvrez votre projet frontend Vue.js
2. Cliquez sur l'icône **Azure** dans la barre latérale
3. Sous **Static Web Apps**, cliquez sur **+** (Create Static Web App)
4. Suivez l'assistant :
   - **Name** : `pontpesee-frontend`
   - **Region** : `West Europe`
   - **Build Preset** : `Vue.js`
   - **App location** : `/` (racine de votre projet frontend)
   - **API location** : (laissez vide)
   - **Output location** : `dist`

5. VS Code va créer un workflow GitHub Actions automatiquement

#### Étape 3 : Configuration pour déploiement

1. Dans votre projet frontend Vue.js, créez/modifiez `.env.production` :

   ```env
   VITE_API_BASE_URL=https://pontpesee-api.azurewebsites.net
   ```

2. Assurez-vous que votre code utilise cette variable :

   ```javascript
   // Dans votre fichier de configuration API (ex: src/api/config.js)
   const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000';
   ```

#### Étape 4 : Build et déploiement

1. Committez vos modifications sur Git
2. Poussez vers GitHub
3. Le workflow GitHub Actions se déclenchera automatiquement
4. Attendez la fin du déploiement (3-5 minutes)
5. Votre app sera disponible sur : `https://pontpesee-frontend.azurestaticapps.net`

---

### Méthode 2 : Via Azure CLI (Alternative)

```powershell
# Installer l'extension Static Web Apps
az extension add --name staticwebapp

# Créer la Static Web App
az staticwebapp create \
  --name pontpesee-frontend \
  --resource-group pontpesee-rg \
  --location westeurope \
  --source https://github.com/votre-username/votre-repo \
  --branch main \
  --app-location "/" \
  --output-location "dist" \
  --login-with-github

# Ou déploiement manuel depuis le dossier dist
cd "D:\PROJETVBNET2015\PONT VBPP2 CAM2ADMIN\PONT_WEBAPP\frontend"
npm run build
az staticwebapp upload --name pontpesee-frontend --resource-group pontpesee-rg --source ./dist
```

---

## 🔧 Configuration Post-Déploiement

### Mettre à jour CORS dans le Backend

1. Retournez sur le Portail Azure
2. Allez dans votre App Service : `pontpesee-api`
3. **Configuration** → **Application settings**
4. Mettez à jour `Cors__AllowedOrigins__0` avec l'URL réelle de votre frontend :
   ```
   https://pontpesee-frontend.azurestaticapps.net
   ```
5. **Save** et redémarrez l'App Service

### Tester l'Application Complète

1. Ouvrez votre frontend : `https://pontpesee-frontend.azurestaticapps.net`
2. Testez la connexion à l'API
3. Vérifiez que l'authentification fonctionne
4. Testez les opérations CRUD

---

## 💰 Limitations du Plan Gratuit

### Azure App Service (F1 Free)

- ✅ 60 minutes de CPU par jour
- ✅ 1 Go de RAM
- ✅ 1 Go de stockage
- ✅ HTTPS automatique
- ❌ Pas de custom domain SSL
- ❌ L'app peut "dormir" après 20 min d'inactivité (temps de démarrage froid : 10-30s)

### Azure Static Web Apps (Free)

- ✅ 100 Go de bande passante/mois
- ✅ 0.5 Go de stockage
- ✅ HTTPS automatique
- ✅ Custom domains avec SSL gratuit
- ✅ Pas de "sommeil" - toujours actif

### MySQL (Votre serveur existant)

- ✅ Déjà configuré sur `fga.sitsci.com`
- ⚠️ Assurez-vous que le firewall autorise les connexions depuis Azure

---

## 🔐 Sécurité : Changements Importants

### 1. Changer la clé JWT

Dans Azure Portal → App Service → Configuration :

```
Jwt__Key = "VotreCléTrèsSécuriséePourProduction2025_MinimumTrenteDeuxCaractères!"
```

### 2. Sécuriser la base de données

- Activez SSL pour MySQL si possible
- Changez le mot de passe de production
- Utilisez Azure Key Vault pour stocker les secrets (optionnel, non gratuit)

### 3. Variables d'environnement

⚠️ **Ne commitez JAMAIS** `appsettings.Production.json` avec des secrets sur Git !
Utilisez les Application Settings d'Azure à la place.

---

## 🛠️ Dépannage

### L'API ne répond pas (500 Internal Server Error)

1. Vérifiez les logs :
   - Azure Portal → App Service → **Log stream**
   - Ou téléchargez les logs : **Advanced Tools (Kudu)** → Debug Console

2. Vérifiez la connexion MySQL :
   ```powershell
   # Testez depuis Azure Cloud Shell
   telnet fga.sitsci.com 3306
   ```

3. Vérifiez les variables d'environnement dans **Configuration**

### Le frontend ne peut pas appeler l'API (CORS)

1. Vérifiez que CORS est configuré correctement dans App Service
2. Vérifiez l'URL de l'API dans `.env.production`
3. Ouvrez la console du navigateur pour voir les erreurs CORS

### L'application "dort" (démarrage lent)

C'est normal avec le plan gratuit F1. Solutions :
- Utilisez Azure Functions avec un timer pour "réveiller" l'app toutes les 15 minutes
- Passez au plan B1 Basic (≈13€/mois) pour éviter le sommeil

---

## 🔄 Mises à Jour Futures

### Backend

**Via Visual Studio** :
1. Clic droit sur le projet → **Publish**
2. Cliquez sur **Publish** pour déployer la nouvelle version

**Via CLI** :
```powershell
dotnet publish -c Release -o ./publish
cd publish
Compress-Archive -Path * -DestinationPath ../deploy.zip -Force
az webapp deployment source config-zip --resource-group pontpesee-rg --name pontpesee-api --src ../deploy.zip
```

### Frontend

1. Committez et poussez sur GitHub
2. GitHub Actions déploiera automatiquement
3. Ou utilisez VS Code → Azure extension → Deploy

---

## 📊 Monitoring

### Surveiller l'utilisation

1. Azure Portal → App Service → **Metrics**
2. Surveillez :
   - CPU time (max 60 min/jour gratuit)
   - Memory working set
   - HTTP requests

### Alertes

Configurez des alertes si vous approchez des limites :
- Azure Portal → App Service → **Alerts** → New alert rule

---

## ✅ Checklist de Déploiement

### Backend
- [ ] Compte Azure créé
- [ ] App Service créé (Plan F1 gratuit)
- [ ] Application publiée
- [ ] Variables d'environnement configurées
- [ ] Connexion MySQL testée
- [ ] CORS configuré
- [ ] API testée : `https://pontpesee-api.azurewebsites.net/swagger`

### Frontend
- [ ] Static Web App créée
- [ ] `.env.production` configuré avec URL de l'API
- [ ] Build réussi
- [ ] Déploiement effectué
- [ ] Application accessible : `https://pontpesee-frontend.azurestaticapps.net`
- [ ] Connexion à l'API testée

### Post-Déploiement
- [ ] Clé JWT changée pour production
- [ ] CORS mis à jour avec l'URL réelle du frontend
- [ ] Logs activés pour monitoring
- [ ] Application complète testée (login, CRUD, rapports)

---

## 🎓 Ressources Utiles

- **Documentation Azure App Service** : https://docs.microsoft.com/azure/app-service/
- **Azure Static Web Apps** : https://docs.microsoft.com/azure/static-web-apps/
- **Azure Free Account** : https://azure.microsoft.com/free/
- **Tutoriel ASP.NET Core sur Azure** : https://docs.microsoft.com/aspnet/core/host-and-deploy/azure-apps/

---

## 💡 Prochaines Étapes

1. Créez votre compte Azure (si pas encore fait)
2. Déployez le backend via Visual Studio
3. Notez l'URL de l'API : `https://pontpesee-api.azurewebsites.net`
4. Configurez et déployez le frontend
5. Testez l'application complète
6. Configurez un domaine personnalisé (optionnel)

Besoin d'aide à une étape particulière ? Demandez-moi !
