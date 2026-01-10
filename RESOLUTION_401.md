# 🔧 Résolution de l'erreur 401 (Unauthorized)

## ✅ Corrections apportées

J'ai ajouté plusieurs améliorations pour diagnostiquer et corriger l'erreur 401 :

### 1. **Logs de debug JWT** (dans Program.cs)
- ✅ Affiche quand un token est reçu
- ✅ Affiche si le token est validé avec succès
- ✅ Affiche les erreurs de validation

### 2. **Endpoint de test** (HealthController.cs)
- ✅ `/api/Health/ping` - Test SANS authentification
- ✅ `/api/Health/secure-ping` - Test AVEC authentification

### 3. **Page de test HTML** (test-api.html)
- ✅ Interface simple pour tester l'API
- ✅ Pas besoin de Postman

---

## 🚀 Procédure de test

### Étape 1 : Redémarrer le backend

```bash
# Arrêtez le backend (Ctrl+C)
cd D:\PROJETVBNET2015\PONT VBPP2 CAM2ADMIN\PONT_WEBAPP\backend\PontPesee.API
dotnet run
```

### Étape 2 : Ouvrir la page de test

Double-cliquez sur : `D:\PROJETVBNET2015\PONT VBPP2 CAM2ADMIN\PONT_WEBAPP\test-api.html`

Ou ouvrez-la dans votre navigateur.

### Étape 3 : Suivre les tests dans l'ordre

1. **Test 1** : Cliquez sur "Tester /api/Health/ping"
   - ✅ Doit retourner `status: "OK"`
   - ❌ Si erreur → Le backend ne fonctionne pas

2. **Test 2** : Entrez username/password et cliquez "Se connecter"
   - ✅ Doit retourner un token
   - ❌ Si erreur → Vérifiez que l'utilisateur existe dans MySQL

3. **Test 3** : Cliquez sur "Tester /api/Health/secure-ping"
   - ✅ Doit retourner `You are authenticated`
   - ❌ Si erreur 401 → Problème de token JWT

4. **Test 4** : Cliquez sur "Rechercher" (pesées)
   - ✅ Doit retourner les données
   - ❌ Si erreur 401 → Même problème que Test 3

---

## 🔍 Diagnostic dans la console

Quand vous testez, regardez la **console du backend** (où `dotnet run` est lancé).

### ✅ Si tout fonctionne bien, vous verrez :

```
JWT Token received: eyJhbGciOiJIUzI1Ni...
JWT Token validated successfully
```

### ❌ Si erreur de validation :

```
JWT Authentication failed: The signature is invalid
```

→ **Cause** : La clé JWT n'est pas la même entre la génération et la validation

**Solution** :
1. Vérifiez `appsettings.json` ligne 13
2. La clé doit faire au moins 32 caractères
3. Redémarrez le backend après modification

### ❌ Si erreur "The token is expired" :

```
JWT Authentication failed: The token is expired
```

→ **Cause** : Le token a plus de 8 heures

**Solution** :
1. Reconnectez-vous pour obtenir un nouveau token
2. Ou augmentez la durée dans `AuthService.cs` ligne 46

---

## 🛠️ Vérifications à faire

### 1. Vérifier que l'utilisateur existe

```sql
-- Connectez-vous à MySQL
mysql -h fga.sitsci.com -u pontuser -p

-- Vérifiez la table user
USE gestpeseedb;
SELECT * FROM user;
```

Si vide ou pas d'utilisateur :

```sql
INSERT INTO user (NomUt, motpasse, LeGroupe, Actif)
VALUES ('admin', 'admin123', 'SUPERVISEUR', 1);
```

### 2. Vérifier appsettings.json

Le fichier doit contenir :

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=fga.sitsci.com;Port=3306;Database=gestpeseedb;User=pontuser;Password=Soprol@2022;connect timeout=60;SslMode=none;"
  },
  "Jwt": {
    "Key": "VotreCleSecretePourJWT_MinimumTrenteDeuxCaracteres2025",
    "Issuer": "PontPeseeAPI",
    "Audience": "PontPeseeClient"
  }
}
```

⚠️ **Important** : La clé JWT doit faire au moins 32 caractères !

### 3. Vérifier que CORS est bien configuré

Si vous testez depuis `http://localhost:5173`, vérifiez que cette URL est dans `appsettings.json` :

```json
"Cors": {
  "AllowedOrigins": [
    "http://localhost:5173"
  ]
}
```

---

## 🧪 Test avec Swagger

### Alternative : Utiliser Swagger directement

1. Ouvrez : `http://localhost:5000`
2. Testez `/api/Auth/login` :
   - Cliquez sur "Try it out"
   - Entrez username/password
   - Cliquez "Execute"
   - **Copiez le token** (tout le texte après `"token": "`)

3. Cliquez sur **Authorize** 🔓 en haut à droite
4. Collez le token (SANS le mot "Bearer")
5. Cliquez "Authorize" puis "Close"

6. Maintenant testez `/api/Pesee/search` :
   - Cliquez "Try it out"
   - Entrez le JSON :
   ```json
   {
     "tableName": "pesee",
     "acceptes": true,
     "page": 1,
     "pageSize": 10
   }
   ```
   - Cliquez "Execute"

---

## 🔧 Solutions courantes

### Problème 1 : Token non envoyé

**Symptôme** : Erreur 401 immédiatement

**Cause** : Le header `Authorization` n'est pas envoyé

**Solution** :
- Dans Swagger : Utilisez le bouton "Authorize"
- Dans Postman : Onglet "Authorization" → "Bearer Token"
- Dans le code : Vérifiez l'intercepteur Axios

### Problème 2 : Token invalide

**Symptôme** : Erreur "The signature is invalid"

**Cause** : La clé JWT est différente entre login et validation

**Solution** :
1. Arrêtez le backend
2. Vérifiez `appsettings.json`
3. Redémarrez le backend
4. Reconnectez-vous pour obtenir un nouveau token

### Problème 3 : Token expiré

**Symptôme** : Erreur "The token is expired"

**Cause** : Le token a plus de 8 heures

**Solution** :
- Reconnectez-vous pour obtenir un nouveau token

### Problème 4 : CORS

**Symptôme** : Erreur dans la console du navigateur : "CORS policy"

**Cause** : L'origine du frontend n'est pas autorisée

**Solution** :
1. Ajoutez votre URL frontend dans `appsettings.json`
2. Redémarrez le backend

---

## 📊 Checklist de diagnostic

Cochez au fur et à mesure :

- [ ] Backend démarre sans erreur
- [ ] Swagger accessible sur http://localhost:5000
- [ ] `/api/Health/ping` retourne OK (sans auth)
- [ ] Un utilisateur existe dans la table `user`
- [ ] `/api/Auth/login` retourne un token
- [ ] Le token fait plus de 100 caractères
- [ ] Dans la console backend, on voit "JWT Token received"
- [ ] Dans la console backend, on voit "JWT Token validated successfully"
- [ ] `/api/Health/secure-ping` retourne OK avec le token
- [ ] `/api/Pesee/search` retourne des données avec le token

---

## 🎯 Test final

Si tous les tests passent dans `test-api.html` :

1. ✅ Ping fonctionne
2. ✅ Login retourne un token
3. ✅ Secure-ping avec token fonctionne
4. ✅ Search avec token retourne des données

→ **L'API est 100% fonctionnelle !**

Le problème vient alors du frontend (Vue.js).

---

## 🔄 Si le problème persiste

### Option 1 : Désactiver temporairement l'authentification

Dans `PeseeController.cs`, commentez `[Authorize]` :

```csharp
[Route("api/[controller]")]
[ApiController]
// [Authorize]  ← Commenté temporairement
public class PeseeController : ControllerBase
```

Redémarrez et testez. Si ça marche → Le problème est bien dans JWT.

### Option 2 : Logs détaillés

Dans `appsettings.json`, activez les logs détaillés :

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Debug",
      "Microsoft.AspNetCore.Authentication": "Debug"
    }
  }
}
```

---

## 📞 Informations pour le support

Si vous avez besoin d'aide, fournissez :

1. Le message exact dans la console backend
2. Le résultat des 4 tests dans `test-api.html`
3. La réponse de `/api/Auth/login`
4. Le premier caractère du token (pour vérifier le format)

---

**Bon courage ! 🚀**
