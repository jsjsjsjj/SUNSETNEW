using Sunset.Modelos;

namespace Sunset.Repositorio.Interfaces;

// Contrato do repositório de quadras. O Negocio conhece só esta interface.
public interface IQuadraRepositorio
{
    List<Quadra> Listar();
    List<Quadra> ListarPorGestor(int gestorId);
    Quadra? ObterPorId(int id);
    void Adicionar(Quadra quadra);
    void Atualizar(Quadra quadra);
    void Remover(Quadra quadra);
}
