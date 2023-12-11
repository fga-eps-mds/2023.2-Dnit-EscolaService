using System.Linq;
using System.Threading.Tasks;
using app.Controllers;
using app.Entidades;
using app.Repositorios.Interfaces;
using test.Fixtures;
using test.Stubs;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace test
{
	public class PriorizacaoRepositorioTest : TestBed<Base>, IDisposable
    {
		IPriorizacaoRepositorio priorizacaoRepositorio;
		AppDbContext db;

		public PriorizacaoRepositorioTest(ITestOutputHelper testOutputHelper, Base fixture) : base(testOutputHelper, fixture)
        {
			priorizacaoRepositorio = fixture.GetService<IPriorizacaoRepositorio>(testOutputHelper)!; ;
			db = fixture.GetService<AppDbContext>(testOutputHelper)!;
			db.PopulaCondicao(4);
		}

		[Fact]
		public async Task VisualizarFatorComCondicaoId_QuandoColocarId_DeveRetornarFator()
		{
			var fatorCondicaoDb = db.FatorCondicoes.First();
			var fatorCondicao = await priorizacaoRepositorio.ObterFatorCondiPorIdAsync(fatorCondicaoDb.Id);

			Assert.NotNull(fatorCondicaoDb);
			Assert.NotNull(fatorCondicao);
		}
	}
}