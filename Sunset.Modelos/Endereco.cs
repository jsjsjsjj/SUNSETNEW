using System.ComponentModel.DataAnnotations;

namespace Sunset.Modelos;

public class Endereco
{
    public int Id { get; set; }

    [Required, StringLength(8)]
    public string Cep { get; set; } = string.Empty;

    [Required, StringLength(70)]
    public string Logradouro { get; set; } = string.Empty;

    // Texto, porque o banco aceita "S/N" quando o imóvel não tem número.
    [StringLength(5)]
    public string? Numero { get; set; }

    [StringLength(40)]
    public string? Bairro { get; set; }

    [Required, StringLength(40)]
    public string Cidade { get; set; } = string.Empty;

    [Required, StringLength(40)]
    public string Estado { get; set; } = string.Empty;
}
