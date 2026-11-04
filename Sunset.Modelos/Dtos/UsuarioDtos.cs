using System.ComponentModel.DataAnnotations;

namespace Sunset.Modelos.Dtos;

// DTO de ENTRADA do cadastro (tela de cadastro da Web e POST da Api).
public class CadastroDto
{
    [Display(Name = "Nome")]
    [Required(ErrorMessage = "Informe o nome.")]
    [StringLength(50, ErrorMessage = "O nome pode ter no máximo 50 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Display(Name = "Sobrenome")]
    [Required(ErrorMessage = "Informe o sobrenome.")]
    [StringLength(49, ErrorMessage = "O sobrenome pode ter no máximo 49 caracteres.")]
    public string Sobrenome { get; set; } = string.Empty;

    [Display(Name = "E-mail")]
    [Required(ErrorMessage = "Informe o e-mail.")]
    [StringLength(100)]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "CPF")]
    [Required(ErrorMessage = "Informe o CPF.")]
    public string Cpf { get; set; } = string.Empty;

    [Display(Name = "Senha")]
    [Required(ErrorMessage = "Informe a senha.")]
    [DataType(DataType.Password)]
    public string Senha { get; set; } = string.Empty;

    [Display(Name = "Confirmar senha")]
    [Required(ErrorMessage = "Confirme a senha.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Senha), ErrorMessage = "As senhas não coincidem.")]
    public string ConfirmarSenha { get; set; } = string.Empty;

    // Só usado na tela: depois do cadastro, leva o usuário para cadastrar a quadra dele.
    [Display(Name = "Quero cadastrar minha quadra (ser gestor)")]
    public bool QuerSerGestor { get; set; }
}

// DTO de ENTRADA do login. O "usuário" do login é o e-mail.
public class LoginDto
{
    [Display(Name = "E-mail")]
    [Required(ErrorMessage = "Informe o e-mail.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Senha")]
    [Required(ErrorMessage = "Informe a senha.")]
    [DataType(DataType.Password)]
    public string Senha { get; set; } = string.Empty;
}

// DTO de SAÍDA: o que se mostra de um usuário. Nunca leva a senha, nem o hash.
public class UsuarioDto
{
    public int Id { get; set; }

    [Display(Name = "Nome")]
    public string NomeSobrenome { get; set; } = string.Empty;

    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Perfil")]
    public string Permissao { get; set; } = string.Empty;

    public static UsuarioDto De(Usuario u) => new()
    {
        Id = u.Id,
        NomeSobrenome = u.NomeSobrenome,
        Email = u.Email,
        Permissao = u.Permissao
    };
}
