using api.Escolas;
using api.Escolas;
using app.Controllers;
using app.Entidades;
using System.Threading.Tasks;
using test.Fixtures;
using Xunit.Abstractions;

namespace test
{
    public class RanqueControllerTest : AuthTest
    {
        private readonly RanqueController controller;
        private readonly AppDbContext dbContext;

        public RanqueControllerTest(ITestOutputHelper testOutputHelper, Base fixture) : base(testOutputHelper, fixture)
        {
            dbContext = fixture.GetService<AppDbContext>(testOutputHelper)!;
            controller = fixture.GetService<RanqueController>(testOutputHelper)!;
            AutenticarUsuario(controller);
        }
    }
}
