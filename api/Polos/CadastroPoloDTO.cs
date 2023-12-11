namespace api.Polos;

public class CadastroPoloDTO
{
    public string Endereco { get; set; }  
    
    public string Cep { get; set; }
    
    public string Latitude { get; set; }
    
    public string Nome { get; set; }
    
    public int MunicipioId { get; set; }
    
    public string Longitude { get; set; }
    
    public int IdUf { get; set; }
}
