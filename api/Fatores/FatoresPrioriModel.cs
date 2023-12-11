using System.ComponentModel.DataAnnotations;

namespace api.Fatores
{
    public class FatorPrioriModel
    { 
        public Guid Id {get; set;}
        
        public string Nome {get; set;}
        
        public int Peso {get;set;}
        
        public bool Ativo {get;set;}

        public bool Primario {get;set;}

        public List<FatorCondicaoModel> FatorCondicoes {get;set;}
    }
}
