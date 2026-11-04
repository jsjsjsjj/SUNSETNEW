# SUNSET - aplicativo Flutter

App do cliente que consome a Web API do projeto (`Sunset.Api`). Não tem regra de negócio própria: tudo passa pela Api.

## O que faz

- Login por e-mail e senha (`POST /api/usuarios/login`).
- **Quadras**: lista as quadras (`GET /api/quadras`), escolhe o dia e vê os horários livres e ocupados (`GET /api/quadras/{id}/horarios`).
- **Reservar** 1 hora (`POST /api/agendamentos/reservar`) e pagar com o **Pix simulado**, com QR de exemplo e relógio de 5 minutos (`GET /api/pagamentos/{id}/pix` e `POST /api/pagamentos/{id}/simular`).
- **Meus agendamentos** (funcionalidade complementar ao sistema): lista os agendamentos do cliente (`GET /api/agendamentos?usuarioId=`), permite cancelar (`POST /api/agendamentos/{id}/cancelar`) e pagar um Pix pendente.

## Arquivos

- `lib/main.dart`: abre o app.
- `lib/servico_api.dart`: todas as chamadas HTTP (pacote `http`) e o tratamento de erro da Api.
- `lib/modelos.dart`: classes que representam o JSON (iguais aos DTOs do C#) e a sessão do usuário logado.
- `lib/telas/`: `login_tela`, `home_tela` (abas), `quadras_tela`, `horarios_tela`, `pix_tela`, `agendamentos_tela`.

## Como rodar

1. Suba a Api: `dotnet run --project Sunset.Api` (porta `5218`).
2. Na pasta `sunset_app`: `flutter pub get`.
3. No navegador: `flutter run -d chrome`.
4. No emulador Android: `flutter run` (o app usa `10.0.2.2:5218`, que é o PC visto pelo emulador). O endereço fica em `ServicoApi.urlBase`.

Login de teste (depois de rodar `banco/dados_exemplo.sql`): `bruno.os@outlook.com` / `Senha1234`.

A Api libera CORS (`app.UseCors()` no `Program.cs`) para o app rodando no navegador conseguir chamá-la.
