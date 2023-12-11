using api.Municipios;
using api.Polos;
using api.Escolas;

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
        public double DistanciaPolo { get; set; }
        public PoloModel Polo { get; set; }
        public bool TemSolicitacao { get; set; } = false;
        public int Ups { get; set; }
        public EscolaParaOtimizacao ParaOtimizacao()
        {   
            UF ufRetorno = (UF) (Uf == null ? 0 : Uf.Id);
            return new EscolaParaOtimizacao(Id, Ups, DistanciaPolo, ufRetorno);
        }
    }
}
