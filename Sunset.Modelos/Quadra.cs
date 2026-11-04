using System.ComponentModel.DataAnnotations;

namespace Sunset.Modelos;

public class Quadra
{
    public int Id { get; set; }

    [Required, StringLength(50)]
    public string NomeQuadra { get; set; } = string.Empty;

    public decimal PrecoHora { get; set; }

    // Volei ou Beach Tennis (veja a classe Categorias).
    [Required, StringLength(40)]
    public string Categoria { get; set; } = Categorias.Volei;

    public int? EnderecoId { get; set; }
    public Endereco? Endereco { get; set; }

    // Usuário responsável pela quadra. Quando a quadra é cadastrada,
    // uma trigger do banco promove esse usuário de Cliente para Gestor.
    public int? GestorId { get; set; }
    public Usuario? Gestor { get; set; }
}
