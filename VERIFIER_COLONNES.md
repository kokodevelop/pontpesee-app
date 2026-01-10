# 🔍 Vérification des Colonnes

## Problème

L'erreur `Unable to cast object of type 'System.Int32' to type 'System.String'` vient du fait qu'Entity Framework essaie de mapper une colonne qui n'existe pas ou qui a un type différent.

## ✅ Action Immédiate

Exécutez cette requête SQL dans votre base de données pour voir la structure exacte de la table `pesee`:

```sql
DESCRIBE pesee;
```

Ou:

```sql
SHOW COLUMNS FROM pesee;
```

## 🎯 Colonnes que le modèle C# attend

Voici toutes les colonnes définies dans le modèle `Pesee.cs`:

1. codepesee (int - clé primaire)
2. CodeSite (varchar)
3. Campagne (varchar)
4. Immatriculation (varchar)
5. Poids1 (double)
6. DateP1 (datetime)
7. DateP2 (datetime)
8. Poids2 (double)
9. PoidsNet (double)
10. CodeFournisseur (varchar)
11. IdFournisseur (varchar)
12. CodeClient (varchar)
13. nomclient (varchar)
14. Mouvement (varchar)
15. Provenance (varchar)
16. Destination (varchar)
17. imprime (boolean/tinyint)
18. NumTicket (varchar) ← **C'est probablement ici le problème!**
19. Exportateur (varchar)
20. Transporteur (varchar)
21. Chauffeur (varchar)
22. Barcode (varchar)
23. heureP1 (datetime)
24. heureP2 (datetime)
25. Peseur (varchar)
26. Peseur2 (varchar)
27. Annuler (boolean/tinyint)
28. Label (varchar)
29. Ref_Piece (varchar)
30. cnsment (varchar)
31. IDEmballage (varchar)
32. PoidsEmb (double)
33. Refac2 (double)
34. Emballage (varchar)
35. dmv (datetime)
36. ImageVeh (blob)
37. remorque (varchar)

## 🔎 Vérifiez

1. **NumTicket** existe-t-il dans votre table `pesee`?
2. Si oui, quel est son type? (VARCHAR, CHAR, INT?)
3. Y a-t-il des colonnes dans le modèle C# qui n'existent PAS dans votre table MySQL?

## 💡 Solution Temporaire

En attendant, je vais créer un modèle minimal qui n'utilise que les champs essentiels.
