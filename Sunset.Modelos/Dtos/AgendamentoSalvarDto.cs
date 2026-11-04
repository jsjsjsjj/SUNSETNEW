using System.ComponentModel.DataAnnotations;

namespace Sunset.Modelos.Dtos;

// DTO de ENTRADA do agendamento (usado pela Api).
public class AgendamentoSalvarDto
{
    [Required(ErrorMessage = "Informe a data.")]
    public DateTime DataReserva { get; set; }

    // Formato "18:00:00"
    [Required(ErrorMessage = "Informe a hora de início.")]
    public TimeSpan HoraInicio { get; set; }

    [Required(ErrorMessage = "Informe a hora de término.")]
    public TimeSpan HoraFim { get; set; }

    public int UsuarioId { get; set; }
    public int QuadraId { get; set; }
    public int PagamentoId { get; set; }
}
