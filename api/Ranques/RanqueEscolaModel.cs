using api.Municipios;
using api.Superintendencias;

namespace api.Ranques
{
    public class RanqueEscolaModel
    {
        public int RanqueId { get; set; }
        public int Pontuacao { get; set; }
        public int Posicao { get; set; }
        public EscolaRanqueInfo Escola { get; set; }
    }

    public class EscolaRanqueInfo
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public UfModel? Uf { get; set; }
        public List<EtapasdeEnsinoModel>? EtapaEnsino { get; set; }
        public MunicipioModel? Municipio { get; set; }
        public double DistanciaSuperintendencia { get; set; }
        public SuperintendenciaModel Superintendencia { get; set; }
        public bool TemSolicitacao { get; set; } = false;
    }
}