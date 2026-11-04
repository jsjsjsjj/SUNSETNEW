using System.ComponentModel.DataAnnotations;

namespace Sunset.Modelos;

// Representa uma linha da tabela Usuario.
// A senha guardada aqui já é o HASH (SHA256), nunca a senha digitada.
public class Usuario
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string NomeSobrenome { get; set; } = string.Empty;

    [Required, StringLength(100), EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(255)]
    public string Senha { get; set; } = string.Empty;

    // Só os 11 dígitos, sem ponto nem traço.
    [Required, StringLength(11, MinimumLength = 11)]
    public string Cpf { get; set; } = string.Empty;

    // Admin, Gestor ou Cliente (veja a classe Perfis).
    public string Permissao { get; set; } = Perfis.Cliente;

    public string? Genero { get; set; }

    // Ativo, Inativo ou Bloqueado.
    public string StatusUsuario { get; set; } = "Ativo";
}
