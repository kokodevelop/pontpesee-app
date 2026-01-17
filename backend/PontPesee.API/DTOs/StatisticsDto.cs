namespace PontPesee.API.DTOs
{
    /// <summary>
    /// Statistiques par entité (fournisseur, client, produit, etc.)
    /// </summary>
    public class EntityStatDto
    {
        public string Code { get; set; } = string.Empty;
        public string Nom { get; set; } = string.Empty;
        public int NombrePesees { get; set; }
        public double PoidsBrutTotal { get; set; }
        public double PoidsNetTotal { get; set; }
        public double PoidsMoyen { get; set; }
    }

    /// <summary>
    /// Statistiques par période (jour, semaine, mois)
    /// </summary>
    public class PeriodStatDto
    {
        public string Periode { get; set; } = string.Empty;
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public int NombrePesees { get; set; }
        public double PoidsBrutTotal { get; set; }
        public double PoidsNetTotal { get; set; }
    }

    /// <summary>
    /// Statistiques par mouvement (Entrée/Sortie)
    /// </summary>
    public class MouvementStatDto
    {
        public string Mouvement { get; set; } = string.Empty;
        public int NombrePesees { get; set; }
        public double PoidsBrutTotal { get; set; }
        public double PoidsNetTotal { get; set; }
        public double Pourcentage { get; set; }
    }

    /// <summary>
    /// Résumé global des statistiques
    /// </summary>
    public class StatisticsSummaryDto
    {
        public int TotalPesees { get; set; }
        public double TotalPoidsBrut { get; set; }
        public double TotalPoidsNet { get; set; }
        public double PoidsMoyen { get; set; }
        public int NombreFournisseurs { get; set; }
        public int NombreClients { get; set; }
        public int NombreProduits { get; set; }
        public int NombreVehicules { get; set; }
        public DateTime? PremierePesee { get; set; }
        public DateTime? DernierePesee { get; set; }
    }

    /// <summary>
    /// Réponse complète des statistiques avancées
    /// </summary>
    public class AdvancedStatisticsDto
    {
        public StatisticsSummaryDto Summary { get; set; } = new();
        public List<EntityStatDto> ParFournisseur { get; set; } = new();
        public List<EntityStatDto> ParClient { get; set; } = new();
        public List<EntityStatDto> ParProduit { get; set; } = new();
        public List<MouvementStatDto> ParMouvement { get; set; } = new();
        public List<PeriodStatDto> ParJour { get; set; } = new();
        public List<PeriodStatDto> ParMois { get; set; } = new();
    }

    /// <summary>
    /// Filtre pour les statistiques avancées
    /// </summary>
    public class StatisticsFilterDto
    {
        public string? TableName { get; set; } = "pesee";
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public string? Mouvement { get; set; }
        public string? Fournisseur { get; set; }
        public string? Client { get; set; }
        public string? Produit { get; set; }
        public string? CodeSite { get; set; }
        public int TopN { get; set; } = 10; // Nombre de résultats top pour chaque catégorie
    }
}
