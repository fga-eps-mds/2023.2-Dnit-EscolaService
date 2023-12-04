using api;
using api.Escolas;
using app.Entidades;

namespace app.Repositorios.Interfaces
{
    public interface IRanqueRepositorio
    {
        Task<ListaPaginada<EscolaRanque>> ListarEscolasPaginadaAsync(int ranqueId, PesquisaEscolaFiltro filtro);
        Task<List<EscolaRanque>> ListarEscolasAsync(int ranqueId);
        Task<Ranque?> ObterUltimoRanqueAsync();
        Task<Ranque?> ObterRanqueEmProcessamentoAsync();
        Task<Ranque?> ObterPorIdAsync(int id);
        Task<EscolaRanque?> ObterEscolaRanquePorIdAsync(Guid escolaId, int ranqueId);
        Task<ListaPaginada<Ranque>> ListarRanques( PesquisaEscolaFiltro filtro);
        Task<List<EscolaRanque>> ListarEscolaRanquesAsync(int ranqueId);
    }
}