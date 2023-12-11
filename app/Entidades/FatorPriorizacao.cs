using System.ComponentModel.DataAnnotations;

namespace app.Entidades
{
    public class FatorPriorizacao
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(100)]
        public string Nome { get; set; }

        [Required]
        public int Peso { get; set; }

        [Required]
        public bool Ativo { get; set; }

        [Required]
        public bool Primario { get; set; }

        public DateTime? DeleteTime { get; set; }

        public List<FatorCondicao> FatorCondicoes { get; set; }
    }
}
