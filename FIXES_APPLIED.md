# ✅ Corrections Appliquées pour l'Erreur 401

## 📋 Résumé du problème

**Symptôme** :
- ✅ `/api/Auth/login` fonctionne et retourne un token
- ❌ `/api/Pesee/*` retourne erreur 401 Unauthorized

**Cause probable** :
- Token JWT non validé correctement
- Ou token non envoyé dans les requêtes suivantes

---

## 🔧 Corrections Appliquées

### 1. ✅ Ajout de logs de debug JWT (Program.cs)

**Fichier modifié** : `backend/PontPesee.API/Program.cs`

**Ajout** :
```csharp
options.Events = new JwtBearerEvents
{
    OnAuthenticationFailed = context =>
    {
        Console.WriteLine($"JWT Authentication failed: {context.Exception.Message}");
        return Task.CompletedTask;
    },
    OnTokenValidated = context =>
    {
        Console.WriteLine("JWT Token validated successfully");
        return Task.CompletedTask;
    },
    OnMessageReceived = context =>
    {
        Console.WriteLine($"JWT Token received: {context.Token?.Substring(0, 20)}...");
        return Task.CompletedTask;
    }
};
```

**Avantage** : Vous pouvez maintenant voir dans la console si le token est bien reçu et validé.

---

### 2. ✅ Création d'un controller de test (HealthController.cs)

**Nouveau fichier** : `backend/PontPesee.API/Controllers/HealthController.cs`

**Endpoints créés** :
- `GET /api/Health/ping` - Test SANS authentification
- `GET /api/Health/secure-ping` - Test AVEC authentification

**Avantage** : Permet de tester l'authentification isolément, sans la logique métier.

---

### 3. ✅ Page de test HTML interactive (test-api.html)

**Nouveau fichier** : `PONT_WEBAPP/test-api.html`

**Fonctionnalités** :
- Test de connexion à l'API
- Login et stockage du token
- Test des endpoints protégés
- Recherche de pesées
- Interface visuelle simple

**Avantage** : Pas besoin de Postman, test directement dans le navigateur.

---

### 4. ✅ Documentation complète (3 fichiers)

**Fichiers créés** :

1. **TEST_API.md** - Guide de test complet avec exemples cURL et PowerShell
2. **RESOLUTION_401.md** - Guide de résolution de l'erreur 401
3. **FIXES_APPLIED.md** - Ce fichier

**Avantage** : Documentation claire pour diagnostiquer et résoudre le problème.

---

## 🚀 Prochaines Étapes

### Étape 1 : Redémarrer le backend

```bash
# Arrêtez le backend actuel (Ctrl+C)
cd D:\PROJETVBNET2015\PONT VBPP2 CAM2ADMIN\PONT_WEBAPP\backend\PontPesee.API
dotnet run
```

### Étape 2 : Tester avec la page HTML

1. Ouvrez `test-api.html` dans votre navigateur
2. Suivez les 4 tests dans l'ordre
3. Observez les résultats

### Étape 3 : Vérifier les logs

Dans la console où `dotnet run` est lancé, vous devriez voir :

```
JWT Token received: eyJhbGciOiJIUzI1Ni...
JWT Token validated successfully
```

---

## 🔍 Diagnostics Possibles

### Scénario 1 : Test 1 échoue (Ping)

**Symptôme** : `/api/Health/ping` retourne une erreur

**Cause** : L'API ne fonctionne pas du tout

**Solution** :
- Vérifiez que le backend est bien démarré
- Vérifiez l'URL (http://localhost:5000/api)
- Vérifiez qu'aucun firewall ne bloque

---

### Scénario 2 : Test 2 échoue (Login)

**Symptôme** : `/api/Auth/login` retourne 401 ou 500

**Cause** :
- L'utilisateur n'existe pas dans la base
- Problème de connexion MySQL

**Solution** :
```sql
-- Vérifiez l'utilisateur
SELECT * FROM user WHERE NomUt = 'admin';

-- Si vide, créez-le
INSERT INTO user (NomUt, motpasse, LeGroupe, Actif)
VALUES ('admin', 'admin123', 'SUPERVISEUR', 1);
```

---

### Scénario 3 : Test 3 échoue (Secure Ping)

**Symptôme** : `/api/Health/secure-ping` retourne 401

**Cause** : Le token n'est pas validé correctement

**Vérifications** :

1. **Vérifiez la console backend** :
   ```
   JWT Token received: eyJ...
   JWT Authentication failed: ...
   ```

   → Regardez le message d'erreur

2. **Erreurs courantes** :

   **a) "The signature is invalid"**
   - Cause : Clé JWT incorrecte
   - Solution : Vérifiez `appsettings.json` ligne 13
   - La clé doit faire 32+ caractères

   **b) "The token is expired"**
   - Cause : Token > 8 heures
   - Solution : Reconnectez-vous

   **c) "IDX10214: Audience validation failed"**
   - Cause : Audience ne correspond pas
   - Solution : Vérifiez `Jwt:Audience` dans `appsettings.json`

   **d) "IDX10205: Issuer validation failed"**
   - Cause : Issuer ne correspond pas
   - Solution : Vérifiez `Jwt:Issuer` dans `appsettings.json`

---

### Scénario 4 : Test 4 échoue (Search)

**Symptôme** : `/api/Pesee/search` retourne 401

**Cause** : Même que Scénario 3 (problème de token)

**Solution** : Résolvez d'abord le Test 3

---

### Scénario 5 : Tests 1-3 passent, mais 4 échoue avec une autre erreur

**Symptôme** : 500 Internal Server Error

**Cause** : Problème dans la logique métier (base de données, requête SQL)

**Vérifications** :
```sql
-- Vérifiez que les tables existent
SHOW TABLES LIKE 'pesee%';

-- Vérifiez la structure
DESCRIBE pesee;
```

---

## 📊 Tableau de Diagnostic

| Test | URL | Auth | Résultat attendu | Si échec |
|------|-----|------|------------------|----------|
| 1️⃣ | /Health/ping | ❌ Non | 200 OK | Backend KO |
| 2️⃣ | /Auth/login | ❌ Non | 200 + token | User inexistant |
| 3️⃣ | /Health/secure-ping | ✅ Oui | 200 OK | JWT KO |
| 4️⃣ | /Pesee/search | ✅ Oui | 200 + données | Tables KO |

---

## 🛠️ Commandes Utiles

### Redémarrer le backend
```bash
cd backend/PontPesee.API
dotnet run
```

### Vérifier la base de données
```bash
mysql -h fga.sitsci.com -u pontuser -p
USE gestpeseedb;
SHOW TABLES;
SELECT COUNT(*) FROM pesee;
SELECT * FROM user LIMIT 5;
```

### Logs détaillés
Modifiez `appsettings.json` :
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore.Authentication": "Debug"
    }
  }
}
```

---

## ✅ Validation Finale

Quand tous les tests passent dans `test-api.html` :

- ✅ **Test 1** : Ping OK
- ✅ **Test 2** : Login retourne un token
- ✅ **Test 3** : Secure-ping OK avec token
- ✅ **Test 4** : Search retourne des pesées

→ **L'API backend est 100% fonctionnelle !** 🎉

Le problème pourrait alors venir du frontend (Vue.js).

---

## 🎨 Si le backend fonctionne mais pas le frontend

### Vérifications frontend :

1. **Console du navigateur (F12)** :
   - Vérifiez les erreurs réseau
   - Vérifiez que le token est envoyé dans le header

2. **Network tab** :
   - Cliquez sur une requête `/api/Pesee/search`
   - Onglet "Headers"
   - Vérifiez `Authorization: Bearer ...`

3. **localStorage** :
   - Console > `localStorage.getItem('token')`
   - Le token doit être présent

### Code à vérifier :

**Fichier** : `frontend/pont-pesee-app/src/services/api.ts`

Ligne 16-18 :
```typescript
const token = localStorage.getItem('token')
if (token) {
  config.headers.Authorization = `Bearer ${token}`
}
```

---

## 📞 Support

Si le problème persiste après tous ces tests :

1. Copiez les logs de la console backend
2. Faites une capture d'écran de `test-api.html` avec les résultats
3. Vérifiez le fichier `appsettings.json`
4. Vérifiez qu'un utilisateur existe dans MySQL

---

## 📁 Fichiers Modifiés/Créés

### Modifiés :
- ✅ `backend/PontPesee.API/Program.cs` (ajout logs JWT)

### Créés :
- ✅ `backend/PontPesee.API/Controllers/HealthController.cs`
- ✅ `test-api.html`
- ✅ `TEST_API.md`
- ✅ `RESOLUTION_401.md`
- ✅ `FIXES_APPLIED.md`

---

**Bonne chance ! 🚀**
