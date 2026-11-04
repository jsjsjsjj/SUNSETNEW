using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Sunset.Negocio;

// Funções pequenas para tratar texto antes de gravar no banco.
public static class Texto
{
    // O banco só aceita letras sem acento, números e espaços (regra da documentação).
    // Por isso "João" vira "Joao" antes de gravar.
    public static string RemoverAcentos(string texto)
    {
        var separado = texto.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var c in separado)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }
        return sb.ToString().Normalize(NormalizationForm.FormC);
    }

    // Verdadeiro se o texto tem só letras (sem acento), números e espaços.
    public static bool SoLetrasNumerosEspacos(string texto) =>
        Regex.IsMatch(texto, "^[A-Za-z0-9 ]+$");

    // Guarda só os dígitos: "123.456.789-09" vira "12345678909".
    public static string SoDigitos(string texto) =>
        new string(texto.Where(char.IsDigit).ToArray());
}
