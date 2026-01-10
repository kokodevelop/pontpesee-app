# 🎯 INSTRUCTIONS SWAGGER - Très Détaillées

## 🚨 PROBLÈME IDENTIFIÉ

Les logs PowerShell montrent:
```
⚠️  JWT Challenge triggered
   Error:
   Error Description:
```

**Cela signifie:** Le backend ne reçoit PAS le token JWT, ou il est mal formaté.

---

## ✅ SOLUTION ÉTAPE PAR ÉTAPE

### ÉTAPE 1: Effacer l'autorisation actuelle

1. Dans Swagger (http://localhost:5059), regardez **en haut à droite**
2. Vous devriez voir un bouton **"Authorize"** avec un cadenas
3. **Cliquez dessus**
4. Dans la popup, cliquez sur **"Logout"** (si disponible) pour effacer l'ancienne autorisation
5. Fermez la popup

---

### ÉTAPE 2: Obtenir un NOUVEAU token

1. Scrollez vers le haut de la page Swagger
2. Trouvez la section **"Auth"**
3. Cliquez sur **POST /api/Auth/login** pour l'ouvrir
4. Cliquez sur **"Try it out"** (bouton bleu à droite)
5. Dans le champ Request body, vous verrez:
   ```json
   {
     "username": "string",
     "password": "string"
   }
   ```

6. **REMPLACEZ PAR** (supprimez "string" et mettez les vraies valeurs):
   ```json
   {
     "username": "admin",
     "password": "0101"
   }
   ```

7. Cliquez sur **"Execute"** (gros bouton bleu en bas)

8. **Vous devriez voir une réponse 200** comme:
   ```json
   {
     "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW4iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBRE1JTklTVFJBVEVVUiIsIlByb2ZpbCI6IkFETUlOSVNUUkFURVVSIiwiZXhwIjoxNzM2NDUxNjcwLCJpc3MiOiJQb250UGVzZWVBUEkiLCJhdWQiOiJQb250UGVzZWVDbGllbnQifQ.abcdef123456...",
     "username": "admin",
     "profil": "ADMINISTRATEUR",
     "expiresAt": "2026-01-09T19:34:30Z"
   }
   ```

9. **SÉLECTIONNEZ ET COPIEZ** tout le contenu du champ `"token"`
   - ⚠️ **ATTENTION:** Ne copiez PAS les guillemets, seulement le contenu entre les guillemets!
   - Le token commence par: `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.`
   - Il fait environ 300-500 caractères

**Comment copier correctement:**
- Cliquez TROIS FOIS sur le token pour tout sélectionner
- Ou faites Ctrl+A dans la zone du token
- Puis Ctrl+C pour copier

---

### ÉTAPE 3: Autoriser Swagger avec le token

10. Scrollez **tout en haut** de la page Swagger
11. Cliquez sur le bouton **"Authorize"** 🔓 (en haut à droite, à côté de "Servers")
12. Une popup s'ouvre avec un champ de texte
13. Dans ce champ, tapez **exactement**:
    ```
    Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJod...
    ```

⚠️ **FORMAT IMPORTANT:**
- Tapez d'abord le mot `Bearer` (avec B majuscule)
- Puis un **ESPACE**
- Puis **collez le token** (Ctrl+V)
- Ne mettez PAS de guillemets
- Ne mettez PAS de retour à la ligne

**Exemple complet:**
```
Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW4iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBRE1JTklTVFJBVEVVUiIsIlByb2ZpbCI6IkFETUlOSVNUUkFURVVSIiwiZXhwIjoxNzM2NDUxNjcwLCJpc3MiOiJQb250UGVzZWVBUEkiLCJhdWQiOiJQb250UGVzZWVDbGllbnQifQ.x4n1gM8HjV3p2qL9kR6wT5yU0oI7eA3bF8cD2sG1mN4
```

14. Cliquez sur **"Authorize"** (bouton dans la popup)
15. La popup devrait afficher un message de confirmation
16. Cliquez sur **"Close"** pour fermer la popup

✅ **Le cadenas devrait maintenant être VERT et FERMÉ** 🔒

---

### ÉTAPE 4: Tester l'endpoint protégé

17. Scrollez vers le bas jusqu'à la section **"Pesee"**
18. Cliquez sur **POST /api/Pesee/search**
19. Cliquez sur **"Try it out"**
20. Dans le Request body, **REMPLACEZ TOUT** par:
```json
{
  "tableName": "pesee",
  "dateDebut": "2020-01-01T00:00:00",
  "dateFin": "2030-12-31T23:59:59",
  "tous": true,
  "page": 0,
  "pageSize": 10
}
```

21. Cliquez sur **"Execute"**

---

## 📊 VÉRIFICATIONS

### Dans Swagger, vous devriez voir:

✅ **Si succès (200 OK):**
```
Code: 200
Response body:
{
  "nombrePesees": 42,
  "poidsBrutTotal": 150000,
  "poidsNetTotal": 135000,
  "pesees": [ ... ]
}
```

❌ **Si erreur (401):**
```
Code: 401
Error: Unauthorized
```

### Dans la console PowerShell, vous devriez voir:

✅ **Si succès:**
```
📨 JWT Token received: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
✅ JWT Token validated successfully
   User: admin
```

❌ **Si erreur (token non reçu):**
```
⚠️  JWT Challenge triggered
   Error:
   Error Description:
```
→ Cela veut dire que le token n'est PAS dans la requête!

❌ **Si erreur (token rejeté):**
```
📨 JWT Token received: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
❌ JWT Authentication FAILED
   Error: [Message d'erreur précis]
```
→ Le token est reçu mais invalide

---

## 🔍 ERREURS FRÉQUENTES

### Erreur 1: Oublier "Bearer " devant le token
❌ **Mauvais:**
```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

✅ **Bon:**
```
Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Erreur 2: Ajouter des guillemets
❌ **Mauvais:**
```
"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

✅ **Bon:**
```
Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Erreur 3: Oublier l'espace après "Bearer"
❌ **Mauvais:**
```
BearereyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

✅ **Bon:**
```
Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Erreur 4: Copier seulement une partie du token
❌ **Mauvais:**
```
Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```
(les "..." sont vraiment dans le token copié)

✅ **Bon:**
```
Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW4iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBRE1JTklTVFJBVEVVUiIsIlByb2ZpbCI6IkFETUlOSVNUUkFURVVSIiwiZXhwIjoxNzM2NDUxNjcwLCJpc3MiOiJQb250UGVzZWVBUEkiLCJhdWQiOiJQb250UGVzZWVDbGllbnQifQ.x4n1gM8HjV3p2qL9kR6wT5yU0oI7eA3bF8cD2sG1mN4
```
(token complet, environ 400 caractères)

---

## 📤 SI ÇA NE MARCHE TOUJOURS PAS

Envoyez-moi 3 screenshots:

### Screenshot 1: Réponse du LOGIN
Montrant la réponse complète de POST /api/Auth/login avec le token visible

### Screenshot 2: Popup Authorize
Montrant la popup "Authorize" avec le contenu du champ "Value" (avec Bearer + token)

### Screenshot 3: Résultat du POST /api/Pesee/search
Montrant le code de réponse (200 ou 401)

### Screenshot 4: Console PowerShell
Montrant les logs pendant le test POST /api/Pesee/search

---

## 🎯 CHECKLIST COMPLÈTE

- [ ] Backend redémarré et qui tourne sur http://localhost:5059
- [ ] Swagger ouvert dans le navigateur
- [ ] POST /api/Auth/login exécuté → 200 OK
- [ ] Token copié (tout le contenu entre guillemets, environ 400 caractères)
- [ ] Cliqué sur "Authorize" en haut à droite
- [ ] Tapé "Bearer " (avec espace) puis collé le token
- [ ] Cliqué sur "Authorize" dans la popup
- [ ] Cadenas devenu vert et fermé 🔒
- [ ] POST /api/Pesee/search exécuté
- [ ] Résultat observé dans Swagger ET dans PowerShell

---

**🚀 MAINTENANT: Suivez ces étapes précises dans l'ordre!**
