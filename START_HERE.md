# 🎯 COMMENCEZ ICI - Résolution Erreur 401

## ⚡ Action Rapide

Vous avez une erreur 401 sur `/api/Pesee/*` ? Suivez ces 3 étapes :

---

## Étape 1 : Redémarrer le Backend ⚙️

**Terminal 1** (arrêtez avec Ctrl+C si déjà lancé) :
```bash
cd D:\PROJETVBNET2015\PONT VBPP2 CAM2ADMIN\PONT_WEBAPP\backend\PontPesee.API
dotnet run
```

✅ **Vérifiez** : Vous devez voir "Now listening on: http://localhost:5000"

---

## Étape 2 : Tester avec la Page HTML 🧪

**Double-cliquez** sur ce fichier :
```
D:\PROJETVBNET2015\PONT VBPP2 CAM2ADMIN\PONT_WEBAPP\test-api.html
```

**Suivez les tests dans l'ordre** :

### Test 1 : Ping ✅
Cliquez sur "Tester /api/Health/ping"
- ✅ Doit afficher "API is running"
- ❌ Si erreur → Backend ne fonctionne pas

### Test 2 : Login 🔐
1. Entrez : `admin` / `admin123`
2. Cliquez "Se connecter"
3. ✅ Doit afficher un token (très long texte)
4. ❌ Si erreur → L'utilisateur n'existe pas dans MySQL

### Test 3 : Secure Ping 🔒
Cliquez sur "Tester /api/Health/secure-ping"
- ✅ Doit afficher "You are authenticated"
- ❌ Si erreur 401 → **C'EST ICI LE PROBLÈME JWT**

### Test 4 : Recherche 🔍
Cliquez sur "Rechercher"
- ✅ Doit afficher des pesées
- ❌ Si erreur 401 → Même problème que Test 3

---

## Étape 3 : Vérifier les Logs 📊

**Dans la console** où `dotnet run` est lancé, cherchez :

### ✅ Si ça marche :
```
JWT Token received: eyJhbGciOiJIUzI1Ni...
JWT Token validated successfully
```

### ❌ Si erreur JWT :
```
JWT Authentication failed: The signature is invalid
```

---

## 🔧 Solutions Rapides

### Problème : Test 2 échoue (Login)

**L'utilisateur n'existe pas dans MySQL**

```sql
-- Connectez-vous à MySQL
mysql -h fga.sitsci.com -u pontuser -p

-- Créez l'utilisateur
USE gestpeseedb;
INSERT INTO user (NomUt, motpasse, LeGroupe, Actif)
VALUES ('admin', 'admin123', 'SUPERVISEUR', 1);
```

---

### Problème : Test 3 échoue (Erreur 401)

**Le token n'est pas validé**

**Message : "The signature is invalid"**

→ Vérifiez `backend/PontPesee.API/appsettings.json` ligne 13 :

```json
"Key": "VotreCleSecretePourJWT_MinimumTrenteDeuxCaracteres2025"
```

La clé doit faire **au moins 32 caractères** !

**Solution** :
1. Arrêtez le backend (Ctrl+C)
2. Vérifiez la clé dans `appsettings.json`
3. Relancez : `dotnet run`
4. Refaites le Test 2 (Login) pour obtenir un nouveau token
5. Refaites le Test 3

---

### Problème : Test 4 échoue (500 Error)

**Les tables n'existent pas**

```sql
-- Vérifiez les tables
USE gestpeseedb;
SHOW TABLES LIKE 'pesee%';

-- Vous devez voir :
-- pesee
-- peseep2
```

Si elles n'existent pas → Les tables doivent être créées dans votre base.

---

## 📖 Documentation Complète

Si vous avez besoin de plus de détails :

1. **RESOLUTION_401.md** - Guide complet de résolution
2. **TEST_API.md** - Exemples de tests avec cURL/Postman
3. **FIXES_APPLIED.md** - Liste des corrections appliquées

---

## ✅ Validation Finale

Quand **TOUS les tests passent** dans `test-api.html` :

- ✅ Test 1 (Ping) : OK
- ✅ Test 2 (Login) : Token reçu
- ✅ Test 3 (Secure Ping) : Authentification OK
- ✅ Test 4 (Search) : Données reçues

→ **🎉 L'API est FONCTIONNELLE !**

Le problème vient alors du frontend Vue.js.

---

## 🎨 Si le Backend fonctionne mais pas le Frontend

### Vérifiez que le token est envoyé

**Ouvrez la console du navigateur (F12)** :

1. Allez dans l'onglet "Network"
2. Faites une requête `/api/Pesee/search`
3. Cliquez sur la requête
4. Onglet "Headers"
5. Cherchez : `Authorization: Bearer ...`

**Si absent** → Le frontend n'envoie pas le token
**Si présent** → Le token n'est pas valide (refaites un login)

---

## 🆘 Besoin d'Aide ?

**Informations à fournir** :

1. Résultat des 4 tests dans `test-api.html`
2. Logs de la console backend
3. Contenu de `appsettings.json` (SANS les mots de passe)
4. Résultat de : `SELECT * FROM user LIMIT 1;`

---

**Suivez ces étapes dans l'ordre et vous trouverez le problème ! 🚀**
