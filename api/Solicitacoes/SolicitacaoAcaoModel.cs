using api.Escolas;
using api.Municipios;

namespace api.Solicitacoes
{
    public class SolicitacaoAcaoModel
    {
        public Guid Id { get; set; }
        public EscolaCorretaModel? Escola { get; set; }
        public int CodigoEscola { get; set; }
        public string Nome { get; set; }
        public string NomeSolicitante { get; set; }
        public int QuantidadeAlunos { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Observacoes { get; set; }
        public DateTime DataRealizadaUtc { get; set; }
        public UF Uf { get; set; }
        public MunicipioModel Municipio { get; set; }
        public string Vinculo { get; set; }
    }
}