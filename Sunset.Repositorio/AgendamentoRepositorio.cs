using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Sunset.Modelos;
using Sunset.Repositorio.Contexto;
using Sunset.Repositorio.Interfaces;

namespace Sunset.Repositorio;

public class AgendamentoRepositorio : IAgendamentoRepositorio
{
    private readonly AppDbContext _contexto;

    public AgendamentoRepositorio(AppDbContext contexto)
    {
        _contexto = contexto;
    }

    // ---------- Dapper + stored procedure ----------
    public int CadastrarPelaProcedure(DateTime data, TimeSpan horaInicio, TimeSpan horaFim,
                                      int usuarioId, int quadraId, int pagamentoId)
    {
        // Reaproveita a mesma conexão (e a mesma connection string) do EF.
        var conexao = _contexto.Database.GetDbConnection();

        // Os valores vão como parâmetros, separados do comando: isso impede SQL Injection.
        var parametros = new DynamicParameters();
        parametros.Add("DataReserva", data.Date, DbType.Date);
        parametros.Add("HoraInicio", horaInicio, DbType.Time);
        parametros.Add("HoraFim", horaFim, DbType.Time);
        parametros.Add("ID_Usuario", usuarioId);
        parametros.Add("ID_Quadra", quadraId);
        parametros.Add("ID_Pagamento", pagamentoId);

        try
        {
            // A procedure termina com um SELECT do id do agendamento criado.
            return conexao.QuerySingle<int>(
                "CadastrarAgendamento",
                parametros,
                commandType: CommandType.StoredProcedure);
        }
        catch (SqlException ex) when (ex.Number == 50000)
        {
            // 50000 = erro lançado pela própria procedure com RAISERROR
            // (conflito de horário, horário inválido, vínculo inexistente).
            throw new ErroDeBancoException(ex.Message);
        }
    }

    // ---------- Entity Framework Core ----------

    public Agendamento? ObterPorId(int id) =>
        _contexto.Agendamentos
            .Include(a => a.Quadra)
            .Include(a => a.Usuario)
            .Include(a => a.Pagamento)
            .FirstOrDefault(a => a.Id == id);

    // Agendamentos do cliente, os mais novos primeiro.
    public List<Agendamento> ListarPorUsuario(int usuarioId) =>
        _contexto.Agendamentos.AsNoTracking()
            .Include(a => a.Quadra)
            .Include(a => a.Pagamento)
            .Where(a => a.UsuarioId == usuarioId)
            .OrderByDescending(a => a.DataReserva).ThenBy(a => a.HoraInicio)
            .ToList();

    // Agendamentos que ocupam a quadra no dia (os cancelados não contam).
    public List<Agendamento> ListarDaQuadraNoDia(int quadraId, DateTime dia) =>
        _contexto.Agendamentos.AsNoTracking()
            .Include(a => a.Usuario)
            .Where(a => a.QuadraId == quadraId
                     && a.DataReserva == dia.Date
                     && a.StatusReserva != StatusReserva.Cancelado)
            .OrderBy(a => a.HoraInicio)
            .ToList();

    public Agendamento? ObterPorPagamento(int pagamentoId) =>
        _contexto.Agendamentos
            .Include(a => a.Quadra)
            .Include(a => a.Pagamento)
            .FirstOrDefault(a => a.PagamentoId == pagamentoId);

    public List<Agendamento> ListarPendentesVencidos(DateTime limite) =>
        _contexto.Agendamentos
            .Include(a => a.Pagamento)
            .Where(a => a.StatusReserva == StatusReserva.Pendente
                     && a.Pagamento!.StatusPagamento == StatusPagamento.Pendente
                     && a.Pagamento.DataPagamento < limite)
            .ToList();

    public List<Agendamento> ListarTodos(string? busca, DateTime? de, DateTime? ate)
    {
        var consulta = _contexto.Agendamentos.AsNoTracking()
            .Include(a => a.Quadra)
            .Include(a => a.Usuario)
            .Include(a => a.Pagamento)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(busca))
            consulta = consulta.Where(a => a.Usuario!.NomeSobrenome.Contains(busca)
                                        || a.Quadra!.NomeQuadra.Contains(busca));
        if (de.HasValue) consulta = consulta.Where(a => a.DataReserva >= de.Value.Date);
        if (ate.HasValue) consulta = consulta.Where(a => a.DataReserva <= ate.Value.Date);

        return consulta.OrderByDescending(a => a.DataReserva).ThenBy(a => a.HoraInicio).ToList();
    }

    public List<Agendamento> ListarDasQuadras(List<int> quadraIds, DateTime? de, DateTime? ate)
    {
        var consulta = _contexto.Agendamentos.AsNoTracking()
            .Include(a => a.Quadra)
            .Include(a => a.Usuario)
            .Include(a => a.Pagamento)
            .Where(a => quadraIds.Contains(a.QuadraId));

        if (de.HasValue) consulta = consulta.Where(a => a.DataReserva >= de.Value.Date);
        if (ate.HasValue) consulta = consulta.Where(a => a.DataReserva <= ate.Value.Date);

        return consulta.OrderBy(a => a.DataReserva).ThenBy(a => a.HoraInicio).ToList();
    }

    public void Remover(Agendamento agendamento)
    {
        _contexto.Agendamentos.Remove(agendamento);
        _contexto.SaveChanges();
    }

    public void Atualizar(Agendamento agendamento)
    {
        _contexto.Agendamentos.Update(agendamento);
        _contexto.SaveChanges();
    }
}
