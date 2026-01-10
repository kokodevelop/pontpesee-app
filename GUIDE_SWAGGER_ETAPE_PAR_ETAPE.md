# 🎯 GUIDE SWAGGER - Étape par Étape

## ✅ BONNE NOUVELLE!

Vos logs montrent que:
1. ✅ La clé JWT est bien chargée (54 caractères)
2. ✅ Le backend fonctionne sur http://localhost:5059
3. ✅ La base de données est connectée (requête SQL visible)
4. ✅ AuthService génère bien le token

**Le problème:** Vous n'avez probablement pas cliqué sur "Authorize" dans Swagger!

---

## 📋 ÉTAPES À SUIVRE (DANS L'ORDRE!)

### ÉTAPE 1: Login pour obtenir le token

1. Dans Swagger (http://localhost:5059), cherchez **POST /api/Auth/login**
2. Cliquez sur la section pour l'ouvrir
3. Cliquez sur **"Try it out"** (bouton bleu en haut à droite)
4. Dans le corps de la requête, entrez:
   ```json
   {
     "username": "ADMIN",
     "password": "votre_mot_de_passe_ici"
   }
   ```
   ⚠️ Remplacez par votre vrai mot de passe!

5. Cliquez sur **"Execute"** (bouton bleu)
6. Vous devriez voir une réponse **200 OK** avec:
   ```json
   {
     "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJod...",
     "username": "ADMIN",
     "profil": "ADMINISTRATEUR",
     "expiresAt": "2026-01-09T..."
   }
   ```

7. **COPIEZ TOUT LE TOKEN** (la longue chaîne qui commence par "eyJ...")
   - Sélectionnez uniquement le texte entre les guillemets
   - Ne copiez PAS les guillemets
   - Le token fait environ 300-400 caractères

---

### ÉTAPE 2: Autoriser Swagger avec le token

8. **EN HAUT À DROITE de la page Swagger**, cherchez le bouton **"Authorize"** 🔓
   - C'est un gros bouton avec une icône de cadenas
   - Il est à côté de "Servers"

9. **Cliquez sur "Authorize"**
   - Une popup s'ouvre avec un champ texte

10. Dans le champ "Value", tapez:
    ```
    Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJod...
    ```
    ⚠️ **IMPORTANT:**
    - Tapez d'abord le mot `Bearer` (avec majuscule)
    - Puis un ESPACE
    - Puis collez votre token

    **Exemple complet:**
    ```
    Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiQURNSU4iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBRE1JTklTVFJBVEVVUiIsIlByb2ZpbCI6IkFETUlOSVNUUkFURVVSIiwiZXhwIjoxNzY4MDM3MzI5LCJpc3MiOiJQb250UGVzZWVBUEkiLCJhdWQiOiJQb250UGVzZWVDbGllbnQifQ.uUXnG_JT0Z0oD9SZJ0I0K3jGXq8pv_aL5FzKpQ5bE3o
    ```

11. Cliquez sur **"Authorize"** (le bouton dans la popup)
12. Cliquez sur **"Close"** pour fermer la popup

**✅ Vous verrez maintenant que le cadenas 🔓 est devenu fermé 🔒**

---

### ÉTAPE 3: Tester l'endpoint protégé

13. Cherchez **POST /api/Pesee/search**
14. Cliquez dessus pour l'ouvrir
15. Cliquez sur **"Try it out"**
16. Dans le corps de la requête, modifiez pour simplifier:
    ```json
    {
      "tableName": "pesee",
      "dateDebut": "2020-01-01",
      "dateFin": "2030-12-31",
      "tous": true,
      "page": 0,
      "pageSize": 10
    }
    ```
    ⚠️ Gardez seulement ces champs, supprimez les autres "string"

17. Cliquez sur **"Execute"**

---

## 📊 RÉSULTAT ATTENDU

### ✅ Succès (200 OK)

Vous verrez:
```
Code: 200
```

Et dans le corps de la réponse:
```json
{
  "nombrePesees": 150,
  "poidsBrutTotal": 125000,
  "poidsNetTotal": 110000,
  "pesees": [
    {
      "codePesee": 1,
      "numTicket": "T001",
      "dateMatriculation": "2026-01-05",
      ...
    }
  ]
}
```

**Dans la console PowerShell, vous verrez:**
```
📨 JWT Token received: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
✅ JWT Token validated successfully
   User: ADMIN
```

---

### ❌ Erreur 401 (si vous n'avez pas autorisé)

Vous verrez:
```
Code: 401
Error: Unauthorized
www-authenticate: Bearer
```

**Dans la console PowerShell, vous NE verrez PAS** de logs JWT, car la requête n'a pas de token!

---

### ❌ Erreur 401 (si le token est expiré)

**Dans la console PowerShell, vous verrez:**
```
📨 JWT Token received: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
❌ JWT Authentication FAILED
   Error: IDX10223: Lifetime validation failed. The token is expired.
```

**Solution:** Refaire l'étape 1 (Login) pour obtenir un nouveau token.

---

## 🔍 VÉRIFICATIONS

### Comment savoir si j'ai bien autorisé Swagger?

Regardez **en haut à droite** de la page Swagger:
- 🔓 Cadenas ouvert = **PAS AUTORISÉ**
- 🔒 Cadenas fermé = **AUTORISÉ** ✅

### Comment savoir si mon token est valide?

Allez sur https://jwt.io et collez votre token.

Regardez le champ `"exp"` dans le payload décodé:
```json
{
  "exp": 1768037329
}
```

Ce nombre est un timestamp Unix. Pour vérifier s'il est expiré:
- Allez sur https://www.unixtimestamp.com/
- Collez le nombre
- Si la date est dans le passé, le token est expiré

**OU** regardez directement le champ `"expiresAt"` dans la réponse du login:
```json
{
  "expiresAt": "2026-01-09T18:02:09Z"
}
```

Si cette date est passée, le token est expiré.

---

## 🎯 CHECKLIST COMPLÈTE

- [ ] POST /api/Auth/login → 200 OK → Token reçu
- [ ] Token copié (toute la chaîne eyJ...)
- [ ] Cliqué sur "Authorize" 🔓 en haut à droite
- [ ] Tapé "Bearer " (avec espace) puis collé le token
- [ ] Cliqué sur "Authorize" dans la popup
- [ ] Fermé la popup → Cadenas devient 🔒
- [ ] POST /api/Pesee/search → "Try it out"
- [ ] Modifié le JSON pour simplifier
- [ ] Cliqué sur "Execute"
- [ ] Résultat: **200 OK** ✅

---

## 📤 SI ÇA NE MARCHE TOUJOURS PAS

Envoyez-moi:

1. **Screenshot de la page Swagger** montrant:
   - Le cadenas en haut à droite (🔓 ou 🔒)
   - Le résultat de POST /api/Pesee/search

2. **Screenshot de la console PowerShell** montrant les logs JWT

3. **Le token décodé** depuis jwt.io (payload JSON uniquement)

---

## 🚀 MAINTENANT

Suivez les 17 étapes ci-dessus dans l'ordre, sans sauter d'étapes!

**L'étape la plus importante est l'ÉTAPE 2** - Cliquer sur "Authorize" 🔓
