using Sunset.Modelos;
using Sunset.Repositorio.Interfaces;

namespace Sunset.Negocio;

// Regras de negócio das quadras. Web e Api chamam esta classe;
// esta classe chama o repositório. Nenhuma regra fica no controller.
public class QuadraNegocio
{
    private readonly IQuadraRepositorio _repositorio;

    public QuadraNegocio(IQuadraRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public List<Quadra> Listar() => _repositorio.Listar();

    public Quadra? ObterPorId(int id) => _repositorio.ObterPorId(id);

    public List<Quadra> ListarDoGestor(int gestorId) => _repositorio.ListarPorGestor(gestorId);

    // Só o gestor dono da quadra (ou o Admin) pode editar ou excluir.
    public bool PodeGerenciar(Quadra quadra, int usuarioId, string perfil) =>
        perfil == Perfis.Admin || quadra.GestorId == usuarioId;

    // gestorId é o usuário logado que está cadastrando a quadra.
    // O banco (trigger trg_VirarGestor) promove esse usuário de Cliente para Gestor.
    public Quadra Cadastrar(Quadra quadra, int? gestorId = null)
    {
        Preparar(quadra);
        quadra.GestorId = gestorId;
        _repositorio.Adicionar(quadra);
        return quadra;
    }

    public void Atualizar(Quadra quadra)
    {
        var existente = _repositorio.ObterPorId(quadra.Id)
            ?? throw new RegraDeNegocioException("Quadra não encontrada.");

        Preparar(quadra);

        // Copia só o que pode mudar. O gestor continua o mesmo.
        existente.NomeQuadra = quadra.NomeQuadra;
        existente.PrecoHora = quadra.PrecoHora;
        existente.Categoria = quadra.Categoria;

        if (existente.Endereco is null)
        {
            existente.Endereco = quadra.Endereco;
        }
        else
        {
            existente.Endereco.Cep = quadra.Endereco!.Cep;
            existente.Endereco.Logradouro = quadra.Endereco.Logradouro;
            existente.Endereco.Numero = quadra.Endereco.Numero;
            existente.Endereco.Bairro = quadra.Endereco.Bairro;
            existente.Endereco.Cidade = quadra.Endereco.Cidade;
            existente.Endereco.Estado = quadra.Endereco.Estado;
        }

        _repositorio.Atualizar(existente);
    }

    // Devolve false quando a quadra não existe.
    public bool Remover(int id)
    {
        var quadra = _repositorio.ObterPorId(id);
        if (quadra is null) return false;

        try
        {
            _repositorio.Remover(quadra);
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException)
        {
            // Acontece quando a quadra já tem pagamentos ou agendamentos ligados a ela.
            throw new RegraDeNegocioException(
                "Não dá para excluir: esta quadra já tem agendamentos ou pagamentos.");
        }
        return true;
    }

    // ---------- regras ----------

    // Limpa os textos e confere as regras antes de gravar.
    private static void Preparar(Quadra quadra)
    {
        quadra.NomeQuadra = Texto.RemoverAcentos(quadra.NomeQuadra.Trim());
        if (!Texto.SoLetrasNumerosEspacos(quadra.NomeQuadra))
            throw new RegraDeNegocioException("O nome da quadra aceita só letras, números e espaços.");

        if (quadra.PrecoHora <= 0)
            throw new RegraDeNegocioException("O preço por hora precisa ser maior que zero.");

        if (!Categorias.Todas.Contains(quadra.Categoria))
            throw new RegraDeNegocioException("Esporte inválido. Use Volei ou Beach Tennis.");

        var e = quadra.Endereco
            ?? throw new RegraDeNegocioException("O endereço da quadra é obrigatório.");

        e.Cep = Texto.SoDigitos(e.Cep);
        if (e.Cep.Length != 8)
            throw new RegraDeNegocioException("O CEP precisa ter 8 dígitos.");

        e.Logradouro = Texto.RemoverAcentos(e.Logradouro.Trim());
        e.Cidade = Texto.RemoverAcentos(e.Cidade.Trim());
        e.Estado = Texto.RemoverAcentos(e.Estado.Trim());
        e.Bairro = string.IsNullOrWhiteSpace(e.Bairro) ? null : Texto.RemoverAcentos(e.Bairro.Trim());

        // Sem número, o banco aceita "S/N".
        e.Numero = string.IsNullOrWhiteSpace(e.Numero) ? "S/N" : e.Numero.Trim().ToUpperInvariant();

        foreach (var campo in new[] { e.Logradouro, e.Cidade, e.Estado })
        {
            if (!Texto.SoLetrasNumerosEspacos(campo))
                throw new RegraDeNegocioException(
                    "Rua, cidade e estado aceitam só letras, números e espaços.");
        }
        if (e.Bairro != null && !Texto.SoLetrasNumerosEspacos(e.Bairro))
            throw new RegraDeNegocioException("O bairro aceita só letras, números e espaços.");
    }
}
