using System.ComponentModel.DataAnnotations;
using api;

namespace app.Entidades
{
    public class PlanejamentoMacroEscola
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Mes Mes { get; set; }

        [Required]
        public string Ano { get; set; }

        [Required]
        public Guid PlanejamentoMacroId { get; set; }

        public PlanejamentoMacro PlanejamentoMacro { get; set; }

        [Required]
        public Guid EscolaId { get; set; }

        public Escola Escola { get; set; }
    }
}