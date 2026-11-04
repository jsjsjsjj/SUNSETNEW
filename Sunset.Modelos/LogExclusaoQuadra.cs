namespace Sunset.Modelos;

// Preenchida só pela trigger trg_LogExclusaoQuadras, quando uma quadra é excluída.
// O C# apenas lê esta tabela.
public class LogExclusaoQuadra
{
    public int Id { get; set; }
    public int QuadraId { get; set; }
    public string NomeQuadra { get; set; } = string.Empty;
    public string UsuarioAlteracao { get; set; } = string.Empty;
    public DateTime DataAlteracao { get; set; }
}
