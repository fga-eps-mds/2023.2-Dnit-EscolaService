namespace app.util;

public static class GeoCalc
{
    private const double raioTerraEmKm = 6371.0;
    public static double ConverterParaRadianos(double grau)
    {
        return grau * Math.PI / 180.0;
    }

    public static double CalcularDistancia(double lat1, double long1, double lat2, double long2)
    {
        var diferencaLatitude = ConverterParaRadianos(lat2 - lat1);
        var diferencaLongitude = ConverterParaRadianos(long2 - long1);

        var primeiraParteFormula = Math.Sin(diferencaLatitude / 2) * Math.Sin(diferencaLatitude / 2) +
                                   Math.Cos(ConverterParaRadianos(lat1)) * Math.Cos(ConverterParaRadianos(lat2)) *
                                   Math.Sin(diferencaLongitude / 2) * Math.Sin(diferencaLongitude / 2);

        var resultadoFormula = 2 * Math.Atan2(Math.Sqrt(primeiraParteFormula), Math.Sqrt(1 - primeiraParteFormula));

        var distance = raioTerraEmKm * resultadoFormula;

        return distance;
    }
}
