using api;
using api.Escolas;
using app.Entidades;

namespace app.Repositorios.Interfaces
{
    public interface ISolicitacaoAcaoRepositorio
    {
        public Task<SolicitacaoAcao> CriarOuAtualizar(SolicitacaoAcaoData solicitacao, Escola? escolaCadastrada, SolicitacaoAcao? solicitacaoAcaoExistente);
        public Task<ListaPaginada<SolicitacaoAcao>> ObterSolicitacoesAsync(PesquisaSolicitacaoFiltro filtro);
        public Task<SolicitacaoAcao?> ObterPorCodigoInepdAsync(int escolaCodigoInep);
    }
}
