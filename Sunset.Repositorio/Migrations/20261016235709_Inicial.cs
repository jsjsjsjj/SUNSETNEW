using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sunset.Repositorio.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Enderecos",
                columns: table => new
                {
                    IDEndereco = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CEP = table.Column<string>(type: "varchar(8)", unicode: false, maxLength: 8, nullable: false),
                    Logradouro = table.Column<string>(type: "varchar(70)", unicode: false, maxLength: 70, nullable: false),
                    Numero = table.Column<string>(type: "varchar(5)", unicode: false, maxLength: 5, nullable: true),
                    Bairro = table.Column<string>(type: "varchar(40)", unicode: false, maxLength: 40, nullable: true),
                    Cidade = table.Column<string>(type: "varchar(40)", unicode: false, maxLength: 40, nullable: false),
                    Estado = table.Column<string>(type: "varchar(40)", unicode: false, maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enderecos", x => x.IDEndereco);
                    table.CheckConstraint("CK_Enderecos_Cidade", "Cidade NOT LIKE '%[^a-zA-Z0-9 ]%'");
                    table.CheckConstraint("CK_Enderecos_Estado", "Estado NOT LIKE '%[^a-zA-Z0-9 ]%'");
                    table.CheckConstraint("CK_Enderecos_Logradouro", "Logradouro NOT LIKE '%[^a-zA-Z0-9 ]%'");
                });

            migrationBuilder.CreateTable(
                name: "LogExclusaoQuadras",
                columns: table => new
                {
                    ID_Log = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Quadra = table.Column<int>(type: "int", nullable: false),
                    Nome_Quadra = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Usuario_Alteracao = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false, defaultValueSql: "SUSER_NAME()"),
                    Data_Alteracao = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogExclusaoQuadras", x => x.ID_Log);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    IDUsuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeSobrenome = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Senha = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    CPF = table.Column<string>(type: "char(11)", unicode: false, fixedLength: true, maxLength: 11, nullable: false),
                    Permissao = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    Genero = table.Column<string>(type: "char(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: true),
                    StatusUsuario = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.IDUsuario);
                    table.CheckConstraint("CHK_cpf_apenas_numeros", "CPF NOT LIKE '%[^0-9]%'");
                    table.CheckConstraint("CHK_cpf_repetido", "CPF NOT LIKE REPLICATE(SUBSTRING(CPF, 1, 1), 11)");
                    table.CheckConstraint("CHK_definir_admin", "(Permissao = 'Admin' AND Email LIKE '%@sunset.com') OR (Permissao IN ('Gestor', 'Cliente'))");
                    table.CheckConstraint("CHK_email_formato", "Email LIKE '%_@__%.__%'");
                    table.CheckConstraint("CK_Usuario_Genero", "Genero IN ('M', 'F')");
                    table.CheckConstraint("CK_Usuario_Nome", "NomeSobrenome NOT LIKE '%[^a-zA-Z0-9 ]%'");
                    table.CheckConstraint("CK_Usuario_Permissao", "Permissao IN ('Admin', 'Gestor', 'Cliente')");
                    table.CheckConstraint("CK_Usuario_Status", "StatusUsuario IN ('Inativo', 'Ativo', 'Bloqueado')");
                });

            migrationBuilder.CreateTable(
                name: "Quadras",
                columns: table => new
                {
                    IDQuadras = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeQuadra = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    PrecoHora = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    Categoria = table.Column<string>(type: "varchar(40)", unicode: false, maxLength: 40, nullable: false),
                    ID_endereco = table.Column<int>(type: "int", nullable: true),
                    ID_gestor = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quadras", x => x.IDQuadras);
                    table.CheckConstraint("CK_Quadras_Categoria", "Categoria IN ('Volei', 'Beach Tennis')");
                    table.CheckConstraint("CK_Quadras_Nome", "NomeQuadra NOT LIKE '%[^a-zA-Z0-9 ]%'");
                    table.ForeignKey(
                        name: "FK_Quadra_Usuario",
                        column: x => x.ID_gestor,
                        principalTable: "Usuario",
                        principalColumn: "IDUsuario");
                    table.ForeignKey(
                        name: "FK_Quadras_Enderecos",
                        column: x => x.ID_endereco,
                        principalTable: "Enderecos",
                        principalColumn: "IDEndereco");
                });

            migrationBuilder.CreateTable(
                name: "Pagamentos",
                columns: table => new
                {
                    IDPagamento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Valor = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    DataPagamento = table.Column<DateTime>(type: "datetime", nullable: false),
                    MetodoPagamento = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false, defaultValue: "PIX"),
                    StatusPagamento = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false, defaultValue: "Pendente"),
                    ID_quadra = table.Column<int>(type: "int", nullable: false),
                    ID_usuario = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagamentos", x => x.IDPagamento);
                    table.CheckConstraint("CHK_Status_Pagamento", "StatusPagamento IN ('Pendente', 'Aprovado', 'Cancelado')");
                    table.ForeignKey(
                        name: "FK_Quadras_Pagamentos",
                        column: x => x.ID_quadra,
                        principalTable: "Quadras",
                        principalColumn: "IDQuadras",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Usuario_Pagamentos",
                        column: x => x.ID_usuario,
                        principalTable: "Usuario",
                        principalColumn: "IDUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Agendamentos",
                columns: table => new
                {
                    IDAgendamento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataReserva = table.Column<DateTime>(type: "date", nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "time", nullable: false),
                    HoraFim = table.Column<TimeSpan>(type: "time", nullable: false),
                    StatusReserva = table.Column<string>(type: "varchar(40)", unicode: false, maxLength: 40, nullable: false, defaultValue: "Pendente"),
                    ID_Usuario = table.Column<int>(type: "int", nullable: false),
                    ID_Quadra = table.Column<int>(type: "int", nullable: false),
                    ID_Pagamento = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agendamentos", x => x.IDAgendamento);
                    table.CheckConstraint("CHK_Status_Reserva", "StatusReserva IN ('Em andamento', 'Finalizado', 'Pendente', 'Cancelado')");
                    table.CheckConstraint("VF_Horario_Agendamentos", "HoraFim > HoraInicio");
                    table.ForeignKey(
                        name: "FK_Pagamento_Agendamento",
                        column: x => x.ID_Pagamento,
                        principalTable: "Pagamentos",
                        principalColumn: "IDPagamento",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Quadra_Agendamento",
                        column: x => x.ID_Quadra,
                        principalTable: "Quadras",
                        principalColumn: "IDQuadras",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Usuario_Agendamento",
                        column: x => x.ID_Usuario,
                        principalTable: "Usuario",
                        principalColumn: "IDUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_ID_Pagamento",
                table: "Agendamentos",
                column: "ID_Pagamento");

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_ID_Quadra",
                table: "Agendamentos",
                column: "ID_Quadra");

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_ID_Usuario",
                table: "Agendamentos",
                column: "ID_Usuario");

            migrationBuilder.CreateIndex(
                name: "IX_Pagamentos_ID_quadra",
                table: "Pagamentos",
                column: "ID_quadra");

            migrationBuilder.CreateIndex(
                name: "IX_Pagamentos_ID_usuario",
                table: "Pagamentos",
                column: "ID_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_Quadras_ID_endereco",
                table: "Quadras",
                column: "ID_endereco");

            migrationBuilder.CreateIndex(
                name: "IX_Quadras_ID_gestor",
                table: "Quadras",
                column: "ID_gestor");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_CPF",
                table: "Usuario",
                column: "CPF",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_Email",
                table: "Usuario",
                column: "Email",
                unique: true);

            // ---------- Escrito à mão: o EF não gera trigger nem procedure ----------
            // Colocar o SQL na migration faz o "dotnet ef database update" criar tudo
            // de uma vez, em qualquer máquina, sem rodar script no SSMS.

            // Trigger 1: quando uma quadra é excluída, grava quem excluiu e quando.
            migrationBuilder.Sql(@"
CREATE TRIGGER trg_LogExclusaoQuadras
ON Quadras
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO LogExclusaoQuadras (ID_Quadra, Nome_Quadra, Usuario_Alteracao, Data_Alteracao)
    SELECT d.IDQuadras, d.NomeQuadra, SYSTEM_USER, GETDATE()
      FROM deleted d;
END");

            // Trigger 2: quem cadastra uma quadra e ainda é Cliente vira Gestor.
            migrationBuilder.Sql(@"
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
END");

            // Trigger 3: só o Admin da sessão (ExecutorID) pode excluir usuários.
            // Quem exclui precisa rodar antes: EXEC sp_set_session_context 'ExecutorID', <id do admin>
            migrationBuilder.Sql(@"
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
END");

            // Procedure: cria o agendamento sem deixar dois horários se cruzarem na mesma quadra.
            // Chamada pelo AgendamentoRepositorio, com Dapper.
            migrationBuilder.Sql(@"
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
END");

            // Administrador inicial (e-mail @sunset.com, como o banco exige).
            // Senha de teste: Admin@1234 (aqui já em SHA256).
            migrationBuilder.Sql(@"
INSERT INTO Usuario (NomeSobrenome, Email, Senha, CPF, Permissao, StatusUsuario)
VALUES ('Administrador Sunset', 'admin@sunset.com', 'bc78e58d55cde1346e68f8e5fe588dedf62fa457aa646a500a53347faff6ee24', '39053344705', 'Admin', 'Ativo')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS CadastrarAgendamento");
            // As triggers são apagadas junto com as tabelas.

            migrationBuilder.DropTable(
                name: "Agendamentos");

            migrationBuilder.DropTable(
                name: "LogExclusaoQuadras");

            migrationBuilder.DropTable(
                name: "Pagamentos");

            migrationBuilder.DropTable(
                name: "Quadras");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "Enderecos");
        }
    }
}
