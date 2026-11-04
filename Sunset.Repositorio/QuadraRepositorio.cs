using Microsoft.EntityFrameworkCore;
using Sunset.Modelos;
using Sunset.Repositorio.Contexto;
using Sunset.Repositorio.Interfaces;

namespace Sunset.Repositorio;

// Única classe que conversa com a tabela Quadras (e o Endereco dela).
// Usa Entity Framework Core para incluir, consultar, alterar e excluir.
public class QuadraRepositorio : IQuadraRepositorio
{
    private readonly AppDbContext _contexto;

    // O contexto chega pronto pela injeção de dependência (ver Program.cs).
    public QuadraRepositorio(AppDbContext contexto)
    {
        _contexto = contexto;
    }

    public List<Quadra> Listar() =>
        _contexto.Quadras.AsNoTracking()
            .Include(q => q.Endereco)
            .OrderBy(q => q.NomeQuadra)
            .ToList();

    public List<Quadra> ListarPorGestor(int gestorId) =>
        _contexto.Quadras.AsNoTracking()
            .Include(q => q.Endereco)
            .Where(q => q.GestorId == gestorId)
            .OrderBy(q => q.NomeQuadra)
            .ToList();

    public Quadra? ObterPorId(int id) =>
        _contexto.Quadras.Include(q => q.Endereco).FirstOrDefault(q => q.Id == id);

    public void Adicionar(Quadra quadra)
    {
        // Como a quadra tem o Endereco dentro, o EF grava os dois juntos.
        _contexto.Quadras.Add(quadra);
        _contexto.SaveChanges();
    }

    public void Atualizar(Quadra quadra)
    {
        _contexto.Quadras.Update(quadra);
        _contexto.SaveChanges();
    }

    public void Remover(Quadra quadra)
    {
        var endereco = quadra.Endereco;

        // A trigger trg_LogExclusaoQuadras grava o log sozinha quando a quadra sai.
        _contexto.Quadras.Remove(quadra);
        if (endereco != null) _contexto.Enderecos.Remove(endereco);
        _contexto.SaveChanges();
    }
}
