IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Enderecos] (
    [IDEndereco] int NOT NULL IDENTITY,
    [CEP] varchar(8) NOT NULL,
    [Logradouro] varchar(70) NOT NULL,
    [Numero] varchar(5) NULL,
    [Bairro] varchar(40) NULL,
    [Cidade] varchar(40) NOT NULL,
    [Estado] varchar(40) NOT NULL,
    CONSTRAINT [PK_Enderecos] PRIMARY KEY ([IDEndereco]),
    CONSTRAINT [CK_Enderecos_Cidade] CHECK (Cidade NOT LIKE '%[^a-zA-Z0-9 ]%'),
    CONSTRAINT [CK_Enderecos_Estado] CHECK (Estado NOT LIKE '%[^a-zA-Z0-9 ]%'),
    CONSTRAINT [CK_Enderecos_Logradouro] CHECK (Logradouro NOT LIKE '%[^a-zA-Z0-9 ]%')
);

CREATE TABLE [LogExclusaoQuadras] (
    [ID_Log] int NOT NULL IDENTITY,
    [ID_Quadra] int NOT NULL,
    [Nome_Quadra] varchar(100) NOT NULL,
    [Usuario_Alteracao] varchar(100) NOT NULL DEFAULT (SUSER_NAME()),
    [Data_Alteracao] datetime NOT NULL DEFAULT (GETDATE()),
    CONSTRAINT [PK_LogExclusaoQuadras] PRIMARY KEY ([ID_Log])
);

CREATE TABLE [Usuario] (
    [IDUsuario] int NOT NULL IDENTITY,
    [NomeSobrenome] varchar(100) NOT NULL,
    [Email] varchar(100) NOT NULL,
    [Senha] varchar(255) NOT NULL,
    [CPF] char(11) NOT NULL,
    [Permissao] varchar(20) NOT NULL,
    [Genero] char(1) NULL,
    [StatusUsuario] varchar(15) NOT NULL,
    CONSTRAINT [PK_Usuario] PRIMARY KEY ([IDUsuario]),
    CONSTRAINT [CHK_cpf_apenas_numeros] CHECK (CPF NOT LIKE '%[^0-9]%'),
    CONSTRAINT [CHK_cpf_repetido] CHECK (CPF NOT LIKE REPLICATE(SUBSTRING(CPF, 1, 1), 11)),
    CONSTRAINT [CHK_definir_admin] CHECK ((Permissao = 'Admin' AND Email LIKE '%@sunset.com') OR (Permissao IN ('Gestor', 'Cliente'))),
    CONSTRAINT [CHK_email_formato] CHECK (Email LIKE '%_@__%.__%'),
    CONSTRAINT [CK_Usuario_Genero] CHECK (Genero IN ('M', 'F')),
    CONSTRAINT [CK_Usuario_Nome] CHECK (NomeSobrenome NOT LIKE '%[^a-zA-Z0-9 ]%'),
    CONSTRAINT [CK_Usuario_Permissao] CHECK (Permissao IN ('Admin', 'Gestor', 'Cliente')),
    CONSTRAINT [CK_Usuario_Status] CHECK (StatusUsuario IN ('Inativo', 'Ativo', 'Bloqueado'))
);

CREATE TABLE [Quadras] (
    [IDQuadras] int NOT NULL IDENTITY,
    [NomeQuadra] varchar(50) NOT NULL,
    [PrecoHora] decimal(8,2) NOT NULL,
    [Categoria] varchar(40) NOT NULL,
    [ID_endereco] int NULL,
    [ID_gestor] int NULL,
    CONSTRAINT [PK_Quadras] PRIMARY KEY ([IDQuadras]),
    CONSTRAINT [CK_Quadras_Categoria] CHECK (Categoria IN ('Volei', 'Beach Tennis')),
    CONSTRAINT [CK_Quadras_Nome] CHECK (NomeQuadra NOT LIKE '%[^a-zA-Z0-9 ]%'),
    CONSTRAINT [FK_Quadra_Usuario] FOREIGN KEY ([ID_gestor]) REFERENCES [Usuario] ([IDUsuario]),
    CONSTRAINT [FK_Quadras_Enderecos] FOREIGN KEY ([ID_endereco]) REFERENCES [Enderecos] ([IDEndereco])
);

CREATE TABLE [Pagamentos] (
    [IDPagamento] int NOT NULL IDENTITY,
    [Valor] decimal(10,2) NOT NULL,
    [DataPagamento] datetime NOT NULL,
    [MetodoPagamento] varchar(20) NOT NULL DEFAULT 'PIX',
    [StatusPagamento] varchar(20) NOT NULL DEFAULT 'Pendente',
    [ID_quadra] int NOT NULL,
    [ID_usuario] int NOT NULL,
    CONSTRAINT [PK_Pagamentos] PRIMARY KEY ([IDPagamento]),
    CONSTRAINT [CHK_Status_Pagamento] CHECK (StatusPagamento IN ('Pendente', 'Aprovado', 'Cancelado')),
    CONSTRAINT [FK_Quadras_Pagamentos] FOREIGN KEY ([ID_quadra]) REFERENCES [Quadras] ([IDQuadras]),
    CONSTRAINT [FK_Usuario_Pagamentos] FOREIGN KEY ([ID_usuario]) REFERENCES [Usuario] ([IDUsuario])
);

CREATE TABLE [Agendamentos] (
    [IDAgendamento] int NOT NULL IDENTITY,
    [DataReserva] date NOT NULL,
    [HoraInicio] time NOT NULL,
    [HoraFim] time NOT NULL,
    [StatusReserva] varchar(40) NOT NULL DEFAULT 'Pendente',
    [ID_Usuario] int NOT NULL,
    [ID_Quadra] int NOT NULL,
    [ID_Pagamento] int NOT NULL,
    CONSTRAINT [PK_Agendamentos] PRIMARY KEY ([IDAgendamento]),
    CONSTRAINT [CHK_Status_Reserva] CHECK (StatusReserva IN ('Em andamento', 'Finalizado', 'Pendente', 'Cancelado')),
    CONSTRAINT [VF_Horario_Agendamentos] CHECK (HoraFim > HoraInicio),
    CONSTRAINT [FK_Pagamento_Agendamento] FOREIGN KEY ([ID_Pagamento]) REFERENCES [Pagamentos] ([IDPagamento]),
    CONSTRAINT [FK_Quadra_Agendamento] FOREIGN KEY ([ID_Quadra]) REFERENCES [Quadras] ([IDQuadras]),
    CONSTRAINT [FK_Usuario_Agendamento] FOREIGN KEY ([ID_Usuario]) REFERENCES [Usuario] ([IDUsuario])
);

CREATE INDEX [IX_Agendamentos_ID_Pagamento] ON [Agendamentos] ([ID_Pagamento]);

CREATE INDEX [IX_Agendamentos_ID_Quadra] ON [Agendamentos] ([ID_Quadra]);

CREATE INDEX [IX_Agendamentos_ID_Usuario] ON [Agendamentos] ([ID_Usuario]);

CREATE INDEX [IX_Pagamentos_ID_quadra] ON [Pagamentos] ([ID_quadra]);

CREATE INDEX [IX_Pagamentos_ID_usuario] ON [Pagamentos] ([ID_usuario]);

CREATE INDEX [IX_Quadras_ID_endereco] ON [Quadras] ([ID_endereco]);

CREATE INDEX [IX_Quadras_ID_gestor] ON [Quadras] ([ID_gestor]);

CREATE UNIQUE INDEX [IX_Usuario_CPF] ON [Usuario] ([CPF]);

CREATE UNIQUE INDEX [IX_Usuario_Email] ON [Usuario] ([Email]);


CREATE TRIGGER trg_LogExclusaoQuadras
ON Quadras
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO LogExclusaoQuadras (ID_Quadra, Nome_Quadra, Usuario_Alteracao, Data_Alteracao)
    SELECT d.IDQuadras, d.NomeQuadra, SYSTEM_USER, GETDATE()
      FROM deleted d;
END


CREATE TRIGGER trg_VirarGestor
ON Quadras
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Usuario
       SET Permissao = 'Gestor'
      FROM Usuario
      INNER JOIN inserted i ON Usuario.IDUsuario = i.ID_gestor
     WHERE Usuario.Permissao = 'Cliente';
END


CREATE TRIGGER trg_ExclusaoSomenteAdmin
ON Usuario
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM Usuario
                    WHERE Permissao = 'Admin'
                      AND IDUsuario = CAST(SESSION_CONTEXT(N'ExecutorID') AS INT))
    BEGIN
        RAISERROR ('Ação negada: Apenas administradores podem realizar essa função', 16, 1);
        RETURN;
    END

    DELETE FROM Usuario WHERE IDUsuario IN (SELECT IDUsuario FROM deleted);
END


CREATE PROCEDURE CadastrarAgendamento
    @DataReserva DATE,
    @HoraInicio TIME,
    @HoraFim TIME,
    @ID_Usuario INT,
    @ID_Quadra INT,
    @ID_Pagamento INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF @HoraFim <= @HoraInicio
        BEGIN
            RAISERROR('Erro: O horário de término deve ser maior que o horário de início.', 16, 1);
        END

        BEGIN TRANSACTION;

        -- Dois horários se cruzam quando o novo começa antes do outro terminar
        -- e termina depois do outro começar. Agendamento cancelado não conta.
        -- UPDLOCK + HOLDLOCK trava a consulta para dois clientes não reservarem ao mesmo tempo.
        IF EXISTS (
            SELECT 1
              FROM Agendamentos WITH (UPDLOCK, HOLDLOCK)
             WHERE ID_Quadra = @ID_Quadra
               AND DataReserva = @DataReserva
               AND StatusReserva <> 'Cancelado'
               AND @HoraInicio < HoraFim
               AND @HoraFim > HoraInicio
        )
        BEGIN
            RAISERROR('Erro: Esta quadra já possui um agendamento para o horário selecionado.', 16, 2);
        END

        INSERT INTO Agendamentos (DataReserva, HoraInicio, HoraFim, ID_Usuario, ID_Quadra, ID_Pagamento)
        VALUES (@DataReserva, @HoraInicio, @HoraFim, @ID_Usuario, @ID_Quadra, @ID_Pagamento);

        DECLARE @IdNovo INT = CAST(SCOPE_IDENTITY() AS INT);

        COMMIT TRANSACTION;

        -- O C# lê este número para saber qual agendamento foi criado.
        SELECT @IdNovo AS IdAgendamento;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

        -- 547 = violação de chave estrangeira (usuário, quadra ou pagamento não existe).
        IF ERROR_NUMBER() = 547
            RAISERROR('Erro de Vínculo: O Usuário, Quadra ou Pagamento informado não existe.', 16, 3);
        ELSE
            THROW;  -- repassa o erro original (por exemplo, o de conflito de horário)
    END CATCH
END


INSERT INTO Usuario (NomeSobrenome, Email, Senha, CPF, Permissao, StatusUsuario)
VALUES ('Administrador Sunset', 'admin@sunset.com', 'bc78e58d55cde1346e68f8e5fe588dedf62fa457aa646a500a53347faff6ee24', '39053344705', 'Admin', 'Ativo')

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20261016235709_Inicial', N'9.0.21');

COMMIT;
GO

