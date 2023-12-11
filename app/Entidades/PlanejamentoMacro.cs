using System.ComponentModel.DataAnnotations;
using api;

namespace app.Entidades
{
    public class PlanejamentoMacro
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(200)]
        public string Nome { get; set; }

        [Required]
        public string Responsavel { get; set; }

        [Required]
        public Mes MesInicio { get; set; }

        [Required]
        public Mes MesFim { get; set; }
        
        [Required]
        public string AnoInicio { get; set; }

        [Required]
        public string AnoFim { get; set; }

        [Required]
        public int QuantidadeAcoes { get; set; }

        public List<PlanejamentoMacroEscola> Escolas { get; set; }
    }
}