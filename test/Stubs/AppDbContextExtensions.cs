
using app.Entidades;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace test.Stubs
{
    public static class AppDbContextExtensions
    {
        private static List<Municipio>? municipios;
        private static Mutex municipios_mutex = new Mutex();

        public static List<SolicitacaoAcao> PopulaSolicitacoes(this AppDbContext dbContext, int limite = 1)
        {
            var sols = new List<SolicitacaoAcao>();
            if (!dbContext.Municipios.Any())
            {
                dbContext.PopulaMunicipios(limite);
            }
            var municipios = dbContext.Municipios.Take(1).ToList();

            foreach (var sol in SolicitacaoAcaoStub.ListarSolicitacoes(municipios).Take(limite))
            {
                dbContext.Solicitacoes.Add(sol);
                sols.Add(sol);
            }
            dbContext.SaveChanges();
            return sols;
        }

        public static List<Escola> PopulaEscolas(this AppDbContext dbContext, int limite = 1, bool comEtapas = true)
        {
            dbContext.Clear();
            var escolas = new List<Escola>();

            if (!dbContext.Municipios.Any())
            {
                dbContext.PopulaMunicipios(limite);
            }

            var municipios = dbContext.Municipios.Take(1).ToList();

            foreach (var escola in EscolaStub.ListarEscolas(municipios, comEtapas).Take(limite))
            {
                dbContext.Add(escola);
                escolas.Add(escola);
            }
            dbContext.SaveChanges();
            return escolas;
        }

        public static List<Municipio> PopulaMunicipios(this AppDbContext dbContext, int limit)
        {
            dbContext.Clear();
            if (municipios != default && limit <= municipios.Count)
            {
                var resultado = municipios.Take(limit).ToList();
                dbContext.AddRange(resultado);
                dbContext.SaveChanges();
                return resultado;
            }
            municipios_mutex.WaitOne(int.MaxValue);
            var caminho = Path.Join("..", "..", "..", "Stubs", "municipios.csv");
            var municipiosDb = dbContext.PopulaMunicipiosPorArquivo(limit, caminho)!;
            municipios = municipiosDb.ToList();
            municipios_mutex.ReleaseMutex();
            return municipiosDb;
        }

        public static List<Superintendencia> PopulaSuperintendencias(this AppDbContext dbContext, int limit, int idStart = 1)
        {
            var superintendencias = SuperintendenciaStub.Listar(idStart).Take(limit).ToList();
            dbContext.AddRange(superintendencias);
            dbContext.SaveChanges();
            return superintendencias;
        }

        public static void Clear(this AppDbContext dbContext)
        {
            dbContext.RemoveRange(dbContext.Escolas);
            dbContext.RemoveRange(dbContext.Solicitacoes);
            dbContext.RemoveRange(dbContext.EscolaEtapaEnsino);
            dbContext.RemoveRange(dbContext.Municipios);
            dbContext.RemoveRange(dbContext.EscolaRanques);
            dbContext.RemoveRange(dbContext.Ranques);
            dbContext.RemoveRange(dbContext.Superintendencias);
            dbContext.SaveChanges();
        }
    }
}