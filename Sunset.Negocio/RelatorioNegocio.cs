using System.Text;
using Sunset.Modelos;
using Sunset.Repositorio.Interfaces;

namespace Sunset.Negocio;

// Monta o relatório geral do sistema em texto simples (arquivo .txt para o Admin baixar).
public class RelatorioNegocio
{
    private readonly IUsuarioRepositorio _usuarios;
    private readonly IQuadraRepositorio _quadras;
    private readonly IAgendamentoRepositorio _agendamentos;

    public RelatorioNegocio(IUsuarioRepositorio usuarios, IQuadraRepositorio quadras,
                            IAgendamentoRepositorio agendamentos)
    {
        _usuarios = usuarios;
        _quadras = quadras;
        _agendamentos = agendamentos;
    }

    public string Gerar()
    {
        var usuarios = _usuarios.Listar(null);
        var quadras = _quadras.Listar();
        var agendamentos = _agendamentos.ListarTodos(null, null, null);

        var texto = new StringBuilder();
        texto.AppendLine("SUNSET - RELATORIO GERAL");
        texto.AppendLine($"Gerado em {DateTime.Now:dd/MM/yyyy HH:mm}");
        texto.AppendLine(new string('=', 50));

        texto.AppendLine();
        texto.AppendLine("USUARIOS");
        texto.AppendLine($"Total: {usuarios.Count}");
        foreach (var perfil in new[] { Perfis.Admin, Perfis.Gestor, Perfis.Cliente })
            texto.AppendLine($"  {perfil}: {usuarios.Count(u => u.Permissao == perfil)}");

        texto.AppendLine();
        texto.AppendLine("QUADRAS");
        texto.AppendLine($"Total: {quadras.Count}");
        foreach (var esporte in Categorias.Todas)
            texto.AppendLine($"  {esporte}: {quadras.Count(q => q.Categoria == esporte)}");

        texto.AppendLine();
        texto.AppendLine("AGENDAMENTOS");
        texto.AppendLine($"Total: {agendamentos.Count}");
        foreach (var status in new[] { StatusReserva.Pendente, StatusReserva.EmAndamento, StatusReserva.Finalizado, StatusReserva.Cancelado })
            texto.AppendLine($"  {status}: {agendamentos.Count(a => a.StatusReserva == status)}");

        // Faturamento = soma dos pagamentos aprovados.
        var faturamento = agendamentos
            .Where(a => a.Pagamento?.StatusPagamento == StatusPagamento.Aprovado)
            .Sum(a => a.Pagamento!.Valor);
        texto.AppendLine();
        texto.AppendLine($"Faturamento (pagamentos aprovados): R$ {faturamento:0.00}");

        texto.AppendLine();
        texto.AppendLine("LISTA DE AGENDAMENTOS");
        foreach (var a in agendamentos)
        {
            texto.AppendLine($"  #{a.Id} | {a.DataReserva:dd/MM/yyyy} {a.HoraInicio:hh\\:mm}-{a.HoraFim:hh\\:mm} | " +
                             $"{a.Quadra?.NomeQuadra} | {a.Usuario?.NomeSobrenome} | {a.StatusReserva} | " +
                             $"pagamento {a.Pagamento?.StatusPagamento}");
        }
        return texto.ToString();
    }

    // Relatório do gestor: só as quadras dele (o Admin que abrir esta tela vê todas, como na agenda).
    // "de" e "ate" são opcionais; sem período, entram todos os agendamentos.
    public string GerarDoGestor(int usuarioId, string perfil, DateTime? de, DateTime? ate)
    {
        var quadras = perfil == Perfis.Admin ? _quadras.Listar() : _quadras.ListarPorGestor(usuarioId);
        var idsDasQuadras = quadras.Select(q => q.Id).ToList();
        var agendamentos = _agendamentos.ListarDasQuadras(idsDasQuadras, de, ate);

        var texto = new StringBuilder();
        texto.AppendLine("SUNSET - RELATORIO DAS QUADRAS");
        texto.AppendLine($"Gerado em {DateTime.Now:dd/MM/yyyy HH:mm}");
        var periodo = de.HasValue || ate.HasValue
            ? $"{(de.HasValue ? de.Value.ToString("dd/MM/yyyy") : "inicio")} a {(ate.HasValue ? ate.Value.ToString("dd/MM/yyyy") : "hoje em diante")}"
            : "todos os agendamentos";
        texto.AppendLine($"Periodo: {periodo}");
        texto.AppendLine(new string('=', 50));

        texto.AppendLine();
        texto.AppendLine("QUADRAS");
        if (quadras.Count == 0) texto.AppendLine("  Nenhuma quadra cadastrada.");
        foreach (var q in quadras)
            texto.AppendLine($"  {q.NomeQuadra} ({q.Categoria})");

        texto.AppendLine();
        texto.AppendLine("AGENDAMENTOS");
        texto.AppendLine($"Total: {agendamentos.Count}");
        foreach (var status in new[] { StatusReserva.Pendente, StatusReserva.EmAndamento, StatusReserva.Finalizado, StatusReserva.Cancelado })
            texto.AppendLine($"  {status}: {agendamentos.Count(a => a.StatusReserva == status)}");

        // Faturamento = soma dos pagamentos aprovados.
        var faturamento = agendamentos
            .Where(a => a.Pagamento?.StatusPagamento == StatusPagamento.Aprovado)
            .Sum(a => a.Pagamento!.Valor);
        texto.AppendLine();
        texto.AppendLine($"Faturamento (pagamentos aprovados): R$ {faturamento:0.00}");

        texto.AppendLine();
        texto.AppendLine("LISTA DE AGENDAMENTOS");
        if (agendamentos.Count == 0) texto.AppendLine("  Nenhum agendamento no periodo.");
        foreach (var a in agendamentos)
        {
            texto.AppendLine($"  #{a.Id} | {a.DataReserva:dd/MM/yyyy} {a.HoraInicio:hh\\:mm}-{a.HoraFim:hh\\:mm} | " +
                             $"{a.Quadra?.NomeQuadra} | {a.Usuario?.NomeSobrenome} | {a.StatusReserva} | " +
                             $"pagamento {a.Pagamento?.StatusPagamento}");
        }
        return texto.ToString();
    }
}
