using Sunset.Modelos;

namespace Sunset.Repositorio.Interfaces;

public interface IUsuarioRepositorio
{
    Usuario? ObterPorId(int id);
    Usuario? ObterPorEmail(string email);
    bool EmailEmUso(string email);
    bool CpfEmUso(string cpf);
    void Adicionar(Usuario usuario);
    List<Usuario> Listar(string? nome);
    // Só o Admin consegue excluir: a trigger do banco confere o executorId.
    void Excluir(int executorId, int usuarioId);
}
