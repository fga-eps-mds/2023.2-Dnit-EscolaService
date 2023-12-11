using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using api;

namespace app.Entidades
{
    public class CondicaoValor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public Guid FatorCondicaoId { get; set; }

        [Required, MaxLength(30)]
        public string Valor { get; set; }
    }
}
