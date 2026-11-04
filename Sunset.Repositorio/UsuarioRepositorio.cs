using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Sunset.Modelos;
using Sunset.Repositorio.Contexto;
using Sunset.Repositorio.Interfaces;

namespace Sunset.Repositorio;

// Única classe que conversa com a tabela Usuario.
public class UsuarioRepositorio : IUsuarioRepositorio
{
    private readonly AppDbContext _contexto;

    public UsuarioRepositorio(AppDbContext contexto)
    {
        _contexto = contexto;
    }

    public Usuario? ObterPorId(int id) =>
        _contexto.Usuarios.FirstOrDefault(u => u.Id == id);

    public Usuario? ObterPorEmail(string email) =>
        _contexto.Usuarios.FirstOrDefault(u => u.Email == email);

    public bool EmailEmUso(string email) =>
        _contexto.Usuarios.Any(u => u.Email == email);

    public bool CpfEmUso(string cpf) =>
        _contexto.Usuarios.Any(u => u.Cpf == cpf);

    public void Adicionar(Usuario usuario)
    {
        _contexto.Usuarios.Add(usuario);
        _contexto.SaveChanges();
    }

    public List<Usuario> Listar(string? nome)
    {
        var consulta = _contexto.Usuarios.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(nome))
            consulta = consulta.Where(u => u.NomeSobrenome.Contains(nome) || u.Email.Contains(nome));
        return consulta.OrderBy(u => u.NomeSobrenome).ToList();
    }

    public void Excluir(int executorId, int usuarioId)
    {
        try
        {
            // O banco só deixa excluir se a sessão informar o id de um Admin
            // (trigger trg_ExclusaoSomenteAdmin). Os dois comandos vão no mesmo lote,
            // então usam a mesma conexão e o valor do contexto vale para o DELETE.
            _contexto.Database.ExecuteSqlInterpolated(
                $"EXEC sp_set_session_context N'ExecutorID', {executorId}; DELETE FROM Usuario WHERE IDUsuario = {usuarioId}");
        }
        catch (SqlException ex) when (ex.Number == 50000)
        {
            // Mensagem da trigger ("Ação negada: ...").
            throw new ErroDeBancoException(ex.Message);
        }
        catch (SqlException ex) when (ex.Number == 547)
        {
            throw new ErroDeBancoException("Não dá para excluir: o usuário tem agendamentos, pagamentos ou quadras.");
        }
    }
}
