# 🧪 TEST IMMÉDIAT - Diagnostic Complet

## ✅ Corrections Appliquées

1. ✅ **HTTPS redirection désactivée** en développement ([Program.cs:127-131](backend/PontPesee.API/Program.cs#L127-131))
2. ✅ **Logs JWT détaillés** avec couleurs ([Program.cs:44-87](backend/PontPesee.API/Program.cs#L44-87))
3. ✅ **Affichage de la config JWT** au démarrage ([Program.cs:21-26](backend/PontPesee.API/Program.cs#L21-26))

---

## 🚀 ÉTAPE 1 : Redémarrer le Backend

```bash
cd D:\PROJETVBNET2015\PONT VBPP2 CAM2ADMIN\PONT_WEBAPP\backend\PontPesee.API
dotnet run
```

### Vous devriez voir au démarrage :

```
=== JWT Configuration ===
JWT Key Length: 58 caractères
JWT Issuer: PontPeseeAPI
JWT Audience: PontPeseeClient
========================
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5059
```

⚠️ **IMPORTANT** : Vérifiez que "JWT Key Length: 58 caractères" s'affiche !

---

## 🧪 ÉTAPE 2 : Tester avec Swagger

### 2.1 Ouvrir Swagger
- Allez sur : http://localhost:5059
- Vous devriez voir la page Swagger UI

### 2.2 Se connecter (Login)
1. Cliquez sur **POST /api/Auth/login**
2. Cliquez sur **Try it out**
3. Entrez :
   ```json
   {
     "username": "ADMIN",
     "password": "votre_mot_de_passe"
   }
   ```
4. Cliquez sur **Execute**

**Résultat attendu** :
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "ADMIN",
  "profil": "ADMINISTRATEUR",
  "expiresAt": "2026-01-10T01:09:38Z"
}
```

5. **COPIEZ LE TOKEN** (toute la chaîne eyJhbG...)

### 2.3 Autoriser avec le token
1. Cliquez sur le bouton **Authorize** (🔓 en haut à droite)
2. Dans la popup, entrez : `Bearer <COLLEZ_VOTRE_TOKEN_ICI>`

   Exemple :
   ```
   Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiQURNSU4iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBRE1JTklTVFJBVEVVUiIsIlByb2ZpbCI6IkFETUlOSVNUUkFURVVSIiwiZXhwIjoxNzY4MDA5MTc4LCJpc3MiOiJQb250UGVzZWVBUEkiLCJhdWQiOiJQb250UGVzZWVDbGllbnQifQ.gIV4M6LHQvd5KhcPdY6BPcTwvPGXzGx2eC4SsE-qG4MZQ
   ```

3. Cliquez sur **Authorize** puis **Close**

### 2.4 Tester un endpoint protégé
1. Cliquez sur **POST /api/Pesee/search**
2. Cliquez sur **Try it out**
3. Entrez :
   ```json
   {
     "tableName": "pesee",
     "dateDebut": "2026-01-09T17:27:07.1972",
     "dateFin": "2026-01-09T17:27:07.1972",
     "mouvement": "string",
     "fournisseur": "string",
     "client": "string",
     "destination": "string",
     "provenance": "string",
     "produit": "string",
     "vehicule": "string",
     "chauffeur": "string",
     "codeSite": "string",
     "acceptes": true,
     "annules": true,
     "tous": true,
     "page": 0,
     "pageSize": 0
   }
   ```
4. Cliquez sur **Execute**

---

## 📊 ÉTAPE 3 : Observer les Logs Backend

### Dans la console du backend, vous devriez voir :

#### ✅ Si ça marche (200 OK) :
```
📨 JWT Token received: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi...
✅ JWT Token validated successfully
   User: ADMIN
```

#### ❌ Si erreur 401 :
```
📨 JWT Token received: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi...
❌ JWT Authentication FAILED
   Error: IDX10503: Signature validation failed...
   Exception Type: SecurityTokenInvalidSignatureException
⚠️  JWT Challenge triggered
   Error: invalid_token
   Error Description: The signature key was not found
```

---

## 🔍 ÉTAPE 4 : Analyser les Résultats

### Scénario A : ✅ Succès (200 OK)
**Bravo !** Le backend fonctionne parfaitement. Vous verrez :
- Dans Swagger : Code 200, avec des données JSON
- Dans la console : ✅ JWT Token validated successfully

**Prochaine étape** : Créer les pages Vue.js du frontend

---

### Scénario B : ❌ Erreur 401 - Signature validation failed

**Diagnostic** : Le token est généré avec une clé, mais validé avec une autre.

**Causes possibles** :
1. Le fichier `appsettings.json` n'est pas chargé correctement
2. Vous avez plusieurs fichiers `appsettings.json` (Development, Production)
3. Le backend utilise un autre fichier de config

**Solution** :

#### 1. Vérifiez les logs au démarrage du backend

Vous devez voir :
```
=== JWT Configuration ===
JWT Key Length: 58 caractères
JWT Issuer: PontPeseeAPI
JWT Audience: PontPeseeClient
========================
```

Si vous voyez :
- **JWT Key Length: 54 caractères** → Le backend utilise la clé par défaut "DefaultSecretKeyForPontPeseeApplication2025"
- **JWT Key Length: 58 caractères** → Le backend utilise la bonne clé "VotreCleSecretePourJWT_MinimumTrenteDeuxCaracteres2025"

#### 2. Si la longueur ne correspond pas (≠ 58)

Vérifiez qu'il n'y a pas de fichier `appsettings.Development.json` :

```bash
dir backend\PontPesee.API\appsettings*.json
```

Si vous voyez plusieurs fichiers, vérifiez leur contenu :
```bash
type backend\PontPesee.API\appsettings.Development.json
```

#### 3. Solution de contournement : Hardcoder la clé temporairement

Si le problème persiste, modifiez temporairement [AuthService.cs:48](backend/PontPesee.API/Services/AuthService.cs#L48) :

**AVANT** :
```csharp
Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "DefaultSecretKeyForPontPeseeApplication2025"));
```

**APRÈS** :
```csharp
Encoding.UTF8.GetBytes("VotreCleSecretePourJWT_MinimumTrenteDeuxCaracteres2025"));
```

⚠️ C'est temporaire pour tester !

---

### Scénario C : ❌ Erreur 401 - Audience validation failed

**Message d'erreur dans la console** :
```
❌ JWT Authentication FAILED
   Error: IDX10214: Audience validation failed. Audiences: 'PontPeseeClient'...
```

**Solution** : Vérifiez que [appsettings.json:15](backend/PontPesee.API/appsettings.json#L15) contient :
```json
"Audience": "PontPeseeClient"
```

---

### Scénario D : ❌ Erreur 401 - Issuer validation failed

**Message d'erreur dans la console** :
```
❌ JWT Authentication FAILED
   Error: IDX10205: Issuer validation failed. Issuer: 'PontPeseeAPI'...
```

**Solution** : Vérifiez que [appsettings.json:14](backend/PontPesee.API/appsettings.json#L14) contient :
```json
"Issuer": "PontPeseeAPI"
```

---

### Scénario E : ❌ Erreur 401 - Token expired

**Message d'erreur dans la console** :
```
❌ JWT Authentication FAILED
   Error: IDX10223: Lifetime validation failed. The token is expired...
```

**Solution** : Régénérez un nouveau token en refaisant le login (POST /api/Auth/login)

---

## 📋 CHECKLIST DE TEST

- [ ] Backend redémarré avec `dotnet run`
- [ ] Logs de démarrage affichent "JWT Key Length: 58 caractères"
- [ ] Swagger accessible sur http://localhost:5059
- [ ] Login réussi (POST /api/Auth/login) → Token reçu
- [ ] Token copié et collé dans Authorize
- [ ] Test de /api/Pesee/search exécuté
- [ ] Résultat observé (200 ou 401)
- [ ] Logs backend consultés et copiés

---

## 📤 ENVOYEZ-MOI

### 1. Screenshot du résultat dans Swagger
- Après avoir cliqué sur Execute pour /api/Pesee/search
- Montrant le code de réponse (200 ou 401)

### 2. Copie COMPLÈTE des logs de la console backend

Depuis le démarrage jusqu'à l'erreur :

```
[COLLEZ ICI TOUS LES LOGS]

Incluant :
- === JWT Configuration ===
- JWT Key Length: XX caractères
- 📨 JWT Token received...
- ✅ ou ❌ messages
```

### 3. Confirmation de la longueur de la clé

```
JWT Key Length affiché au démarrage : XX caractères
```

---

## 🎯 OBJECTIF

**Une fois que le test /api/Pesee/search retourne 200 OK** avec des données, le backend sera 100% fonctionnel et nous pourrons passer au frontend Vue.js !

---

**🚀 MAINTENANT : Redémarrez le backend et testez dans Swagger !**
