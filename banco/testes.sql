-- Testes do banco (baseados nos testes da documentação do semestre anterior).
-- Rode depois do dados_exemplo.sql. Cada teste mostra o resultado esperado.
USE Sunset;
GO

PRINT '--- 1. Agendamento bem-sucedido (esperado: devolve o id do agendamento)';
EXEC CadastrarAgendamento @DataReserva='2026-12-15', @HoraInicio='18:00:00', @HoraFim='19:00:00',
     @ID_Usuario=3, @ID_Quadra=2, @ID_Pagamento=2;
GO

PRINT '--- 2. Outro horário na mesma quadra (esperado: sucesso)';
EXEC CadastrarAgendamento @DataReserva='2026-12-15', @HoraInicio='19:00:00', @HoraFim='20:00:00',
     @ID_Usuario=3, @ID_Quadra=2, @ID_Pagamento=2;
GO

PRINT '--- 3. Conflito de horário (esperado: erro "Esta quadra já possui um agendamento...")';
BEGIN TRY
    EXEC CadastrarAgendamento @DataReserva='2026-12-15', @HoraInicio='18:30:00', @HoraFim='19:30:00',
         @ID_Usuario=3, @ID_Quadra=2, @ID_Pagamento=2;
END TRY
BEGIN CATCH
    PRINT 'Erro recebido: ' + ERROR_MESSAGE();
END CATCH
GO

PRINT '--- 4. Hora final menor que a inicial (esperado: erro "O horário de término deve ser maior...")';
BEGIN TRY
    EXEC CadastrarAgendamento @DataReserva='2026-12-15', @HoraInicio='20:00:00', @HoraFim='19:00:00',
         @ID_Usuario=3, @ID_Quadra=2, @ID_Pagamento=2;
END TRY
BEGIN CATCH
    PRINT 'Erro recebido: ' + ERROR_MESSAGE();
END CATCH
GO

PRINT '--- 5. Usuário inexistente (esperado: erro "Erro de Vínculo...")';
BEGIN TRY
    EXEC CadastrarAgendamento @DataReserva='2026-12-15', @HoraInicio='21:00:00', @HoraFim='22:00:00',
         @ID_Usuario=999, @ID_Quadra=2, @ID_Pagamento=2;
END TRY
BEGIN CATCH
    PRINT 'Erro recebido: ' + ERROR_MESSAGE();
END CATCH
GO

PRINT '--- 6. Agendamento cancelado libera o horário (esperado: sucesso no segundo EXEC)';
UPDATE Agendamentos SET StatusReserva = 'Cancelado'
 WHERE ID_Quadra = 2 AND DataReserva = '2026-12-15' AND HoraInicio = '18:00:00';
EXEC CadastrarAgendamento @DataReserva='2026-12-15', @HoraInicio='18:00:00', @HoraFim='19:00:00',
     @ID_Usuario=3, @ID_Quadra=2, @ID_Pagamento=2;
GO

PRINT '--- 7. CPF com dígitos repetidos (esperado: erro da constraint CHK_cpf_repetido)';
BEGIN TRY
    INSERT INTO Usuario (NomeSobrenome, Email, Senha, CPF, Permissao, Genero, StatusUsuario)
    VALUES ('Testador Falha', 'falha.cpf@email.com', 'x', '22222222222', 'Cliente', 'M', 'Ativo');
END TRY
BEGIN CATCH
    PRINT 'Erro recebido: ' + ERROR_MESSAGE();
END CATCH
GO

PRINT '--- 8. E-mail sem @ (esperado: erro da constraint CHK_email_formato)';
BEGIN TRY
    INSERT INTO Usuario (NomeSobrenome, Email, Senha, CPF, Permissao, Genero, StatusUsuario)
    VALUES ('Erro Email', 'usuario_sem_email.com', 'y', '96385274100', 'Cliente', 'M', 'Ativo');
END TRY
BEGIN CATCH
    PRINT 'Erro recebido: ' + ERROR_MESSAGE();
END CATCH
GO

PRINT '--- 9. Excluir usuário sem ser Admin (esperado: "Ação negada: Apenas administradores...")';
BEGIN TRY
    EXEC sp_set_session_context 'ExecutorID', 3;   -- Bruno, que não é Admin
    DELETE FROM Usuario WHERE IDUsuario = 2;       -- Amanda não tem vínculos
END TRY
BEGIN CATCH
    PRINT 'Erro recebido: ' + ERROR_MESSAGE();
END CATCH
SELECT COUNT(*) AS 'Amanda ainda existe (esperado 1)' FROM Usuario WHERE IDUsuario = 2;
GO

PRINT '--- 10. Excluir usuário como Admin (esperado: Amanda é excluída)';
EXEC sp_set_session_context 'ExecutorID', 1;       -- admin@sunset.com
DELETE FROM Usuario WHERE IDUsuario = 2;
SELECT COUNT(*) AS 'Amanda ainda existe (esperado 0)' FROM Usuario WHERE IDUsuario = 2;
GO

PRINT '--- 11. Nome de quadra com caracteres especiais (esperado: erro da constraint CK_Quadras_Nome)';
BEGIN TRY
    INSERT INTO Quadras (NomeQuadra, PrecoHora, Categoria, ID_gestor, ID_endereco)
    VALUES ('Quadra de Beach Tennis ###', 90.00, 'Beach Tennis', 3, 2);
END TRY
BEGIN CATCH
    PRINT 'Erro recebido: ' + ERROR_MESSAGE();
END CATCH
GO

PRINT '--- 12. Trigger de log: excluir uma quadra sem vínculos grava o log';
INSERT INTO Quadras (NomeQuadra, PrecoHora, Categoria, ID_gestor, ID_endereco)
VALUES ('Quadra Temporaria', 50.00, 'Volei', NULL, 1);
DELETE FROM Quadras WHERE NomeQuadra = 'Quadra Temporaria';
SELECT * FROM LogExclusaoQuadras;   -- esperado: uma linha com 'Quadra Temporaria'
GO

PRINT '--- 13. Trigger trg_VirarGestor: Mariana e Bruno (esperado: Permissao = Gestor)';
SELECT IDUsuario, NomeSobrenome, Permissao FROM Usuario;
GO
