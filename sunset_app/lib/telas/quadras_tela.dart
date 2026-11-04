import 'package:flutter/material.dart';
import '../modelos.dart';
import '../servico_api.dart';
import 'horarios_tela.dart';

// Lista as quadras vindas da Api (GET /api/quadras).
class QuadrasTela extends StatefulWidget {
  const QuadrasTela({super.key});

  @override
  State<QuadrasTela> createState() => _QuadrasTelaState();
}

class _QuadrasTelaState extends State<QuadrasTela> {
  late Future<List<Quadra>> _quadras;

  @override
  void initState() {
    super.initState();
    _quadras = ServicoApi.listarQuadras();
  }

  void _recarregar() {
    setState(() {
      _quadras = ServicoApi.listarQuadras();
    });
  }

  @override
  Widget build(BuildContext context) {
    return FutureBuilder<List<Quadra>>(
      future: _quadras,
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

        final quadras = resultado.data!;
        if (quadras.isEmpty) return const Center(child: Text('Nenhuma quadra cadastrada.'));

        return RefreshIndicator(
          onRefresh: () async => _recarregar(),
          child: ListView.builder(
            padding: const EdgeInsets.all(12),
            itemCount: quadras.length,
            itemBuilder: (context, i) {
              final q = quadras[i];
              return Card(
                child: ListTile(
                  leading: Icon(q.esporte == 'Volei' ? Icons.sports_volleyball : Icons.sports_tennis),
                  title: Text(q.nome),
                  subtitle: Text('${q.esporte} · ${formatarDinheiro(q.preco)} por hora\n${q.endereco}'),
                  isThreeLine: true,
                  trailing: const Icon(Icons.chevron_right),
                  onTap: () => Navigator.of(context)
                      .push(MaterialPageRoute(builder: (_) => HorariosTela(quadra: q))),
                ),
              );
            },
          ),
        );
      },
    );
  }
}
