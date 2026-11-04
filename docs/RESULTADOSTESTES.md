# Resultados dos testes do SUNSET



| Parte | Como foi testado | Resultado |
|---|---|---|
| Migration e banco | `dotnet ef database update` | Criou as tabelas, as triggers e a procedure. O erro de cascata em `Agendamentos` foi corrigido (chaves sem cascata) |
| Banco | `banco/testes.sql` (13 testes), com o banco limpo | Resultados esperados conferidos |
| Regras de negócio | Programa de teste local com repositórios de mentira | Passou |
| Api | Chamadas reais contra o SQL Server (login, quadras, horários, reserva, Pix e cancelamento) | Passou |
| Site | Navegador, nos perfis Cliente, Gestor e Admin | Passou |
| Relatório do gestor | Programa de teste local: só as quadras do gestor, filtro de período, Admin vê todas e gestor sem quadra | Passou (7 testes) |
| Relatório TXT na tela | Site rodando no GitHub Codespaces com SQL Server: pagamento de uma reserva, botão de baixar e gestores diferentes | Passou. O relatório atualizou com o pagamento e cada gestor viu só as quadras dele |
| Exclusão de usuário pelo Admin | Codespaces com SQL Server: usuário com agendamentos e usuário sem vínculos | Passou. O usuário com vínculos foi recusado com mensagem e o outro foi excluído |
| App Flutter | Chrome contra a Api real, `flutter analyze` e `flutter test` | Passou. Um erro de `setState` foi achado e corrigido |
| Responsividade | Chrome em 1280px e 390px, todas as telas do site nos três perfis e o app em 390px, com dados em memória | Sem rolagem lateral em nenhuma tela. As tabelas viram cartões no celular. O banco real não entrou nesse teste |

Os prints das telas estão em `prints/` (computador, celular e app). Eles foram tirados com dados em memória, sem SQL Server, usando os mesmos controllers, views e regras de negócio. O relatório do gestor não tem print porque baixa um arquivo TXT.
