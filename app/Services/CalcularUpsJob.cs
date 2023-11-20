using api;
using api.Escolas;
using app.Entidades;
using app.Repositorios.Interfaces;
using Hangfire;
using Microsoft.Extensions.Options;
using service.Interfaces;

namespace app.Services
{
    public class CalcularUpsJob : ICalcularUpsJob
    {
        private readonly IEscolaRepositorio escolaRepositorio;
        private readonly IRanqueRepositorio ranqueRepositorio;
        private readonly IBackgroundJobClient jobClient;
        private readonly IRanqueService ranqueService;
        private readonly AppDbContext dbContext;
        private readonly IUpsService upsService;

        public CalcularUpsJob(
            AppDbContext dbContext,
            IEscolaRepositorio escolaRepositorio,
            IRanqueRepositorio ranqueRepositorio,
            IRanqueService ranqueService,
            IBackgroundJobClient jobClient,
            IUpsService upsService
        )
        {
            this.dbContext = dbContext;
            this.escolaRepositorio = escolaRepositorio;
            this.ranqueRepositorio = ranqueRepositorio;
            this.jobClient = jobClient;
            this.ranqueService = ranqueService;
            this.upsService = upsService;
        }

        [MaximumConcurrentExecutions(3, timeoutInSeconds: 20 * 60)]
        public async Task ExecutarAsync(PesquisaEscolaFiltro filtro, int novoRanqueId, int timeoutMinutos)
        {
            var raio = 2.0D;
            var desde = 2019;

            var lista = await escolaRepositorio.ListarPaginadaAsync(filtro);
            
            var upss = await upsService.CalcularUpsEscolasAsync(lista.Items, raio, desde, timeoutMinutos);
                // ?? throw new ApiException(ErrorCodes.Unknown);

            var ranqueEscolas = new EscolaRanque[lista.Items.Count];

            for (int i = 0; i < lista.Items.Count; i++)
            {
                lista.Items[i].Ups = upss[i];
                ranqueEscolas[i] = new()
                {
                    Pontuacao = upss[i],
                    RanqueId = novoRanqueId,
                    EscolaId = lista.Items[i].Id,
                };
            }

            await dbContext.EscolaRanques.AddRangeAsync(ranqueEscolas);
            await dbContext.SaveChangesAsync();
            jobClient.Enqueue<ICalcularUpsJob>(
                job => job.FinalizarCalcularUpsJob(novoRanqueId));
        }

        // Não pode ser mais que 1. Essa limitação serve como um Mutex
        [MaximumConcurrentExecutions(1)]
        public async Task FinalizarCalcularUpsJob(int ranqueId)
        {
            var ranqueEmProgresso = (await ranqueRepositorio.ObterPorIdAsync(ranqueId))!;
            if (ranqueEmProgresso.DataFim != null || ranqueEmProgresso.BateladasEmProgresso == 0)
                return;

            ranqueEmProgresso.BateladasEmProgresso--;

            if (ranqueEmProgresso.BateladasEmProgresso == 0)
                await ranqueService.ConcluirRanqueamentoAsync(ranqueEmProgresso);

            await dbContext.SaveChangesAsync();
        }
    }
}