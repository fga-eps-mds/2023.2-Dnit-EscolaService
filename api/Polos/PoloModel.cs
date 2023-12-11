using api.Municipios;

namespace api.Polos
{
    public class PoloModel
    {
        public int Id { get; set; }
        
        public string Endereco { get; set; }  
    
        public string Cep { get; set; }
    
        public string Latitude { get; set; }
    
        public string Nome { get; set; }
    
        public MunicipioModel Municipio { get; set; }
    
        public string Longitude { get; set; }
        public UfModel? Uf { get; set; }
    }
}
