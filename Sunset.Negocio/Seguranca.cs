using System.Security.Cryptography;
using System.Text;

namespace Sunset.Negocio;

// Guarda e confere senhas. A senha digitada nunca vai para o banco:
// só o HASH SHA256 dela (a mesma ideia da classe Seguranca do SistemaLogin do semestre passado).
public static class Seguranca
{
    public static string GerarHash(string texto)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(texto));

        // Transforma os bytes em texto hexadecimal (64 caracteres).
        var builder = new StringBuilder();
        foreach (byte b in bytes)
        {
            builder.Append(b.ToString("x2"));
        }
        return builder.ToString();
    }
}
