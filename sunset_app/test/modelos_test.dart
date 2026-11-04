import 'package:flutter_test/flutter_test.dart';
import 'package:sunset_app/modelos.dart';

void main() {
  test('formata data e dinheiro', () {
    expect(formatarData(DateTime(2026, 12, 5)), '05/12/2026');
    expect(formatarDataApi(DateTime(2026, 12, 5)), '2026-12-05');
    expect(formatarDinheiro(150), 'R\$ 150,00');
  });

  test('lê o JSON de um agendamento da Api', () {
    final a = Agendamento.doJson({
      'id': 1, 'quadra': 'Arena', 'esporte': 'Volei', 'data': '2026-12-20T00:00:00',
      'horaInicio': '18:00', 'horaFim': '19:00', 'status': 'Pendente', 'podeCancelar': true,
      'pagamentoId': 4, 'valor': 150, 'statusPagamento': 'Pendente', 'podePagar': true,
    });
    expect(a.data.day, 20);
    expect(a.valor, 150.0);
    expect(a.podePagar, true);
  });

  test('horário só é livre se não está ocupado nem encerrado', () {
    expect(Horario(inicio: '10:00', fim: '11:00', ocupado: false, passado: false).livre, true);
    expect(Horario(inicio: '10:00', fim: '11:00', ocupado: true, passado: false).livre, false);
    expect(Horario(inicio: '10:00', fim: '11:00', ocupado: false, passado: true).livre, false);
  });
}
