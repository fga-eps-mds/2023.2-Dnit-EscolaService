using System.Linq.Expressions;
using api;
using api.Planejamento;
using app.Entidades;

namespace app.Repositorios.Interfaces
{
    public interface IPlanejamentoRepositorio
    {
        //Definir os metodos a serem implementados
        void ExcluirPlanejamentoMacro(PlanejamentoMacro pm);
        void ExcluirPlanejamentoMacroEscola(PlanejamentoMacroEscola pm);
        Task<PlanejamentoMacro> ObterPlanejamentoMacroAsync(Guid id);
        PlanejamentoMacro RegistrarPlanejamentoMacro(PlanejamentoMacro pm);
        Task<ListaPaginada<PlanejamentoMacro>> ListarPaginadaAsync(PesquisaPlanejamentoFiltro filtro);
        Task<List<PlanejamentoMacro>> ListarAsync(Expression<Func<PlanejamentoMacro, bool>>? filter = null);  
        //Correcao
        void RegistrarPlanejamentoMacroMensal(PlanejamentoMacroEscola pme);
    }
}