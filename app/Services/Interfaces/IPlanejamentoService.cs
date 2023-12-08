using System.Numerics;
using api.Planejamento;
using app.Entidades;

namespace service.Interfaces
{
    public interface IPlanejamentoService
    {
        // Definir os métodos a serem implementados
        Task<PlanejamentoMacro> GerarRecomendacaoDePlanejamento(PlanejamentoMacroDTO planejamento);
        public PlanejamentoMacro CriarPlanejamentoMacro(PlanejamentoMacro planejamento);
        Task<PlanejamentoMacro> EditarPlanejamentoMacro(PlanejamentoMacro planejamento, PlanejamentoMacroDTO dto); 
        Task<PlanejamentoMacro> ObterPlanejamentoMacroAsync(Guid id);
        Task ExcluirPlanejamentoMacro(Guid id);
    }
} 