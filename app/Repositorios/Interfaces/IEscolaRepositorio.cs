using System.Linq.Expressions;
using api;
using api.Escolas;
using app.Entidades;

namespace app.Repositorios.Interfaces
{
    public interface IEscolaRepositorio
    {
        Task<Escola> ObterPorIdAsync(Guid id, bool incluirEtapas = false, bool incluirMunicipio = false);
        Task<List<Escola>> ListarAsync(Expression<Func<Escola, bool>>? filter = null);
        Escola Criar(CadastroEscolaData escolaData, Municipio municipio, double distanciaPolo = 0, Polo? polo = null);
        Escola Criar(EscolaModel escola, double distanciaPolo = 0, Polo? polo = null);
        Task<ListaPaginada<Escola>> ListarPaginadaAsync(PesquisaEscolaFiltro filtro);
        Task<Escola?> ObterPorCodigoAsync(int codigo, bool incluirEtapas = false, bool incluirMunicipio = false);
        EscolaEtapaEnsino AdicionarEtapaEnsino(Escola escola, EtapaEnsino etapa);
    }
}
