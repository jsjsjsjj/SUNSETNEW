using Sunset.Modelos;

namespace Sunset.Repositorio.Interfaces;

public interface IPagamentoRepositorio
{
    Pagamento? ObterPorId(int id);
    void Adicionar(Pagamento pagamento);
    void Atualizar(Pagamento pagamento);
    void Remover(Pagamento pagamento);
}
