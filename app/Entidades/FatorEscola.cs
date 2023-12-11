using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app.Entidades
{
    public class FatorEscola
    {
        [Required]
        public Guid FatorPriorizacaoId { get; set; }

        [Key, Column(Order = 1)]
        public FatorPriorizacao FatorPriorizacao { get; set; }

        [Required]
        public Guid EscolaId { get; set; }

        [Key, Column(Order = 2)]
        public Escola Escola { get; set; }

        [Required]
        public int Valor { get; set; }

    }
}
