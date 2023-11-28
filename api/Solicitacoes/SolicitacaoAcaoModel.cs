using api.Escolas;

namespace api.Solicitacoes
{
    public class SolicitacaoAcaoModel
    {
        public Guid Id { get; set; }
        public Guid? EscolaId { get; set; }
        public EscolaModel? Escola { get; set; }
        public bool EscolaJaCadastrada { get; set; }
        public string NomeSolicitante { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Observacoes { get; set; }
        public DateTime DataRealizadaUtc { get; set; }
    }
}