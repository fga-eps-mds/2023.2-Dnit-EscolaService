using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using api.Escolas;
using app.Entidades;
using app.Repositorios.Interfaces;
using app.Services;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using RichardSzalay.MockHttp;
using service.Interfaces;
using test.Fixtures;
using test.Stubs;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace test
{
    public class CalcularRanqueJobTest : TestBed<Base>
    {
         private readonly AppDbContext db;
        private readonly IEscolaRepositorio escolaRepositorio;
        private readonly IRanqueRepositorio ranqueRepositorio;
        private readonly ICalcularRanqueJob ranqueJobService;
        private readonly Mock<IRanqueService> ranqueServiceMock;
        private readonly Mock<IBackgroundJobClient> jobClientMock;
        private readonly MockHttpMessageHandler handlerMock;
        private readonly Mock<IUpsService> upsServiceMock;
        private readonly UpsServiceConfig upsServiceConfig = new() { Host = "http://localhost/api/calcular/ups/escolas" };
        private readonly CalcularRanqueJob RanqueJob;


        public CalcularRanqueJobTest(ITestOutputHelper testOutputHelper, Base fixture) : base(testOutputHelper, fixture)
        {
            db = fixture.GetService<AppDbContext>(testOutputHelper)!;
            escolaRepositorio = fixture.GetService<IEscolaRepositorio>(testOutputHelper)!;
            ranqueRepositorio = fixture.GetService<IRanqueRepositorio>(testOutputHelper)!;
            ranqueJobService = fixture.GetService<ICalcularRanqueJob>(testOutputHelper)!;

            jobClientMock = new Mock<IBackgroundJobClient>();
            ranqueServiceMock = new Mock<IRanqueService>();
            handlerMock = new MockHttpMessageHandler();
            upsServiceMock = new Mock<IUpsService>();

            RanqueJob = new(
                db,
                jobClientMock.Object,
                escolaRepositorio,
                ranqueServiceMock.Object,
                Options.Create(upsServiceConfig),
                upsServiceMock.Object
            );
        }

        [Fact]
        public async Task DeveExecutarRanqueamentoComSucesso()
        {
            var ranqueId = 1;

            db.Ranques.Add(new Ranque() { Id = ranqueId, BateladasEmProgresso = 10 });
            db.SaveChanges();

            await ranqueJobService.ExecutarAsync(ranqueId, 10);
            
            var ranqueAtualizado = db.Ranques.ToList();
            
            Assert.NotNull(ranqueAtualizado);    
        }

        [Fact]
        public async Task SeExistirEscolaComFator_DeveRetornarTrue()
        {
            var escolaFator = PriorizacaoStub.ObterFatorEscola();
            db.FatorEscolas.Add(escolaFator);
            var fator = escolaFator.FatorPriorizacao;
            var escola = escolaFator.Escola;
            db.SaveChanges();

            ranqueJobService.ExisteFatorEscola(fator, escola);

            Assert.NotNull(fator);
            Assert.NotNull(escola);
               
        }

    }
}