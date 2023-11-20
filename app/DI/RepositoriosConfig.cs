using app.Entidades;
using app.Repositorios;
using app.Repositorios.Interfaces;

namespace app.DI
{
    public static class RepositoriosConfig
    {
        public static void AddConfigRepositorios(this IServiceCollection services)
        {
            services.AddScoped<IEscolaRepositorio, EscolaRepositorio>();
            services.AddScoped<IMunicipioRepositorio, MunicipioRepositorio>();
            services.AddScoped<ISuperintendenciaRepositorio, SuperIntendenciaRepositorio>();
        }
    }
}
