# Guide de Déploiement - PontPesee.API sur Windows Server + IIS

## Prérequis sur le Serveur

### 1. Installer .NET 8.0 Hosting Bundle
- Téléchargez depuis : https://dotnet.microsoft.com/download/dotnet/8.0
- Installez le **ASP.NET Core Runtime 8.0 Hosting Bundle**
- Redémarrez le serveur après l'installation

### 2. Vérifier IIS
- IIS doit être installé et démarré
- Le module ASP.NET Core doit être installé (inclus dans le Hosting Bundle)

### 3. MySQL
- Assurez-vous que MySQL est accessible depuis le serveur
- Vérifiez la connexion : `Server=fga.sitsci.com;Port=3306`

---

## Étapes de Déploiement

### Étape 1 : Publier l'Application (sur votre machine de développement)

```powershell
cd "D:\PROJETVBNET2015\PONT VBPP2 CAM2ADMIN\PONT_WEBAPP\backend\PontPesee.API"
dotnet publish -c Release -o ./publish --self-contained false
```

Les fichiers seront générés dans le dossier `./publish`

### Étape 2 : Préparer le Serveur

1. **Créer le répertoire de déploiement** (sur le serveur)
   ```
   C:\inetpub\wwwroot\PontPeseeAPI
   ```

2. **Créer le dossier logs**
   ```
   C:\inetpub\wwwroot\PontPeseeAPI\logs
   ```

3. **Créer le dossier generated** (pour les rapports)
   ```
   C:\inetpub\wwwroot\PontPeseeAPI\generated
   ```

### Étape 3 : Copier les Fichiers

1. Copiez tout le contenu du dossier `publish` vers `C:\inetpub\wwwroot\PontPeseeAPI`
2. Les fichiers doivent inclure :
   - `PontPesee.API.dll`
   - `web.config`
   - `appsettings.json`
   - `appsettings.Production.json`
   - Toutes les dépendances (.dll, .json, etc.)

### Étape 4 : Configurer appsettings.Production.json

⚠️ **IMPORTANT** : Avant le déploiement, modifiez `appsettings.Production.json` :

1. **CORS - Domaines autorisés** (ligne 16-19)
   ```json
   "AllowedOrigins": [
     "https://votre-domaine-production.com",
     "http://votre-domaine-production.com"
   ]
   ```
   Remplacez par l'URL de votre frontend en production

2. **JWT Key** (ligne 13)
   ```json
   "Key": "VotreCleSecretePourJWT_MinimumTrenteDeuxCaracteres2025"
   ```
   ⚠️ Changez cette clé pour une clé unique et sécurisée en production !

3. **Chaîne de connexion** (si nécessaire)
   Vérifiez que la connexion MySQL est correcte pour le serveur de production

### Étape 5 : Configurer IIS

1. **Ouvrir IIS Manager**

2. **Créer un nouveau Pool d'Applications**
   - Nom : `PontPeseeAPIPool`
   - .NET CLR Version : **No Managed Code**
   - Pipeline Mode : Integrated
   - Start Mode : AlwaysRunning (optionnel, pour améliorer les performances)

3. **Créer un nouveau Site Web**
   - Nom du site : `PontPesee.API`
   - Pool d'applications : `PontPeseeAPIPool`
   - Chemin physique : `C:\inetpub\wwwroot\PontPeseeAPI`
   - Binding :
     - Type : http
     - Port : `5000` (ou votre port choisi)
     - Nom d'hôte : (optionnel)

4. **Permissions du dossier**
   - Clic droit sur `C:\inetpub\wwwroot\PontPeseeAPI`
   - Propriétés → Sécurité → Modifier
   - Ajouter `IIS AppPool\PontPeseeAPIPool`
   - Permissions : **Lecture & Exécution**, **Lecture**, **Écriture** (pour les dossiers logs et generated)

### Étape 6 : Configuration HTTPS (Recommandé pour Production)

1. **Obtenir un certificat SSL**
   - Via Let's Encrypt (gratuit)
   - Via votre autorité de certification

2. **Dans IIS Manager**
   - Sélectionnez votre site
   - Bindings → Add
   - Type : https
   - Port : 443
   - SSL Certificate : Sélectionnez votre certificat

3. **Redirection HTTP vers HTTPS** (optionnel)
   - Installez le module URL Rewrite
   - Configurez la redirection automatique

### Étape 7 : Démarrer l'Application

1. Dans IIS Manager, sélectionnez votre site
2. Cliquez sur **Start** dans le panneau Actions
3. Testez l'accès : `http://votre-serveur:5000` ou `https://votre-serveur`

---

## Vérification du Déploiement

### Test de l'API
```powershell
# Test simple
Invoke-WebRequest -Uri "http://votre-serveur:5000" -UseBasicParsing

# Si Swagger est activé
# Accédez à : http://votre-serveur:5000/swagger
```

### Vérifier les Logs
Les logs sont stockés dans : `C:\inetpub\wwwroot\PontPeseeAPI\logs\`

Si l'application ne démarre pas, consultez les logs stdout pour identifier le problème.

---

## Dépannage

### L'application ne démarre pas

1. **Vérifier les logs**
   ```
   C:\inetpub\wwwroot\PontPeseeAPI\logs\stdout_*.log
   ```

2. **Vérifier le Event Viewer Windows**
   - Windows Logs → Application
   - Recherchez les erreurs liées à ASP.NET Core

3. **Vérifier que le Hosting Bundle est installé**
   ```powershell
   dotnet --list-runtimes
   ```
   Vous devriez voir : `Microsoft.AspNetCore.App 8.0.x`

4. **Permissions insuffisantes**
   - Vérifiez que `IIS AppPool\PontPeseeAPIPool` a accès au dossier

### Erreur 500.30 - ASP.NET Core app failed to start

- Le Hosting Bundle n'est pas installé
- Le fichier `web.config` est incorrect
- Les dépendances .NET ne sont pas installées

### Erreur de connexion à MySQL

- Vérifiez la chaîne de connexion dans `appsettings.Production.json`
- Vérifiez que le serveur peut accéder à `fga.sitsci.com:3306`
- Testez avec : `Test-NetConnection -ComputerName fga.sitsci.com -Port 3306`

### Erreur CORS

- Mettez à jour `AllowedOrigins` dans `appsettings.Production.json`
- Redémarrez le site IIS après modification

---

## Mises à Jour Futures

Pour déployer une nouvelle version :

1. Arrêtez le site IIS
2. Sauvegardez `appsettings.Production.json`
3. Supprimez les anciens fichiers (sauf `appsettings.Production.json`, `logs`, `generated`)
4. Copiez les nouveaux fichiers depuis `publish`
5. Restaurez `appsettings.Production.json`
6. Redémarrez le site IIS

---

## Checklist de Déploiement

- [ ] .NET 8.0 Hosting Bundle installé sur le serveur
- [ ] Dossier `C:\inetpub\wwwroot\PontPeseeAPI` créé
- [ ] Dossiers `logs` et `generated` créés avec permissions d'écriture
- [ ] Fichiers publiés copiés sur le serveur
- [ ] `appsettings.Production.json` configuré (CORS, JWT Key)
- [ ] Pool d'applications IIS créé (No Managed Code)
- [ ] Site IIS créé et démarré
- [ ] Permissions du dossier configurées pour `IIS AppPool\PontPeseeAPIPool`
- [ ] Connexion MySQL testée depuis le serveur
- [ ] Certificat SSL installé (pour HTTPS)
- [ ] Application testée et fonctionnelle

---

## Support

Pour toute question ou problème de déploiement, consultez :
- Logs de l'application : `C:\inetpub\wwwroot\PontPeseeAPI\logs\`
- Event Viewer Windows
- Documentation Microsoft : https://docs.microsoft.com/aspnet/core/host-and-deploy/iis/
