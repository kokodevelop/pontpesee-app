namespace PontPesee.API.DTOs
{
    public class PeseeDto
    {
        public int CodePesee { get; set; }
        public string? CodeSite { get; set; }
        public string? Campagne { get; set; }
        public string? Immatriculation { get; set; }
        public double? Poids1 { get; set; }
        public DateTime? DateP1 { get; set; }
        public DateTime? DateP2 { get; set; }
        public double? Poids2 { get; set; }
        public double? PoidsNet { get; set; }
        public string? CodeFournisseur { get; set; }
        public string? IdFournisseur { get; set; }
        public string? CodeClient { get; set; }
        public string? NomClient { get; set; }
        public string? Mouvement { get; set; }
        public string? Provenance { get; set; }
        public string? Destination { get; set; }
        public bool Imprime { get; set; }
        public string? NumTicket { get; set; }
        public string? Chauffeur { get; set; }
        public string? Peseur { get; set; }
        public string? Peseur2 { get; set; }
        public bool Annuler { get; set; }
        public string? Label { get; set; }
        public DateTime? Dmv { get; set; }
        public string? Remorque { get; set; }
    }

    public class PeseeFilterDto
    {
        public string? TableName { get; set; } = "pesee";
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public string? Mouvement { get; set; }
        public string? Fournisseur { get; set; }
        public string? Client { get; set; }
        public string? Destination { get; set; }
        public string? Provenance { get; set; }
        public string? Produit { get; set; }
        public string? Vehicule { get; set; }
        public string? Chauffeur { get; set; }
        public string? CodeSite { get; set; }
        public bool? Acceptes { get; set; }
        public bool? Annules { get; set; }
        public bool? Tous { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    public class PeseeStatsDto
    {
        public int NombrePesees { get; set; }
        public double PoidsBrutTotal { get; set; }
        public double PoidsNetTotal { get; set; }
        public List<PeseeDto> Pesees { get; set; } = new();
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
