# 🧪 Guide de Test de l'API

## Problème actuel : Erreur 401 sur les endpoints protégés

### Diagnostic

L'erreur 401 (Unauthorized) signifie que le token JWT n'est pas correctement envoyé ou validé.

---

## 🔧 Étape 1 : Tester l'API sans authentification

### Test 1 : Endpoint de santé

**URL**: `GET http://localhost:5000/api/Health/ping`

**Résultat attendu** :
```json
{
  "status": "OK",
  "message": "API is running",
  "timestamp": "2025-01-09T..."
}
```

✅ Si ça marche → L'API fonctionne, le problème est bien l'authentification

---

## 🔐 Étape 2 : Tester l'authentification

### Test 2 : Login

**URL**: `POST http://localhost:5000/api/Auth/login`

**Headers**:
```
Content-Type: application/json
```

**Body** (vérifiez que cet utilisateur existe dans votre table `user`) :
```json
{
  "username": "admin",
  "password": "admin123"
}
```

**Résultat attendu** :
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "admin",
  "profil": "SUPERVISEUR",
  "expiresAt": "2025-01-09T20:00:00Z"
}
```

📋 **COPIEZ LE TOKEN** (tout le texte après `"token": "...`)

---

## 🔒 Étape 3 : Tester avec le token

### Test 3 : Endpoint protégé de test

**URL**: `GET http://localhost:5000/api/Health/secure-ping`

**Headers** (IMPORTANT) :
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

⚠️ **ATTENTION** :
- Il doit y avoir un ESPACE entre "Bearer" et le token
- Le token doit être complet (très long texte)

**Résultat attendu** :
```json
{
  "status": "OK",
  "message": "You are authenticated",
  "username": "admin",
  "roles": ["SUPERVISEUR"],
  "timestamp": "2025-01-09T..."
}
```

---

## 🧪 Étape 4 : Tester l'endpoint Pesee

### Test 4 : Recherche de pesées

**URL**: `POST http://localhost:5000/api/Pesee/search`

**Headers**:
```
Content-Type: application/json
Authorization: Bearer VOTRE_TOKEN_ICI
```

**Body**:
```json
{
  "tableName": "pesee",
  "dateDebut": "2025-01-01",
  "dateFin": "2025-01-10",
  "acceptes": true,
  "page": 1,
  "pageSize": 10
}
```

**Résultat attendu** :
```json
{
  "nombrePesees": 150,
  "poidsBrutTotal": 50000.5,
  "poidsNetTotal": 48500.2,
  "pesees": [...],
  "totalPages": 15,
  "currentPage": 1
}
```

---

## 🔍 Tests dans Swagger

### Comment utiliser Swagger (plus facile)

1. Ouvrez : `http://localhost:5000`
2. Cliquez sur `/api/Auth/login` → "Try it out"
3. Entrez vos credentials et cliquez "Execute"
4. **Copiez le token** dans la réponse
5. En haut à droite, cliquez sur le bouton **"Authorize"** 🔓
6. Dans la popup, collez le token (SANS le mot "Bearer")
7. Cliquez "Authorize" puis "Close"
8. Maintenant tous les endpoints protégés devraient fonctionner

---

## ❌ Solutions aux erreurs courantes

### Erreur : 401 Unauthorized

**Causes possibles** :

1. **Token non envoyé**
   ```
   ❌ Mauvais: GET /api/Pesee/search
   ✅ Correct: GET /api/Pesee/search
                Headers: Authorization: Bearer eyJ...
   ```

2. **Format du header incorrect**
   ```
   ❌ Mauvais: Authorization: eyJ...
   ❌ Mauvais: Bearer eyJ...
   ✅ Correct: Authorization: Bearer eyJ...
   ```

3. **Token expiré**
   - Reconnectez-vous pour obtenir un nouveau token

4. **Token invalide**
   - Vérifiez que vous avez copié le token complet
   - Pas d'espaces avant/après

### Erreur : 500 Internal Server Error

**Causes possibles** :

1. **Problème de connexion MySQL**
   - Vérifiez `appsettings.json`
   - Testez la connexion MySQL directement

2. **Table n'existe pas**
   - Vérifiez que les tables `pesee` et/ou `peseep2` existent
   - Vérifiez la structure des tables

---

## 🐛 Debug : Vérifier les logs

### Dans la console où vous avez lancé `dotnet run`, vous devriez voir :

```
JWT Token received: eyJhbGciOiJIUzI1Ni...
JWT Token validated successfully
```

Si vous voyez :
```
JWT Authentication failed: ...
```

→ Le problème est dans la validation du token

---

## 🔧 Solution si ça ne marche toujours pas

### Vérifier la table `user`

Assurez-vous qu'un utilisateur existe :

```sql
SELECT * FROM user;
```

Si vide, créez un utilisateur :

```sql
INSERT INTO user (NomUt, motpasse, LeGroupe, Actif)
VALUES ('admin', 'admin123', 'SUPERVISEUR', 1);
```

### Tester avec Postman

1. Téléchargez [Postman](https://www.postman.com/downloads/)
2. Créez une collection "Pont Pesee API"
3. Ajoutez les requêtes ci-dessus
4. Dans l'onglet "Authorization", choisissez "Bearer Token"
5. Collez votre token

---

## 📞 Checklist de diagnostic

- [ ] L'API démarre sans erreur (`dotnet run`)
- [ ] Swagger accessible sur `http://localhost:5000`
- [ ] `/api/Health/ping` retourne OK (sans auth)
- [ ] `/api/Auth/login` retourne un token
- [ ] Le token est bien copié (texte très long)
- [ ] Header `Authorization: Bearer TOKEN` est bien formaté
- [ ] `/api/Health/secure-ping` retourne OK (avec token)
- [ ] `/api/Pesee/search` retourne les données (avec token)

---

## 🎯 Test rapide avec cURL

### Linux/Mac/Git Bash :

```bash
# 1. Login
curl -X POST http://localhost:5000/api/Auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"admin123"}'

# Copiez le token de la réponse

# 2. Test avec token
curl -X GET http://localhost:5000/api/Health/secure-ping \
  -H "Authorization: Bearer VOTRE_TOKEN_ICI"

# 3. Recherche pesées
curl -X POST http://localhost:5000/api/Pesee/search \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer VOTRE_TOKEN_ICI" \
  -d '{"tableName":"pesee","acceptes":true,"page":1,"pageSize":10}'
```

### Windows PowerShell :

```powershell
# 1. Login
Invoke-RestMethod -Uri "http://localhost:5000/api/Auth/login" `
  -Method POST `
  -ContentType "application/json" `
  -Body '{"username":"admin","password":"admin123"}'

# 2. Test avec token (remplacez TOKEN)
$headers = @{ Authorization = "Bearer TOKEN" }
Invoke-RestMethod -Uri "http://localhost:5000/api/Health/secure-ping" `
  -Method GET -Headers $headers
```

---

## ✅ Confirmation que tout fonctionne

Quand tous les tests passent :
- ✅ Ping fonctionne
- ✅ Login retourne un token
- ✅ Secure-ping avec token fonctionne
- ✅ Pesee/search avec token retourne des données

→ **L'API est 100% fonctionnelle !**

---

**Besoin d'aide ?** Vérifiez les logs dans la console où `dotnet run` est lancé.
