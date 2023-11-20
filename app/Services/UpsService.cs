using System.Text.Json;
using api;
using app.Entidades;
using Microsoft.Extensions.Options;
using service.Interfaces;

namespace app.Services
{
    // Segue abordagem Typed Client
    // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-8.0#consumption-patterns
    public class UpsService : IUpsService
    {
        private readonly HttpClient httpClient;
        private readonly UpsServiceConfig config;
        private readonly string Endpoint = "api/calcular/ups/escolas";

        public UpsService(HttpClient httpClient, IOptions<UpsServiceConfig> config)
        {
            this.httpClient = httpClient;
            this.config = config.Value;
            this.httpClient.DefaultRequestHeaders.Add("Authorization", config.Value.ApiKey);
        }
        public async Task<List<int>> CalcularUpsEscolasAsync(List<Escola> escolas, double raioKm, int desdeAno, int expiracaoMinutos)
        {
            var localizacoes = escolas.Select(
                e => new LocalizacaoEscola
                {
                    // As latitudes no banco são armazenadas com vírgula em vez de ponto.
                    Latitude = double.Parse(e.Latitude.Replace(',', '.')),
                    Longitude = double.Parse(e.Longitude.Replace(',', '.')),
                });

            httpClient.Timeout = expiracaoMinutos <= 0
                ? new TimeSpan(0, 0, 0, 0, -1) // tempo infinito para expiração
                : TimeSpan.FromMinutes(expiracaoMinutos);

            var conteudo = JsonContent.Create(localizacoes);

            var resposta = await httpClient.PostAsync(
                config.Host + Endpoint + $"?desde={desdeAno}&raiokm={raioKm}", conteudo);

            resposta.EnsureSuccessStatusCode();

            try
            {
                List<int> upss = await resposta.Content.ReadFromJsonAsync<List<int>>()
                    ?? throw new ApiException(ErrorCodes.FormatoJsonNaoReconhecido);
                return upss;
            }
            catch (JsonException e)
            {
                if (e.Message.ToLower().Contains("the json value could not be converted"))
                    throw new ApiException(ErrorCodes.FormatoJsonNaoReconhecido);

                throw e;
            }
        }
    }
}