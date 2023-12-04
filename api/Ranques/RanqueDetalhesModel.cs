using api.Escolas;
using api.Municipios;
using api.Superintendencias;

namespace api.Ranques
{

    public class RanqueDetalhesModel
    {
        public int Id { get ; set; }
        public int NumEscolas { get ; set; } 
        public DateTime Data { get ; set; } 
        public string? Descricao { get ; set; } 
        public FatorModel[] Fatores { get ; set; } 
    }
}

