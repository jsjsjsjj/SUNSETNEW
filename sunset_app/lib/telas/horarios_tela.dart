import 'package:flutter/material.dart';
import '../modelos.dart';
import '../servico_api.dart';
import 'pix_tela.dart';

// Mostra os horários de uma quadra em um dia e permite reservar um horário livre.
class HorariosTela extends StatefulWidget {
  final Quadra quadra;
  const HorariosTela({super.key, required this.quadra});

  @override
  State<HorariosTela> createState() => _HorariosTelaState();
}

class _HorariosTelaState extends State<HorariosTela> {
  DateTime _dia = DateTime.now();
  late Future<List<Horario>> _horarios;

  @override
  void initState() {
    super.initState();
    _carregar();
  }

  void _carregar() {
    // As chaves são importantes: o setState não pode devolver um Future.
    setState(() {
      _horarios = ServicoApi.horarios(widget.quadra.id, _dia);
    });
  }

  Future<void> _escolherDia() async {
    final hoje = DateTime.now();
    final escolhido = await showDatePicker(
      context: context,
      initialDate: _dia,
      firstDate: DateTime(hoje.year, hoje.month, hoje.day),
      lastDate: hoje.add(const Duration(days: 60)),
    );
    if (escolhido != null) {
      _dia = escolhido;
      _carregar();
    }
  }

  Future<void> _reservar(Horario h) async {
    try {
      final pagamentoId = await ServicoApi.reservar(widget.quadra.id, _dia, h.inicio);
      if (!mounted) return;
      // Reservou: vai para o pagamento do Pix.
      await Navigator.of(context).push(MaterialPageRoute(builder: (_) => PixTela(pagamentoId: pagamentoId)));
      _carregar();
    } on ErroApi catch (e) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(e.mensagem)));
      _carregar();
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text(widget.quadra.nome)),
      body: Column(
        children: [
          ListTile(
            title: Text('Dia: ${formatarData(_dia)}'),
            trailing: const Icon(Icons.calendar_month),
            onTap: _escolherDia,
          ),
          const Divider(height: 1),
          Expanded(
            child: FutureBuilder<List<Horario>>(
              future: _horarios,
              builder: (context, resultado) {
                if (resultado.connectionState != ConnectionState.done) {
                  return const Center(child: CircularProgressIndicator());
                }
                if (resultado.hasError) return Center(child: Text(resultado.error.toString()));

                return GridView.count(
                  crossAxisCount: 2,
                  childAspectRatio: 2.4,
                  padding: const EdgeInsets.all(12),
                  mainAxisSpacing: 8,
                  crossAxisSpacing: 8,
                  children: resultado.data!.map((h) {
                    return FilledButton.tonal(
                      onPressed: h.livre ? () => _reservar(h) : null,
                      child: Text('${h.inicio} - ${h.fim}\n${h.ocupado ? 'Ocupado' : h.passado ? 'Encerrado' : 'Livre'}',
                          textAlign: TextAlign.center),
                    );
                  }).toList(),
                );
              },
            ),
          ),
        ],
      ),
    );
  }
}
