
using api.Municipios;
using api.Polos;
// TODO: consertar namespace inconsistente
namespace api.Escolas
{

    public class DetalhesEscolaRanqueModel
    {
        public RanqueInfo RanqueInfo { get; set; }

        public Guid Id { get; set; }
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public string Cep { get; set; }
        public string Endereco { get; set; }
        public string? Longitude { get; set; }
        public string? Latitude { get; set; }
        public int? TotalAlunos { get; set; }
        public string Telefone { get; set; }
        public int TotalDocentes { get; set; }
        public UfModel? Uf { get; set; }
        public MunicipioModel? Municipio { get; set; }
        public RedeModel? Rede { get; set; }
        public PorteModel? Porte { get; set; }
        public LocalizacaoModel? Localizacao { get; set; }
        public SituacaoModel? Situacao { get; set; }
        public List<EtapasdeEnsinoModel>? EtapasEnsino { get; set; }
        public double DistanciaPolo { get; set; }
        public PoloModel Polo { get; set; }
        public bool TemSolicitacao { get; set; } = false;
    }

    public class RanqueInfo
    {
        public int RanqueId { get; set; }
        public int Pontuacao { get; set; }
        public int Posicao { get; set; }
        public FatorModel[] Fatores { get; set; }
    }

    public class FatorModel
    {
        public string Nome { get; set; }
        public int Peso { get; set; }
        public int Valor { get; set; }
    }
}
