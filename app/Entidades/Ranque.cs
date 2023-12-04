using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app.Entidades
{
    public class Ranque
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        // Mudar pra BateladasRestantes
        // Quando chega em 0, o processamento do ranking terminou
        public int BateladasEmProgresso { get; set; }
        
        [NotMapped]
        public DateTimeOffset DataInicio { get; set; }

        public DateTime DataInicioUtc
        {
            get => DataInicio.UtcDateTime;
            set => DataInicio = new DateTimeOffset(value, TimeSpan.Zero);
        }
        
        [NotMapped]
        public DateTimeOffset? DataFim { get; set; } = null;

        public DateTime? DataFimUtc
        {
            get => DataFim?.UtcDateTime;
            set => DataFim = value != null ? new DateTimeOffset(value.Value, TimeSpan.Zero) : null;
        }

        [MaxLength(100)]
        public string? Descricao { get; set; }

        public List<EscolaRanque> EscolaRanques { get; set; }
    }
}