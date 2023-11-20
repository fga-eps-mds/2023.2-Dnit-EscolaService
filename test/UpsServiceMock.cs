using System.Collections.Generic;
using System.Threading.Tasks;
using app.Entidades;
using service.Interfaces;

namespace test
{
    public class UpsServiceMock : IUpsService
    {
        public Task<List<int>> CalcularUpsEscolasAsync(List<Escola> escolas, double raioKm, int desdeAno, int expiracaoMinutos)
        {
            throw new NotImplementedException();
        }
    }
}