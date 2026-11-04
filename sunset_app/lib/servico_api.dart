import 'dart:convert';
import 'package:flutter/foundation.dart' show kIsWeb;
import 'package:http/http.dart' as http;
import 'modelos.dart';

// Erro com a mensagem que a Api devolveu (ex.: "Email ou senha incorretos").
class ErroApi implements Exception {
  final String mensagem;
  ErroApi(this.mensagem);

  @override
  String toString() => mensagem;
}

// Todas as chamadas à Web API ficam aqui, para as telas não montarem URL nem JSON.
class ServicoApi {
  // No navegador usa localhost; no emulador Android o PC é 10.0.2.2.
  static String get urlBase => kIsWeb ? 'http://localhost:5218' : 'http://10.0.2.2:5218';

  static const _cabecalho = {'Content-Type': 'application/json'};

  static Future<Usuario> login(String email, String senha) async {
    final resposta = await _enviar(() => http.post(
          Uri.parse('$urlBase/api/usuarios/login'),
          headers: _cabecalho,
          body: jsonEncode({'email': email, 'senha': senha}),
        ));
    return Usuario.doJson(jsonDecode(resposta.body));
  }

  static Future<List<Quadra>> listarQuadras() async {
    final resposta = await _enviar(() => http.get(Uri.parse('$urlBase/api/quadras')));
    final lista = jsonDecode(resposta.body) as List;
    return lista.map((e) => Quadra.doJson(e)).toList();
  }

  static Future<List<Horario>> horarios(int quadraId, DateTime dia) async {
    final resposta = await _enviar(() => http.get(
        Uri.parse('$urlBase/api/quadras/$quadraId/horarios?data=${formatarDataApi(dia)}')));
    final json = jsonDecode(resposta.body);
    return (json['horarios'] as List).map((e) => Horario.doJson(e)).toList();
  }

  // Reserva 1 hora e devolve o id do pagamento Pix criado.
  static Future<int> reservar(int quadraId, DateTime dia, String horaInicio) async {
    final resposta = await _enviar(() => http.post(
          Uri.parse('$urlBase/api/agendamentos/reservar'),
          headers: _cabecalho,
          body: jsonEncode({
            'usuarioId': Sessao.usuario!.id,
            'quadraId': quadraId,
            'data': formatarDataApi(dia),
            'horaInicio': '$horaInicio:00',
          }),
        ));
    return jsonDecode(resposta.body)['pagamentoId'];
  }

  static Future<List<Agendamento>> meusAgendamentos() async {
    final resposta = await _enviar(() => http.get(
        Uri.parse('$urlBase/api/agendamentos?usuarioId=${Sessao.usuario!.id}')));
    final lista = jsonDecode(resposta.body) as List;
    return lista.map((e) => Agendamento.doJson(e)).toList();
  }

  static Future<void> cancelar(int agendamentoId) async {
    await _enviar(() => http.post(
          Uri.parse('$urlBase/api/agendamentos/$agendamentoId/cancelar'),
          headers: _cabecalho,
          body: jsonEncode({'usuarioId': Sessao.usuario!.id}),
        ));
  }

  static Future<Pix> obterPix(int pagamentoId) async {
    final resposta = await _enviar(() => http.get(
        Uri.parse('$urlBase/api/pagamentos/$pagamentoId/pix?usuarioId=${Sessao.usuario!.id}')));
    return Pix.doJson(jsonDecode(resposta.body));
  }

  static Future<void> simularPagamento(int pagamentoId) async {
    await _enviar(() => http.post(
          Uri.parse('$urlBase/api/pagamentos/$pagamentoId/simular'),
          headers: _cabecalho,
          body: jsonEncode({'usuarioId': Sessao.usuario!.id}),
        ));
  }

  // Faz a chamada e transforma qualquer falha em ErroApi com uma mensagem legível.
  static Future<http.Response> _enviar(Future<http.Response> Function() chamada) async {
    http.Response resposta;
    try {
      resposta = await chamada().timeout(const Duration(seconds: 15));
    } catch (_) {
      throw ErroApi('Não foi possível falar com a Api. Ela está rodando?');
    }

    if (resposta.statusCode >= 200 && resposta.statusCode < 300) return resposta;

    // A Api devolve {"erro": "mensagem"} nos erros de regra de negócio.
    try {
      final json = jsonDecode(resposta.body);
      if (json is Map && json['erro'] != null) throw ErroApi(json['erro']);
      if (json is Map && json['errors'] != null) {
        final primeiro = (json['errors'] as Map).values.first as List;
        throw ErroApi(primeiro.first.toString());
      }
    } on ErroApi {
      rethrow;
    } catch (_) {}
    throw ErroApi('Erro ${resposta.statusCode} ao falar com a Api.');
  }
}
