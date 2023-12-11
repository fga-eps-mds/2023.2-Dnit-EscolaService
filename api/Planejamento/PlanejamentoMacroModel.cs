namespace api.Planejamento
{
    public class PlanejamentoMacroModel
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Responsavel { get; set; }
        public Mes MesInicio { get; set; }
        public Mes MesFim { get; set; }    
        public string AnoInicio { get; set; }
        public string AnoFim { get; set; }
        public int QuantidadeAcoes { get; set; }
    }

    public class PlanejamentoMacroDetalhadoModel : PlanejamentoMacroModel
    {
        public List<PlanejamentoMacroMensalModel> PlanejamentoMacroMensal { get; set; }
    }
}