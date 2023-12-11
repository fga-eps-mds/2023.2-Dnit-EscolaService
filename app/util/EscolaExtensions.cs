using System.Globalization;
using app.Entidades;

namespace app.util;

public static class EscolaExtensions
{
    public static double? CalcularDistanciaParaPolo(this Escola escola, Polo polo)
    {

        bool escolaVazia = String.IsNullOrEmpty(escola.Latitude) || String.IsNullOrEmpty(escola.Longitude);
        bool poloVazio = String.IsNullOrEmpty(polo.Latitude) || String.IsNullOrEmpty(polo.Longitude);

        if (escolaVazia || poloVazio)
            return null;
        
        CultureInfo c = new CultureInfo("pt-BR"); 
        (double elat, double elon) = (double.Parse(escola.Latitude, c), double.Parse(escola.Longitude, c));
        (double plat, double plon) = (double.Parse(polo.Latitude, c), double.Parse(polo.Longitude, c));

        return GeoCalc.CalcularDistancia(elat, elon, plat, plon);
    }

    public static (Polo?, double?) CalcularPoloMaisProximo(this Escola escola, IEnumerable<Polo> polos)
    {
        var poloMaisProximo = polos.Select(p => new
            {
                Polo = p,
                Distancia = escola.CalcularDistanciaParaPolo(p),
            }).Where(o => o.Distancia.HasValue)
            .MinBy(o => o.Distancia.GetValueOrDefault(double.MaxValue));

        return (poloMaisProximo?.Polo, poloMaisProximo?.Distancia);
    }

    public static void SubstituirSeMaisProximo(this Escola escola, Polo polo)
    {
        var d = escola.CalcularDistanciaParaPolo(polo);
        if (!d.HasValue || d >= escola.DistanciaPolo) return;
        escola.Polo = polo;
        escola.DistanciaPolo = d.Value;
    }
}
