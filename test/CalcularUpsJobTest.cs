using app.Entidades;
using app.Repositorios.Interfaces;
using app.Services;
using Hangfire;
using Microsoft.Extensions.Options;
using Moq;
using RichardSzalay.MockHttp;
using service.Interfaces;
using test.Fixtures;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.Abstracts;
using test.Stubs;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace test
{
    public class CalcularUpsJobTest : TestBed<Base>
    {
        private readonly AppDbContext db;
        private readonly IEscolaRepositorio escolaRepositorio;
        private readonly IRanqueRepositorio ranqueRepositorio;
        private readonly Mock<IRanqueService> ranqueServiceMock;
        private readonly Mock<IBackgroundJobClient> jobClientMock;
        private readonly MockHttpMessageHandler handlerMock;
        private readonly Mock<IUpsService> upsServiceMock;
        private readonly UpsServiceConfig upsServiceConfig = new() { Host = "http://localhost/" };
        private CalcularUpsJob upsJob;


        public CalcularUpsJobTest(ITestOutputHelper testOutputHelper, Base fixture) : base(testOutputHelper, fixture)
        {
            db = fixture.GetService<AppDbContext>(testOutputHelper)!;
            escolaRepositorio = fixture.GetService<IEscolaRepositorio>(testOutputHelper)!;
            ranqueRepositorio = fixture.GetService<IRanqueRepositorio>(testOutputHelper)!;

            jobClientMock = new Mock<IBackgroundJobClient>();
            ranqueServiceMock = new Mock<IRanqueService>();
            handlerMock = new MockHttpMessageHandler();
            upsServiceMock = new Mock<IUpsService>();

            upsJob = new(
                db,
                escolaRepositorio,
                ranqueRepositorio,
                ranqueServiceMock.Object,
                jobClientMock.Object,
                upsServiceMock.Object
            );
        }

        [Fact]
        public async Task ExecutarAsync_QuandoTudoCerto_DefineUpsDeAcordoComARespostaDoMicroservicoUps()
        {
            var escolas = db.PopulaEscolas(3);
            upsServiceMock
                .Setup(x => x.CalcularUpsEscolasAsync(It.IsAny<List<Escola>>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new List<int> { 1, 2, 3 });

            await upsJob.ExecutarAsync(new(), 1, 1);

            escolas = db.Escolas.OrderBy(e => e.Nome).ToList();
            Assert.Equal(1, escolas[0].Ups);
            Assert.Equal(2, escolas[1].Ups);
            Assert.Equal(3, escolas[2].Ups);
        }

        [Fact]
        public async Task ExecutarAsync_QuandoTudoCerto_CriaEscolaRanques()
        {
            db.PopulaEscolas(6);
            upsServiceMock
                .Setup(x => x.CalcularUpsEscolasAsync(It.IsAny<List<Escola>>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new List<int> { 2, 2, 2, 2, 3, 3 });


            await upsJob.ExecutarAsync(new(), 1, 1);

            var ersCount = db.EscolaRanques.Count();
            Assert.Equal(6, ersCount);
        }

        [Fact]
        public async Task FinalizarCalcularUpsJob_QuandoCalculoEmProgresso_DecrementaBateladasEmProgresso()
        {
            var ranqueId = 1;
            db.Ranques.Add(new Ranque() { Id = ranqueId, BateladasEmProgresso = 10 });
            db.SaveChanges();

            await upsJob.FinalizarCalcularUpsJob(ranqueId);

            var ranque = db.Ranques.FirstOrDefault();

            Assert.Equal(9, ranque!.BateladasEmProgresso);
        }

        [Fact]
        public async Task FinalizarCalcularUpsJob_QuandoUltimabatelada_InvocaConcluirRanqueamentoAsync()
        {
            var ranqueId = 1;
            db.Ranques.Add(new Ranque() { Id = ranqueId, BateladasEmProgresso = 1 });
            db.SaveChanges();

            await upsJob.FinalizarCalcularUpsJob(ranqueId);

            var ranque = db.Ranques.FirstOrDefault();
            Assert.Equal(0, ranque!.BateladasEmProgresso);
            ranqueServiceMock
                .Verify(x => x.ConcluirRanqueamentoAsync(ranque), Times.AtLeastOnce());
        }
    }
}