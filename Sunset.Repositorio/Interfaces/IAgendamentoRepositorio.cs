using Sunset.Modelos;

namespace Sunset.Repositorio.Interfaces;

public interface IAgendamentoRepositorio
{
    // Executa a stored procedure CadastrarAgendamento e devolve o id criado.
    int CadastrarPelaProcedure(DateTime data, TimeSpan horaInicio, TimeSpan horaFim,
                               int usuarioId, int quadraId, int pagamentoId);

    // Consultas feitas com Entity Framework Core.
    Agendamento? ObterPorId(int id);
    List<Agendamento> ListarPorUsuario(int usuarioId);
    List<Agendamento> ListarDaQuadraNoDia(int quadraId, DateTime dia);
    Agendamento? ObterPorPagamento(int pagamentoId);
    // Reservas que ficaram sem pagar além do prazo do Pix.
    List<Agendamento> ListarPendentesVencidos(DateTime limite);
    // Para o painel do Admin: busca por nome (cliente ou quadra) e período.
    List<Agendamento> ListarTodos(string? busca, DateTime? de, DateTime? ate);
    // Para o relatório do gestor: só os agendamentos das quadras informadas, dentro do período.
    List<Agendamento> ListarDasQuadras(List<int> quadraIds, DateTime? de, DateTime? ate);
    void Atualizar(Agendamento agendamento);
    void Remover(Agendamento agendamento);
}
