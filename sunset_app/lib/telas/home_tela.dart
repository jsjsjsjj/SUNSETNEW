import 'package:flutter/material.dart';
import '../modelos.dart';
import 'agendamentos_tela.dart';
import 'login_tela.dart';
import 'quadras_tela.dart';

// Tela principal com duas abas: Quadras (para reservar) e Meus agendamentos.
class HomeTela extends StatefulWidget {
  const HomeTela({super.key});

  @override
  State<HomeTela> createState() => _HomeTelaState();
}

class _HomeTelaState extends State<HomeTela> {
  int _aba = 0;

  void _sair() {
    Sessao.usuario = null;
    Navigator.of(context).pushReplacement(MaterialPageRoute(builder: (_) => const LoginTela()));
  }

  @override
  Widget build(BuildContext context) {
    final telas = [const QuadrasTela(), const AgendamentosTela()];

    return Scaffold(
      appBar: AppBar(
        title: Text('Olá, ${Sessao.usuario!.nome}'),
        actions: [
          IconButton(onPressed: _sair, icon: const Icon(Icons.logout), tooltip: 'Sair'),
        ],
      ),
      // Chave nova ao trocar de aba para a lista ser recarregada.
      body: KeyedSubtree(key: ValueKey(_aba), child: telas[_aba]),
      bottomNavigationBar: NavigationBar(
        selectedIndex: _aba,
        onDestinationSelected: (i) => setState(() => _aba = i),
        destinations: const [
          NavigationDestination(icon: Icon(Icons.sports_volleyball), label: 'Quadras'),
          NavigationDestination(icon: Icon(Icons.event), label: 'Meus agendamentos'),
        ],
      ),
    );
  }
}
