using System.ComponentModel.DataAnnotations;

namespace app.Entidades
{
    public class CustoLogistico
    {
        [Key]
        public int Custo { get; set; }

        [Required]
        public int Valor { get; set; }

        [Required]
        public int RaioMin { get; set; }
        public int? RaioMax { get; set; }
    }
}
