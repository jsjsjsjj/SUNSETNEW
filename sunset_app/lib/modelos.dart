// Classes simples que representam o JSON devolvido pela Api (os DTOs do C#).

class Usuario {
  final int id;
  final String nome;
  final String email;
  final String perfil;

  Usuario({required this.id, required this.nome, required this.email, required this.perfil});

  factory Usuario.doJson(Map<String, dynamic> json) => Usuario(
        id: json['id'],
        nome: json['nomeSobrenome'],
        email: json['email'],
        perfil: json['permissao'],
      );
}

class Quadra {
  final int id;
  final String nome;
  final double preco;
  final String esporte;
  final String endereco;

  Quadra({required this.id, required this.nome, required this.preco, required this.esporte, required this.endereco});

  factory Quadra.doJson(Map<String, dynamic> json) => Quadra(
        id: json['id'],
        nome: json['nomeQuadra'],
        preco: (json['precoHora'] as num).toDouble(),
        esporte: json['categoria'],
        endereco: json['endereco'],
      );
}

class Horario {
  final String inicio;
  final String fim;
  final bool ocupado;
  final bool passado;

  Horario({required this.inicio, required this.fim, required this.ocupado, required this.passado});

  factory Horario.doJson(Map<String, dynamic> json) => Horario(
        inicio: json['inicio'],
        fim: json['fim'],
        ocupado: json['ocupado'],
        passado: json['passado'],
      );

  bool get livre => !ocupado && !passado;
}

class Agendamento {
  final int id;
  final String quadra;
  final String esporte;
  final DateTime data;
  final String horaInicio;
  final String horaFim;
  final String status;
  final bool podeCancelar;
  final int pagamentoId;
  final double valor;
  final String statusPagamento;
  final bool podePagar;

  Agendamento({
    required this.id,
    required this.quadra,
    required this.esporte,
    required this.data,
    required this.horaInicio,
    required this.horaFim,
    required this.status,
    required this.podeCancelar,
    required this.pagamentoId,
    required this.valor,
    required this.statusPagamento,
    required this.podePagar,
  });

  factory Agendamento.doJson(Map<String, dynamic> json) => Agendamento(
        id: json['id'],
        quadra: json['quadra'],
        esporte: json['esporte'],
        data: DateTime.parse(json['data']),
        horaInicio: json['horaInicio'],
        horaFim: json['horaFim'],
        status: json['status'],
        podeCancelar: json['podeCancelar'],
        pagamentoId: json['pagamentoId'],
        valor: (json['valor'] as num).toDouble(),
        statusPagamento: json['statusPagamento'],
        podePagar: json['podePagar'],
      );
}

class Pix {
  final int pagamentoId;
  final double valor;
  final String codigo;
  final String status;
  final int segundosRestantes;

  Pix({required this.pagamentoId, required this.valor, required this.codigo, required this.status, required this.segundosRestantes});

  factory Pix.doJson(Map<String, dynamic> json) => Pix(
        pagamentoId: json['pagamentoId'],
        valor: (json['valor'] as num).toDouble(),
        codigo: json['codigo'],
        status: json['statusPagamento'],
        segundosRestantes: json['segundosRestantes'],
      );
}

// Guarda quem está logado enquanto o app está aberto.
class Sessao {
  static Usuario? usuario;
}

// Formatadores simples (sem biblioteca extra).
String formatarData(DateTime d) =>
    '${d.day.toString().padLeft(2, '0')}/${d.month.toString().padLeft(2, '0')}/${d.year}';

String formatarDataApi(DateTime d) =>
    '${d.year}-${d.month.toString().padLeft(2, '0')}-${d.day.toString().padLeft(2, '0')}';

String formatarDinheiro(double v) => 'R\$ ${v.toStringAsFixed(2).replaceAll('.', ',')}';
