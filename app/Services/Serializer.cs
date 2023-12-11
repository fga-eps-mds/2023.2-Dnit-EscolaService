using System.Text;

namespace app.Services
{
    public interface ISerializable
    {
        List<object?> Serialize();
    }

    public static class CsvSerializer
    {
        public static string Serialize(ISerializable value, string delimiter) {
            var fields = value.Serialize()
            .Select(f => f?.ToString() ?? "")
            .Select(f => f.Replace("\\", "\\\\").Replace("\"", "\"\""));
            return string.Join(delimiter, fields);
        }
    }
}
