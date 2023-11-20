using api;
using app.Entidades;
using System.Collections.Generic;
using System.Linq;

namespace test.Stubs
{
    public static class SuperintendenciaStub
    {
        public static IEnumerable<Superintendencia> Listar(int idInicio = 1)
        {
            while (true)
            {
                var superintendencias = new Superintendencia
                {
                    Id = idInicio++,
                    Cep = $"7215436{Random.Shared.Next() % 10}",
                    Endereco = $"Endereço Teste {Random.Shared.Next()}",
                    Latitude = Random.Shared.NextDouble().ToString().Truncate(12),
                    Longitude = Random.Shared.NextDouble().ToString().Truncate(12),
                    Uf = Enum.GetValues<UF>().TakeRandom().FirstOrDefault(),
                };
                yield return superintendencias;
            }
        }
    }
}
