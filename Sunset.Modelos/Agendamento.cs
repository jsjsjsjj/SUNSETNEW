namespace Sunset.Modelos;

public class Agendamento
{
    public int Id { get; set; }

    // Só a data (a coluna do banco é DATE).
    public DateTime DataReserva { get; set; }

    // Horários como duração desde meia-noite (colunas TIME).
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraFim { get; set; }

    public string StatusReserva { get; set; } = Modelos.StatusReserva.Pendente;

    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    public int QuadraId { get; set; }
    public Quadra? Quadra { get; set; }

    public int PagamentoId { get; set; }
    public Pagamento? Pagamento { get; set; }
}
