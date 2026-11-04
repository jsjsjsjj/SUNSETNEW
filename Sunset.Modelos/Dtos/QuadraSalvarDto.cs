using System.ComponentModel.DataAnnotations;

namespace Sunset.Modelos.Dtos;

// DTO de ENTRADA: o que o gestor preenche para cadastrar ou alterar uma quadra.
// Junta os dados da quadra e do endereço, porque a tela é uma só.
public class QuadraSalvarDto
{
    [Display(Name = "Nome da quadra")]
    [Required(ErrorMessage = "Informe o nome da quadra.")]
    [StringLength(50, ErrorMessage = "O nome pode ter no máximo 50 caracteres.")]
    public string NomeQuadra { get; set; } = string.Empty;

    [Display(Name = "Preço por hora (R$)")]
    [Range(0.01, 99999, ErrorMessage = "Informe um preço maior que zero.")]
    public decimal PrecoHora { get; set; }

    [Display(Name = "Esporte")]
    [Required(ErrorMessage = "Escolha o esporte.")]
    public string Categoria { get; set; } = Categorias.Volei;

    [Display(Name = "CEP")]
    [Required(ErrorMessage = "Informe o CEP.")]
    public string Cep { get; set; } = string.Empty;

    [Display(Name = "Rua")]
    [Required(ErrorMessage = "Informe a rua.")]
    [StringLength(70)]
    public string Logradouro { get; set; } = string.Empty;

    [Display(Name = "Número")]
    [StringLength(5, ErrorMessage = "Use no máximo 5 caracteres (ou S/N).")]
    public string? Numero { get; set; }

    [Display(Name = "Bairro")]
    [StringLength(40)]
    public string? Bairro { get; set; }

    [Display(Name = "Cidade")]
    [Required(ErrorMessage = "Informe a cidade.")]
    [StringLength(40)]
    public string Cidade { get; set; } = string.Empty;

    [Display(Name = "Estado")]
    [Required(ErrorMessage = "Informe o estado.")]
    [StringLength(40)]
    public string Estado { get; set; } = string.Empty;

    // Monta a entidade Quadra (com o Endereco dentro) para o Negocio.
    public Quadra ParaQuadra(int id = 0) => new()
    {
        Id = id,
        NomeQuadra = NomeQuadra,
        PrecoHora = PrecoHora,
        Categoria = Categoria,
        Endereco = new Endereco
        {
            Cep = Cep,
            Logradouro = Logradouro,
            Numero = Numero,
            Bairro = Bairro,
            Cidade = Cidade,
            Estado = Estado
        }
    };

    // Preenche o formulário de edição com os dados atuais.
    public static QuadraSalvarDto De(Quadra q) => new()
    {
        NomeQuadra = q.NomeQuadra,
        PrecoHora = q.PrecoHora,
        Categoria = q.Categoria,
        Cep = q.Endereco?.Cep ?? "",
        Logradouro = q.Endereco?.Logradouro ?? "",
        Numero = q.Endereco?.Numero,
        Bairro = q.Endereco?.Bairro,
        Cidade = q.Endereco?.Cidade ?? "",
        Estado = q.Endereco?.Estado ?? ""
    };
}
