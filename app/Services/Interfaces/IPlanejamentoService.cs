using System.Numerics;
using api;
using api.Planejamento;
using app.Entidades;

namespace service.Interfaces
{
    public interface IPlanejamentoService
    {
        // Definir os métodos a serem implementados
        Task<PlanejamentoMacro> GerarRecomendacaoDePlanejamento(PlanejamentoMacroDTO planejamento);
        public PlanejamentoMacro CriarPlanejamentoMacro(PlanejamentoMacro planejamento);
        Task<PlanejamentoMacro> EditarPlanejamentoMacro(Guid id, string nome, List<PlanejamentoMacroMensalDTO> planejamentoMacroMensal); 
        Task<PlanejamentoMacro> ObterPlanejamentoMacroAsync(Guid id);
        Task ExcluirPlanejamentoMacro(Guid id);
        Task<ListaPaginada<PlanejamentoMacroDetalhadoModel>> ListarPaginadaAsync(PesquisaPlanejamentoFiltro filtro);
    }
} 