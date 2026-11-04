namespace Sunset.Modelos;

public class Pagamento
{
    public int Id { get; set; }

    public decimal Valor { get; set; }

    // Quando o pagamento foi criado. O prazo de 5 minutos conta a partir daqui.
    public DateTime DataPagamento { get; set; }

    public string MetodoPagamento { get; set; } = "PIX";

    public string StatusPagamento { get; set; } = Modelos.StatusPagamento.Pendente;

    public int QuadraId { get; set; }
    public int UsuarioId { get; set; }
}
