using System.ComponentModel.DataAnnotations;
using api;

namespace app.Entidades
{
    public class FatorCondicao
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(30)]
        public PropriedadeCondicao Propriedade { get; set; }

        [Required]
        public OperacaoCondicao Operador { get; set; }

        [Required]
        public List<CondicaoValor> Valores { get; set; }

        [Required]
        public Guid FatorPriorizacaoId { get; set; }
    }
}
