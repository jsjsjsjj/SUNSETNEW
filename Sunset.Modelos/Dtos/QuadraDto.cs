using System.ComponentModel.DataAnnotations;

namespace Sunset.Modelos.Dtos;

// DTO de SAÍDA: o que o sistema mostra de uma quadra (telas da Web e JSON da Api).
public class QuadraDto
{
    public int Id { get; set; }

    [Display(Name = "Quadra")]
    public string NomeQuadra { get; set; } = string.Empty;

    [Display(Name = "Preço por hora")]
    public decimal PrecoHora { get; set; }

    [Display(Name = "Esporte")]
    public string Categoria { get; set; } = string.Empty;

    // Endereço já montado em uma linha só, para mostrar na tela.
    [Display(Name = "Endereço")]
    public string Endereco { get; set; } = string.Empty;

    public static QuadraDto De(Quadra q) => new()
    {
        Id = q.Id,
        NomeQuadra = q.NomeQuadra,
        PrecoHora = q.PrecoHora,
        Categoria = q.Categoria,
        Endereco = q.Endereco is null
            ? ""
            : $"{q.Endereco.Logradouro}, {q.Endereco.Numero} - {q.Endereco.Bairro}, {q.Endereco.Cidade}/{q.Endereco.Estado}"
    };
}
