namespace Domain.Entities
{
    public class Dashboard
    {
        public int TotalUsuarios { get; set; }
        public int TotalUsuarioHoje { get; set; }

        public int TotalPecasDisponiveis { get; set; }
        public int TotalPecasDisponiveisHoje { get; set; }

        public int TotalMarcas { get; set; }

        public int TotalTrocas { get; set; }
        public int TotalInfluenciadoras { get; set; }
        public int TotalDenuncias { get; set; }
        public int TotalHashTags { get; set; }
        public int TotalInteracoesDiarias { get; set; }
        public decimal TotalVendas { get; set; }
        public decimal TotalRepassesPendentes { get; set; }
        public decimal TotalSaldoMelhorEnvio { get; set; }

    }
}
