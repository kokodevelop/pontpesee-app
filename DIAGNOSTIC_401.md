# 🔍 DIAGNOSTIC - Erreur 401 Identifiée

## 📸 Ce que je vois dans votre screenshot

```
Request URL: http://localhost:5059/api/Pesee/search
Code: 401
Error: Unauthorized
Response headers: www-authenticate: Bearer
```

## 🎯 PROBLÈME IDENTIFIÉ

**Le backend répond `www-authenticate: Bearer`** → Cela signifie que le serveur **attend** un token JWT, mais il le **rejette**.

## 🔧 CAUSE PROBABLE #1 : HTTPS Redirection

Dans [Program.cs:127](backend/PontPesee.API/Program.cs#L127), il y a :

```csharp
app.UseHttpsRedirection();
```

**Problème** : Vous testez sur `http://localhost:5059` (HTTP), mais cette ligne force une redirection vers HTTPS, ce qui peut perturber l'authentification.

---

## ✅ SOLUTION RAPIDE

### Option 1 : Désactiver HTTPS redirection (recommandé pour le dev)

Éditez [Program.cs](backend/PontPesee.API/Program.cs) ligne 127 :

```csharp
// Commentez cette ligne en dev
// app.UseHttpsRedirection();
```

**Ou** ajoutez une condition :

```csharp
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
```

### Option 2 : Utiliser HTTPS

Modifiez votre URL de test pour utiliser `https://localhost:7059` au lieu de `http://localhost:5059`

⚠️ **Attention** : Avec HTTPS, vous pourriez avoir des warnings de certificat SSL en dev.

---

## 🔧 CORRECTION IMMÉDIATE

Je vais modifier Program.cs pour désactiver HTTPS redirection en développement.

### Fichier à modifier : backend/PontPesee.API/Program.cs

**AVANT (ligne 127)** :
```csharp
app.UseHttpsRedirection();
```

**APRÈS** :
```csharp
// Désactivé en dev pour tester sur HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
```

---

## 🧪 APRÈS LA CORRECTION

1. **Redémarrez le backend** :
   ```bash
   cd backend\PontPesee.API
   dotnet run
   ```

2. **Retestez dans Swagger** :
   - Allez sur http://localhost:5059
   - Cliquez sur **Authorize** (en haut à droite)
   - Entrez : `Bearer VotreCleSecretePourJWT_MinimumTrenteDeuxCaracteres2025`
   - Testez `/api/Pesee/search`

3. **Vérifiez la console** pour voir les messages JWT :
   ```
   JWT Token received: eyJhbGciOiJIUzI1Ni...
   JWT Token validated successfully
   ```

---

## 🔍 SI L'ERREUR PERSISTE

### Vérification 1 : Le token est-il bien généré ?

Dans Swagger, après le login, copiez le token et vérifiez-le sur https://jwt.io

Vous devriez voir :
```json
{
  "header": {
    "alg": "HS256",
    "typ": "JWT"
  },
  "payload": {
    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name": "votreUsername",
    "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": "ADMIN",
    "Profil": "ADMIN",
    "exp": 1746808635,
    "iss": "PontPeseeAPI",
    "aud": "PontPeseeClient"
  }
}
```

⚠️ **Vérifiez que** :
- `iss` = "PontPeseeAPI" (doit matcher appsettings.json ligne 14)
- `aud` = "PontPeseeClient" (doit matcher appsettings.json ligne 15)
- `exp` est dans le futur (timestamp Unix)

### Vérification 2 : Les logs backend

Dans la console du backend (`dotnet run`), vous devez voir :

✅ **Si ça marche** :
```
JWT Token received: eyJhbGciOiJIUzI1Ni...
JWT Token validated successfully
```

❌ **Si erreur** :
```
JWT Authentication failed: [MESSAGE]
```

Le message d'erreur vous dira exactement le problème :
- `The token has no expiration` → Problème d'expiration
- `IDX10214: Audience validation failed` → Problème d'Audience
- `IDX10205: Issuer validation failed` → Problème d'Issuer
- `IDX10503: Signature validation failed` → Problème de clé

---

## 📝 CHECKLIST DE VÉRIFICATION

- [ ] HTTPS redirection désactivée en dev
- [ ] Backend redémarré
- [ ] Token JWT vérifié sur jwt.io
- [ ] Issuer/Audience correspondent à appsettings.json
- [ ] Token non expiré
- [ ] Logs backend consultés
- [ ] Test dans Swagger avec Authorization

---

## 🎯 PROCHAINE ÉTAPE

Une fois que vous avez appliqué la correction, testez et envoyez-moi :

1. **Screenshot du résultat dans Swagger** (succès ou échec)
2. **Copie des logs de la console backend**
3. **Le token JWT décodé** (copiez le payload depuis jwt.io)

Je pourrai alors affiner le diagnostic si nécessaire !

---

**🚀 ACTION : Appliquer la correction dans Program.cs et redémarrer le backend**
