-- Dados de exemplo para testar o SUNSET. Rode DEPOIS de criar o banco (dotnet ef database update).
-- Senha de todos os usuários de exemplo: Senha1234 (guardada em SHA256).
-- O administrador (admin@sunset.com, senha Admin@1234) já é criado pela migration, com IDUsuario = 1.
USE Sunset;
GO

INSERT INTO Usuario (NomeSobrenome, Email, Senha, CPF, Permissao, Genero, StatusUsuario) VALUES
('Amanda Rodrigues',     'amanda.r@gmail.com',        'd5b7fcc641a62b9683fc93414343ea66f959aa0e999eb8ec8554b861460d754b', '98765432100', 'Cliente', 'F', 'Ativo'),
('Bruno Oliveira Santos','bruno.os@outlook.com',      'd5b7fcc641a62b9683fc93414343ea66f959aa0e999eb8ec8554b861460d754b', '45678912311', 'Cliente', 'M', 'Ativo'),
('Mariana Costa',        'mariana.costa@hotmail.com', 'd5b7fcc641a62b9683fc93414343ea66f959aa0e999eb8ec8554b861460d754b', '74185296300', 'Cliente', 'F', 'Ativo');
GO

INSERT INTO Enderecos (CEP, Logradouro, Numero, Bairro, Cidade, Estado) VALUES
('14020250', 'Avenida Joao Fiusa', '1500', 'Jardim Botanico', 'Ribeirao Preto', 'SP'),
('14050000', 'Rua Sao Sebastiao',  '342',  'Centro',          'Ribeirao Preto', 'SP');
GO

-- Mariana (ID 4) e Bruno (ID 3) viram Gestores sozinhos, pela trigger trg_VirarGestor.
INSERT INTO Quadras (NomeQuadra, PrecoHora, Categoria, ID_gestor, ID_endereco) VALUES
('Arena Sunset Society 1',   150.00, 'Volei',        4, 1),
('Quadra de Beach Tennis A',  90.00, 'Beach Tennis', 3, 2);
GO

-- Pagamentos do Bruno (ID 3). O ID_usuario é obrigatório.
INSERT INTO Pagamentos (Valor, StatusPagamento, DataPagamento, ID_quadra, ID_usuario) VALUES
(150.00, 'Aprovado', GETDATE(), 1, 3),
( 90.00, 'Pendente', GETDATE(), 2, 3);
GO
