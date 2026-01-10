# 🚀 Guide de Démarrage Rapide

## Étapes pour lancer l'application en 5 minutes

### ✅ Prérequis vérifiés
- [ ] .NET SDK 8.0 installé
- [ ] Node.js 18+ installé
- [ ] MySQL Server accessible
- [ ] Base de données avec tables `pesee`, `peseep2` et `user`

---

## 🔧 Configuration Rapide

### 1. Configuration Backend (2 minutes)

Ouvrez : `backend/PontPesee.API/appsettings.json`

Modifiez la ligne 10 avec VOS informations :

```json
"DefaultConnection": "Server=VOTRE_SERVEUR;Port=3306;Database=VOTRE_BASE;User=VOTRE_USER;Password=VOTRE_MDP;SslMode=none;"
```

**Exemples :**
```json
// Connexion locale
"Server=localhost;Port=3306;Database=gestpeseedb;User=root;Password=root;SslMode=none;"

// Connexion distante (votre cas)
"Server=54.36.224.114;Port=3306;Database=gestpeseedb;User=pontuser;Password=Soprol@2022;SslMode=none;"
```

### 2. Démarrage Backend (1 minute)

Ouvrez un terminal (CMD ou PowerShell) :

```bash
cd D:\PROJETVBNET2015\PONT VBPP2 CAM2ADMIN\PONT_WEBAPP\backend\PontPesee.API
dotnet run
```

✅ **Succès si vous voyez :**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
```

🌐 **Testez l'API** : Ouvrez http://localhost:5000 dans votre navigateur
Vous devriez voir Swagger (documentation interactive)

---

### 3. Démarrage Frontend (1 minute)

Ouvrez un NOUVEAU terminal :

```bash
cd D:\PROJETVBNET2015\PONT VBPP2 CAM2ADMIN\PONT_WEBAPP\frontend\pont-pesee-app
npm run dev
```

✅ **Succès si vous voyez :**
```
VITE v5.x.x  ready in xxx ms

  ➜  Local:   http://localhost:5173/
```

🌐 **Ouvrez l'application** : http://localhost:5173

---

## 🎯 Premier Test

### Test de connexion

1. Ouvrez http://localhost:5173
2. Vous devriez voir la page de login
3. Entrez vos identifiants de la table `user`
4. Si ça marche 🎉 Vous êtes connecté !

### En cas d'erreur

#### Erreur "Cannot connect to MySQL"
```bash
# Vérifiez que MySQL est démarré
mysql -h SERVEUR -u USER -p

# Testez la connexion
mysql -h 54.36.224.114 -u pontuser -p
# Entrez le mot de passe : Soprol@2022
```

#### Erreur "Network Error" dans le frontend
```bash
# Vérifiez que le backend tourne sur le port 5000
# Ouvrez http://localhost:5000 dans le navigateur
```

#### Erreur "Unauthorized"
```bash
# Vérifiez que vous avez un utilisateur dans la table `user`
# Exemple de requête SQL :
INSERT INTO user (NomUt, motpasse, LeGroupe, Actif)
VALUES ('admin', 'admin123', 'SUPERVISEUR', 1);
```

---

## 🎨 Utilisation de base

### 1. Page Dashboard (Menu principal)
- Tuiles cliquables
- Accès aux fonctionnalités

### 2. Page Stats
- **Sélectionner le site** : SOPRECI GUTRI ou OLODIO
- **Choisir les dates**
- **Appliquer les filtres**
- **Consulter les statistiques**

### 3. Réédition
- Rechercher un ticket par numéro
- Réimprimer

---

## 📊 Structure de la base de données

Votre base doit avoir au minimum :

```sql
-- Table user (pour l'authentification)
CREATE TABLE IF NOT EXISTS `user` (
  `id` INT PRIMARY KEY AUTO_INCREMENT,
  `NomUt` VARCHAR(100) NOT NULL,
  `motpasse` VARCHAR(255) NOT NULL,
  `LeGroupe` VARCHAR(50),
  `Actif` BOOLEAN DEFAULT TRUE
);

-- Insérer un utilisateur test
INSERT INTO user (NomUt, motpasse, LeGroupe, Actif)
VALUES ('admin', 'admin123', 'SUPERVISEUR', 1);

-- Tables pesee et peseep2 (doivent déjà exister)
```

---

## 🆘 Aide rapide

### Commandes utiles

```bash
# Arrêter le backend : Ctrl+C dans le terminal
# Arrêter le frontend : Ctrl+C dans le terminal

# Relancer le backend
dotnet run

# Relancer le frontend
npm run dev

# Voir les logs backend : directement dans le terminal
# Voir les logs frontend : F12 > Console dans le navigateur
```

### Ports utilisés

- **Backend API** : http://localhost:5000
- **Frontend** : http://localhost:5173
- **MySQL** : localhost:3306 (ou votre serveur distant)

---

## ✨ Prochaines étapes

Une fois l'application fonctionnelle :

1. **Testez la recherche de pesées**
2. **Changez de site** (GUTRI ↔ OLODIO)
3. **Explorez les filtres**
4. **Consultez Swagger** pour l'API : http://localhost:5000

---

## 📞 Besoin d'aide ?

1. Vérifiez les logs dans les terminaux
2. Consultez le [README.md](README.md) complet
3. Ouvrez F12 dans le navigateur pour voir les erreurs
4. Testez l'API directement dans Swagger

---

**🎉 Félicitations ! Votre application web est prête !**
