using api;
using System.ComponentModel.DataAnnotations;

namespace app.Entidades
{
    public class Superintendencia
    {
        [Key]
        public int Id { get; set; }
        
        public string Endereco { get; set; }  
        
        [MaxLength(10)]
        public string Cep { get; set; }
        
        [Required]
        public string Latitude { get; set; }
        
        [Required]
        public string Longitude { get; set; }
        
        [Required]
        public UF? Uf { get; set; }
    }
}

