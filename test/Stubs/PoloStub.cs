using api;
using api.Escolas;
using api.Polos;
using app.Entidades;
using System.Collections.Generic;
using System.Linq;

namespace test.Stubs
{
    public static class PoloStub
    {
        public static IEnumerable<Polo> Listar(IEnumerable<Municipio> municipios, int idInicio = 1)
        {
            while (true)
            {
                var polos = new Polo
                {
                    Id = idInicio++,
                    Nome = $"Polo DNIT {Random.Shared.Next()}",
                    Cep = $"7215436{Random.Shared.Next() % 10}",
                    Endereco = $"Endereço Teste {Random.Shared.Next()}",
                    Municipio = municipios.TakeRandom().First(),
                    Latitude = Random.Shared.NextDouble().ToString().Truncate(12),
                    Longitude = Random.Shared.NextDouble().ToString().Truncate(12),
                    Uf = Enum.GetValues<UF>().TakeRandom().FirstOrDefault(),
                };
                yield return polos;
            }
        }

        public static IEnumerable<CadastroPoloDTO> ListarPolosDto(IEnumerable<Municipio> municipios)
        {
            while (true)
            {
                var polos = new CadastroPoloDTO
                {
                    Nome = $"Polo DNIT {Random.Shared.Next()}",
                    Cep = $"7215436{Random.Shared.Next() % 10}",
                    Endereco = $"Endereço Teste {Random.Shared.Next()}",
                    MunicipioId = municipios.TakeRandom().First().Id,
                    Latitude = Random.Shared.NextDouble().ToString().Truncate(12),
                    Longitude = Random.Shared.NextDouble().ToString().Truncate(12),
                    IdUf = (int)Enum.GetValues<UF>().TakeRandom().FirstOrDefault(),
                };
                yield return polos;
            }
        }
    }
}
