using Sunset.Modelos;
using Sunset.Repositorio.Contexto;
using Sunset.Repositorio.Interfaces;

namespace Sunset.Repositorio;

// Acesso à tabela Pagamentos (Entity Framework Core).
public class PagamentoRepositorio : IPagamentoRepositorio
{
    private readonly AppDbContext _contexto;

    public PagamentoRepositorio(AppDbContext contexto)
    {
        _contexto = contexto;
    }

    public Pagamento? ObterPorId(int id) => _contexto.Pagamentos.Find(id);

    public void Adicionar(Pagamento pagamento)
    {
        _contexto.Pagamentos.Add(pagamento);
        _contexto.SaveChanges();
    }

    public void Atualizar(Pagamento pagamento)
    {
        _contexto.Pagamentos.Update(pagamento);
        _contexto.SaveChanges();
    }

    public void Remover(Pagamento pagamento)
    {
        _contexto.Pagamentos.Remove(pagamento);
        _contexto.SaveChanges();
    }
}
