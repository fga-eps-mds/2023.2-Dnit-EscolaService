using api;
using api.Escolas;
using api.Solicitacoes;
using app.Entidades;

namespace service.Interfaces
{
    public interface ISolicitacaoAcaoService
    {
        public void EnviarSolicitacaoAcao(SolicitacaoAcaoData solicitacaoAcaoDTO);
        public void EnviarEmail(string emailDestinatario, string assunto, string corpo);
        public Task<IEnumerable<EscolaInep>> ObterEscolas(int municipio);
        public Task CriarOuAtualizar(SolicitacaoAcaoData solicitacao);
        public Task<ListaPaginada<SolicitacaoAcaoModel>> ObterSolicitacoesAsync(PesquisaSolicitacaoFiltro filtro);
    }
}
