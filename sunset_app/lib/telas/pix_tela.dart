import 'dart:async';
import 'package:flutter/material.dart';
import '../modelos.dart';
import '../servico_api.dart';

// Pix simulado: mostra o valor, o código de exemplo e o relógio de 5 minutos.
// O prazo de verdade é conferido na Api; o relógio daqui é só visual.
class PixTela extends StatefulWidget {
  final int pagamentoId;
  const PixTela({super.key, required this.pagamentoId});

  @override
  State<PixTela> createState() => _PixTelaState();
}

class _PixTelaState extends State<PixTela> {
  Pix? _pix;
  String? _erro;
  int _restante = 0;
  Timer? _relogio;

  @override
  void initState() {
    super.initState();
    _carregar();
  }

  @override
  void dispose() {
    _relogio?.cancel();
    super.dispose();
  }

  Future<void> _carregar() async {
    try {
      final pix = await ServicoApi.obterPix(widget.pagamentoId);
      setState(() {
        _pix = pix;
        _restante = pix.segundosRestantes;
      });
      _relogio?.cancel();
      _relogio = Timer.periodic(const Duration(seconds: 1), (_) {
        if (_restante <= 1) {
          _relogio?.cancel();
          _carregar(); // zerou: pergunta à Api como ficou o pagamento
        } else {
          setState(() => _restante--);
        }
      });
    } on ErroApi catch (e) {
      setState(() => _erro = e.mensagem);
    }
  }

  Future<void> _simular() async {
    try {
      await ServicoApi.simularPagamento(widget.pagamentoId);
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Pagamento aprovado! Reserva confirmada.')));
      Navigator.of(context).pop();
    } on ErroApi catch (e) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(e.mensagem)));
      _carregar();
    }
  }

  String get _tempo {
    final min = (_restante ~/ 60).toString().padLeft(2, '0');
    final seg = (_restante % 60).toString().padLeft(2, '0');
    return '$min:$seg';
  }

  @override
  Widget build(BuildContext context) {
    Widget corpo;
    final pix = _pix;

    if (_erro != null) {
      corpo = Text(_erro!, textAlign: TextAlign.center);
    } else if (pix == null) {
      corpo = const CircularProgressIndicator();
    } else if (pix.status == 'Aprovado') {
      corpo = const Text('Pagamento já aprovado.');
    } else if (pix.status != 'Pendente' || pix.segundosRestantes <= 0 && _restante <= 0) {
      corpo = const Text('O tempo do Pix acabou e a reserva foi cancelada.', textAlign: TextAlign.center);
    } else {
      corpo = Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          Text(formatarDinheiro(pix.valor),
              style: Theme.of(context).textTheme.displaySmall?.copyWith(fontWeight: FontWeight.bold)),
          const SizedBox(height: 16),
          // No lugar do QR Code de verdade, um ícone (a tela do site mostra um QR de exemplo).
          const Icon(Icons.qr_code_2, size: 160),
          const Text('QR Code de exemplo: pagamento simulado', style: TextStyle(fontSize: 12)),
          const SizedBox(height: 12),
          SelectableText(pix.codigo, textAlign: TextAlign.center),
          const SizedBox(height: 16),
          Text('Tempo para pagar: $_tempo', style: const TextStyle(fontWeight: FontWeight.bold)),
          const SizedBox(height: 16),
          FilledButton(onPressed: _simular, child: const Text('Simular pagamento')),
        ],
      );
    }

    return Scaffold(
      appBar: AppBar(title: const Text('Pagamento por Pix')),
      body: Center(child: Padding(padding: const EdgeInsets.all(24), child: corpo)),
    );
  }
}
