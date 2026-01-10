# 🔄 REDÉMARRER LE BACKEND - Diagnostic Amélioré

## 🎯 Problème Identifié

Dans votre screenshot, on voit :
```
JWT Key Length: 54 caractères
```

**54 caractères** = Clé par défaut (fallback)
**58 caractères** = Bonne clé depuis appsettings.json

## ✅ Corrections Appliquées

### 1. [Program.cs:17-30](backend/PontPesee.API/Program.cs#L17-30)
- Ajout de logs pour voir si la clé est NULL
- Affichage des 20 premiers caractères de la clé

### 2. [AuthService.cs:47-57](backend/PontPesee.API/Services/AuthService.cs#L47-57)
- Ajout de logs dans GenerateJwtToken
- Exception si la clé est NULL (au lieu du fallback)
- Plus de fallback silencieux !

---

## 🚀 ACTIONS

### 1. Arrêter le backend actuel
Dans le PowerShell, appuyez sur **Ctrl+C**

### 2. Nettoyer et rebuild
```powershell
cd D:\PROJETVBNET2015\PONT' VBPP2 CAM2ADMIN'\PONT_WEBAPP\backend\PontPesee.API
dotnet clean
dotnet build
```

### 3. Redémarrer le backend
```powershell
dotnet run
```

---

## 📊 Logs Attendus

### Au démarrage, vous devriez voir :

```
DEBUG: Jwt:Key from config = VotreCleSecretePourJWT_MinimumTrenteDeuxCaracteres2025
=== JWT Configuration ===
JWT Key Length: 58 caractères
JWT Key Value: VotreCleSecretePour...
JWT Issuer: PontPeseeAPI
JWT Audience: PontPeseeClient
========================
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5059
```

⚠️ **Si vous voyez** :
```
DEBUG: Jwt:Key from config = NULL
```
→ Le problème est dans le chargement de appsettings.json

---

### Lors du login (POST /api/Auth/login), vous verrez :

```
[AuthService] JWT Key from config: VotreCleSecretePourJWT_MinimumTrenteDeuxCaracteres2025
[AuthService] JWT Key Length: 58 caractères
```

⚠️ **Si vous voyez** :
```
[AuthService] JWT Key from config: NULL
Unhandled exception: JWT Key not found in configuration in AuthService!
```
→ Le IConfiguration n'est pas injecté correctement dans AuthService

---

### Lors de l'appel à /api/Pesee/search, vous verrez :

#### ✅ Succès (200 OK) :
```
📨 JWT Token received: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...
✅ JWT Token validated successfully
   User: ADMIN
```

#### ❌ Échec (401) :
```
📨 JWT Token received: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...
❌ JWT Authentication FAILED
   Error: IDX10503: Signature validation failed. Keys tried: 'Microsoft.IdentityModel.Tokens.SymmetricSecurityKey'
   Exception Type: SecurityTokenInvalidSignatureException
⚠️  JWT Challenge triggered
   Error: invalid_token
   Error Description: The signature key was not found
```

---

## 🔍 Scénarios de Diagnostic

### Scénario A : Program.cs charge la clé (58 chars) MAIS AuthService dit NULL

**Cause** : IConfiguration n'est pas injecté correctement dans AuthService

**Solution** : Vérifier l'enregistrement du service dans Program.cs ligne 71-72

### Scénario B : Program.cs dit NULL

**Cause** : appsettings.json n'est pas chargé

**Solutions possibles** :
1. Le fichier appsettings.json a une erreur de syntaxe JSON
2. Le fichier n'est pas copié dans bin/Debug/net8.0/
3. Problème de casse (Windows est case-insensitive mais .NET peut être strict)

**Test** : Vérifier manuellement
```powershell
type backend\PontPesee.API\bin\Debug\net8.0\appsettings.json
```

### Scénario C : Les deux chargent la clé (58 chars) mais 401 persiste

**Cause** : Autre problème (Issuer, Audience, expiration)

**Solution** : Regarder le message d'erreur exact dans les logs rouges ❌

---

## 🧪 Test Complet

### 1. Redémarrer le backend
```powershell
cd D:\PROJETVBNET2015\PONT' VBPP2 CAM2ADMIN'\PONT_WEBAPP\backend\PontPesee.API
dotnet clean
dotnet build
dotnet run
```

### 2. Ouvrir Swagger
http://localhost:5059

### 3. Login (POST /api/Auth/login)
```json
{
  "username": "ADMIN",
  "password": "votre_mot_de_passe"
}
```

Copiez le token reçu.

### 4. Authorize
Cliquez sur **Authorize** (🔓)
Entrez : `Bearer <TOKEN_COPIÉ>`

### 5. Test Pesee Search (POST /api/Pesee/search)
```json
{
  "tableName": "pesee",
  "dateDebut": "2026-01-01",
  "dateFin": "2026-01-31",
  "tous": true,
  "page": 0,
  "pageSize": 10
}
```

---

## 📤 ENVOYEZ-MOI

1. **Screenshot du PowerShell au démarrage** montrant les logs JWT Configuration
2. **Screenshot du PowerShell lors du login** montrant [AuthService] logs
3. **Screenshot du PowerShell lors de /api/Pesee/search** montrant ✅ ou ❌
4. **Screenshot du résultat dans Swagger** (200 ou 401)

---

## 🎯 OBJECTIF

Identifier pourquoi la clé JWT n'est pas chargée correctement et pourquoi on voit "54 caractères" au lieu de "58 caractères".

---

**🚀 MAINTENANT : Redémarrez avec `dotnet clean && dotnet build && dotnet run`**
