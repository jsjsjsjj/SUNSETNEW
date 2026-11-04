# SUNSET - Sistema de agendamento de quadras

O SUNSET nasceu de uma dor de mercado quase invisível, mas enorme. Os grandes complexos de quadras têm (ou desenvolvem) os seus próprios sistemas de gerenciamento, e o pequeno empreendedor fica de fora. Nossa plataforma tem apenas um objetivo: levar as mesmas facilidades dos grandes complexos para quem não tem tantos recursos e tempo, sem precisar carregar caneta, corretivo e uma agenda no bolso.

Este repositório é o projeto do PIM IV (Análise e Desenvolvimento de Sistemas, UNIP). Nele nós modernizamos o protótipo do semestre anterior, que era um programa de console com arquivos de texto, e criamos o sistema completo: banco de dados SQL Server, site (ASP.NET Core MVC), Web API e aplicativo em Flutter. As regras de negócio do semestre anterior foram mantidas.

## Integrantes

- R9555I7 - Nicholas Jones Teixeira
- H000IF9 - Leonardo Manini de Souza
- H786DE8 - Pedro Geordane Melão Rodrigues
- R4185I1 - Vinicius Fortunato Freitas
- H0102F3 - Pedro Henrique de Paiva
- R859282 - Patrick Pelegi Mariano

## O que o sistema faz

Cliente:
- Cria a conta e faz login com o email
- Escolhe o esporte (Vôlei ou Beach Tennis), a quadra, o dia e o horário
- Paga o Pix (simulado) em até 5 minutos, com QR code e cronômetro
- Vê os seus agendamentos e cancela quando precisar (o horário volta a ficar livre)

Gestor (dono de quadra):
- Cadastra, edita e exclui as quadras dele
- Vê a agenda dos próximos 5 dias
- Cancela agendamentos da quadra dele
- Baixa o relatório das quadras dele em TXT, escolhendo o período

Administrador:
- Vê todos os agendamentos e filtra por nome ou data
- Exclui agendamentos e usuários
- Baixa o relatório geral em TXT

O site tem o VLibras (tradução para LIBRAS) e se ajusta a computador, tablet e celular.

## Tecnologias

- C# com .NET 9
- SQL Server 2022 Express (instância SQLEXPRESS)
- Entity Framework Core 9 (consultas, cadastros e migrations) e Dapper (chama a procedure)
- ASP.NET Core MVC com Razor e Bootstrap (site)
- ASP.NET Core Web API com OpenAPI e Scalar
- Flutter (aplicativo)
- VLibras (acessibilidade)

## Estrutura do projeto

A solution `Sunset.sln` tem cinco projetos. Cada um só conhece o projeto logo abaixo dele:


Sunset.Web  ──┐
              ├──► Sunset.Negocio ──► Sunset.Repositorio ──► Sunset.Modelos
Sunset.Api  ──┘


- `Sunset.Modelos`: entidades, DTOs e constantes (perfis e status)
- `Sunset.Repositorio`: acesso ao banco (EF Core e Dapper) e migrations
- `Sunset.Negocio`: regras de negócio (cadastro, login, CPF, SHA256, reserva, cancelamento, Pix e relatório)
- `Sunset.Web`: site MVC
- `Sunset.Api`: Web API REST
- `sunset_app/`: aplicativo Flutter, que consome a Web API
- `banco/`: `script_banco.sql`, `dados_exemplo.sql` e `testes.sql`
- `docs/`: documentação do PIM, roteiros de apresentação e resultados dos testes
- `prints/`: prints das telas (computador, celular e aplicativo)
- `iniciar-sistema/`: scripts para subir tudo no Windows

A Web e a Api não acessam o banco direto. Todo acesso passa pela camada de Negócio, e as dependências são registradas por injeção de dependência no `Program.cs` (explicado em `INJECAO-DE-DEPENDENCIA.md`). A connection string fica só no `appsettings.json`.

## Como rodar

O que precisa estar instalado: .NET SDK (o projeto é net9.0), SQL Server Express com a instância `SQLEXPRESS`, Git e, só para o aplicativo, o Flutter.

1. Criar o banco, na pasta da solution:

```
dotnet restore
dotnet ef database update --project Sunset.Repositorio --startup-project Sunset.Api
```

Se o seu SQL Server tiver outro nome, mude a connection string em `Sunset.Api/appsettings.json` e em `Sunset.Web/appsettings.json`.

2. (Opcional) Dados de exemplo, no SSMS ou no Azure Data Studio: rodar `banco/dados_exemplo.sql`.

3. Subir a Api (porta 5218):

```
dotnet run --project Sunset.Api
```

A documentação interativa fica em http://localhost:5218/scalar

4. Subir o site (porta 5157), em outro terminal:

```
dotnet run --project Sunset.Web
```

Acesse http://localhost:5157

5. Aplicativo Flutter, com a Api rodando:

```
cd sunset_app
flutter pub get
flutter run -d chrome
```

No emulador Android o app usa o endereço `10.0.2.2:5218` (o PC visto pelo emulador). Esse endereço fica em `ServicoApi.urlBase`.

No Windows também dá para dar duplo clique em `iniciar-sistema/iniciar_tudo.bat`, que cria o banco, insere os dados de exemplo e abre a Api, o site e o app. Esses scripts não foram testados no computador de todos do grupo. Se algum passo falhar, a mensagem aparece na janela e os comandos acima resolvem.

## Logins para teste

- Administrador: `admin@sunset.com` / `Admin@1234`
- Clientes e gestor (depois de rodar `dados_exemplo.sql`): `amanda.r@gmail.com`, `bruno.os@outlook.com` e `mariana.costa@hotmail.com`, todos com a senha `Senha1234`

## Banco de dados

Tabelas: Usuario, Quadras, Enderecos, Pagamentos, Agendamentos e LogExclusaoQuadras. A migration `Inicial` cria tudo, incluindo:

- `trg_LogExclusaoQuadras`: grava no log quem excluiu uma quadra e quando
- `trg_VirarGestor`: o usuário que recebe uma quadra vira Gestor
- `trg_ExclusaoSomenteAdmin`: só o Admin exclui usuários (o sistema informa quem está excluindo pelo `ExecutorID` da sessão)
- `CadastrarAgendamento`: procedure que confere o conflito de horário, com transação e `UPDLOCK`/`HOLDLOCK`. O sistema chama a procedure pelo Dapper

As chaves estrangeiras não têm exclusão em cascata. O arquivo `banco/testes.sql` tem 13 testes (rodar só com o banco limpo).

## Testes

Os resultados estão em `docs/RESULTADOSTESTES.md`. Passaram: migration e banco, regras de negócio (programa de teste local), Api com o SQL Server, site nos três perfis, responsividade e aplicativo no navegador (`flutter analyze` e `flutter test`).

## Limitações

- A Api não usa token (JWT): o aplicativo manda o id do usuário nas chamadas
- O Pix é simulado, e o QR code é uma imagem de exemplo
- Cancelar uma reserva já paga não faz estorno
- Não implementamos Machine Learning. A análise de dados é o relatório em TXT
- O VLibras está só no site, não no aplicativo

