namespace api.Planejamento
{
    public class PlanejamentoMacroDTO
    {
        public string Nome { get; set; }
        public string Responsavel { get; set; }
        public Mes MesInicio { get; set; }
        public Mes MesFim { get; set; }    
        public string AnoInicio { get; set; }
        public string AnoFim { get; set; }
        public int QuantidadeAcoes { get; set; }
    }

    public class PlanejamentoMacroDetalhadoDTO
    {
        public string Nome { get; set; }
        public List<PlanejamentoMacroMensalDTO> PlanejamentoMacroMensal { get; set; }
    }
}