using System.ComponentModel.DataAnnotations;
using api.Fatores;

namespace api.Fatores
{
    public class FatorCondicaoModel
    {
        public Guid Id { get; set; }
        public PropriedadeCondicao Propriedade { get; set; }
        public OperacaoCondicao Operador { get; set; }
        public List<string> Valores { get; set; }
        public Guid? FatorPriorizacaoId { get; set; }
    }
}
