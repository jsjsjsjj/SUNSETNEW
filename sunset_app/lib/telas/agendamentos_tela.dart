import 'package:flutter/material.dart';
import '../modelos.dart';
import '../servico_api.dart';
import 'pix_tela.dart';

// Funcionalidade complementar do app: o cliente acompanha e cancela os agendamentos dele.
class AgendamentosTela extends StatefulWidget {
  const AgendamentosTela({super.key});

  @override
  State<AgendamentosTela> createState() => _AgendamentosTelaState();
}

class _AgendamentosTelaState extends State<AgendamentosTela> {
  late Future<List<Agendamento>> _lista;

  @override
  void initState() {
    super.initState();
    _lista = ServicoApi.meusAgendamentos();
  }

  void _recarregar() {
    setState(() {
      _lista = ServicoApi.meusAgendamentos();
    });
  }

  Future<void> _cancelar(Agendamento a) async {
    final confirmou = await showDialog<bool>(
      context: context,
      builder: (_) => AlertDialog(
        title: const Text('Cancelar agendamento'),
        content: Text('Cancelar ${a.quadra} em ${formatarData(a.data)} às ${a.horaInicio}?'),
        actions: [
          TextButton(onPressed: () => Navigator.pop(context, false), child: const Text('Não')),
          FilledButton(onPressed: () => Navigator.pop(context, true), child: const Text('Cancelar')),
        ],
      ),
    );
    if (confirmou != true) return;

    try {
      await ServicoApi.cancelar(a.id);
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(const SnackBar(content: Text('Agendamento cancelado.')));
    } on ErroApi catch (e) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(e.mensagem)));
    }
    _recarregar();
  }

  Future<void> _pagar(Agendamento a) async {
    await Navigator.of(context).push(MaterialPageRoute(builder: (_) => PixTela(pagamentoId: a.pagamentoId)));
    _recarregar();
  }

  @override
  Widget build(BuildContext context) {
    return FutureBuilder<List<Agendamento>>(
      future: _lista,
      builder: (context, resultado) {
        if (resultado.connectionState != ConnectionState.done) {
          return const Center(child: CircularProgressIndicator());
        }
        if (resultado.hasError) {
          return Center(
            child: Column(mainAxisSize: MainAxisSize.min, children: [
              Text(resultado.error.toString(), textAlign: TextAlign.center),
              const SizedBox(height: 8),
              OutlinedButton(onPressed: _recarregar, child: const Text('Tentar de novo')),
            ]),
          );
        }

        final lista = resultado.data!;
        if (lista.isEmpty) return const Center(child: Text('Você ainda não tem agendamentos.'));

        return RefreshIndicator(
          onRefresh: () async => _recarregar(),
          child: ListView.builder(
            padding: const EdgeInsets.all(12),
            itemCount: lista.length,
            itemBuilder: (context, i) {
              final a = lista[i];
              return Card(
                child: Padding(
                  padding: const EdgeInsets.all(12),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(a.quadra, style: Theme.of(context).textTheme.titleMedium),
                      Text('${a.esporte} · ${formatarData(a.data)} · ${a.horaInicio} - ${a.horaFim}'),
                      Text('Status: ${a.status} · Pagamento: ${a.statusPagamento} (${formatarDinheiro(a.valor)})'),
                      if (a.podePagar || a.podeCancelar)
                        Row(
                          mainAxisAlignment: MainAxisAlignment.end,
                          children: [
                            if (a.podePagar)
                              TextButton(onPressed: () => _pagar(a), child: const Text('Pagar com Pix')),
                            if (a.podeCancelar)
                              TextButton(onPressed: () => _cancelar(a), child: const Text('Cancelar')),
                          ],
                        ),
                    ],
                  ),
                ),
              );
            },
          ),
        );
      },
    );
  }
}
