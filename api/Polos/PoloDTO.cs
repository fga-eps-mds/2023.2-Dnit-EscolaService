namespace api.Polos;

public class PoloDTO
{
    public int Id { get; set; }
        
    public string Endereco { get; set; }  
    
    public string Cep { get; set; }
    
    public string Latitude { get; set; }
    
    public string Nome { get; set; }
    
    public string NomeMunicipio { get; set; }
    
    public string Longitude { get; set; }
    
    public UF Uf { get; set; }
}
