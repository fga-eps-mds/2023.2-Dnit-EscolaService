using api;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app.Entidades
{
    public class EscolaRanque
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
   
        public Guid EscolaId { get; set; }

        public Escola Escola { get; set;}

        public int RanqueId { get; set; }

        public Ranque Ranque { get; set; }
        
        public int Pontuacao { get; set; }

        public int Posicao { get; set; }
    }
}