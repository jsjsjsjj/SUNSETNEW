namespace Sunset.Repositorio;

// Lançada quando uma procedure do banco recusa a operação (RAISERROR),
// por exemplo "quadra já possui agendamento neste horário".
// O Negocio captura e transforma em mensagem para o usuário.
public class ErroDeBancoException : Exception
{
    public ErroDeBancoException(string mensagem) : base(mensagem) { }
}
