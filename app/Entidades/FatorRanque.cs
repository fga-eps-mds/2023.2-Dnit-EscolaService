using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app.Entidades
{
    public class FatorRanque
    {
        [Key, Column(Order = 1)]
        public Guid FatorPriorizacaoId { get; set; }
        public FatorPriorizacao FatorPriorizacao { get; set; }
        [Key, Column(Order = 2)]
        public int RanqueId { get; set; }
        public Ranque Ranque { get; set; }
    }
}