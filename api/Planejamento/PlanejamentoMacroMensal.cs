using api.Ranques;

namespace api.Planejamento
{
    public class PlanejamentoMacroMensalModel
    {
        public Mes Mes { get; set; }
        public string Ano { get; set; }
        public int UPSTotal { get; set; }
        public int QuantidadeEscolasTotal { get; set; }
        public int QuantidadeAlunosTotal { get; set; }
        public List<DetalhesEscolaMensal> Escolas { get; set; }
        public List<DetalhesPorUF> DetalhesPorUF { get; set; }
    }

    public class PlanejamentoMacroMensalDTO
    {
        public Mes Mes { get; set; }
        public string Ano { get; set; }
        public List <Guid> Escolas { get; set; }
    }

    public class DetalhesPorUF
    {
        public UF? UF { get; set; }
        public int QuantidadeEscolasTotal { get; set; }
    }

    public class DetalhesEscolaMensal
    {
        public Guid Id { get; set; }
        public int UPS {get; set; }
        public string Nome { get; set; }
        public UF? UF { get; set; }
        public int QuantidadeAlunos { get; set; }
        public double DistanciaPolo { get; set; }
    }
}