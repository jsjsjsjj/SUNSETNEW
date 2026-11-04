using Microsoft.EntityFrameworkCore;
using Sunset.Modelos;

namespace Sunset.Repositorio.Contexto;

// O DbContext é a "porta" do Entity Framework Core para o banco.
// Cada DbSet vira uma tabela. Os nomes de tabelas e colunas abaixo seguem
// o dicionário de dados da documentação do semestre anterior.
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Endereco> Enderecos => Set<Endereco>();
    public DbSet<Quadra> Quadras => Set<Quadra>();
    public DbSet<Pagamento> Pagamentos => Set<Pagamento>();
    public DbSet<Agendamento> Agendamentos => Set<Agendamento>();
    public DbSet<LogExclusaoQuadra> LogsExclusaoQuadra => Set<LogExclusaoQuadra>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ---------- Usuario ----------
        modelBuilder.Entity<Usuario>(e =>
        {
            e.ToTable("Usuario", t =>
            {
                // Tabela com trigger: o EF Core 7+ exige o HasTrigger, senão dá erro ao gravar.
                t.HasTrigger("trg_ExclusaoSomenteAdmin");

                t.HasCheckConstraint("CK_Usuario_Nome", "NomeSobrenome NOT LIKE '%[^a-zA-Z0-9 ]%'");
                t.HasCheckConstraint("CHK_email_formato", "Email LIKE '%_@__%.__%'");
                t.HasCheckConstraint("CHK_cpf_repetido", "CPF NOT LIKE REPLICATE(SUBSTRING(CPF, 1, 1), 11)");
                t.HasCheckConstraint("CHK_cpf_apenas_numeros", "CPF NOT LIKE '%[^0-9]%'");
                t.HasCheckConstraint("CHK_definir_admin",
                    "(Permissao = 'Admin' AND Email LIKE '%@sunset.com') OR (Permissao IN ('Gestor', 'Cliente'))");
                t.HasCheckConstraint("CK_Usuario_Permissao", "Permissao IN ('Admin', 'Gestor', 'Cliente')");
                t.HasCheckConstraint("CK_Usuario_Genero", "Genero IN ('M', 'F')");
                t.HasCheckConstraint("CK_Usuario_Status", "StatusUsuario IN ('Inativo', 'Ativo', 'Bloqueado')");
            });

            e.HasKey(u => u.Id);
            e.Property(u => u.Id).HasColumnName("IDUsuario");
            e.Property(u => u.NomeSobrenome).HasMaxLength(100).IsUnicode(false);
            e.Property(u => u.Email).HasMaxLength(100).IsUnicode(false);
            e.Property(u => u.Senha).HasMaxLength(255).IsUnicode(false);
            e.Property(u => u.Cpf).HasColumnName("CPF").HasMaxLength(11).IsFixedLength().IsUnicode(false);
            e.Property(u => u.Permissao).HasMaxLength(20).IsUnicode(false);
            e.Property(u => u.Genero).HasMaxLength(1).IsFixedLength().IsUnicode(false);
            e.Property(u => u.StatusUsuario).HasMaxLength(15).IsUnicode(false);

            e.HasIndex(u => u.Email).IsUnique();
            e.HasIndex(u => u.Cpf).IsUnique();
        });

        // ---------- Enderecos ----------
        modelBuilder.Entity<Endereco>(e =>
        {
            e.ToTable("Enderecos", t =>
            {
                t.HasCheckConstraint("CK_Enderecos_Logradouro", "Logradouro NOT LIKE '%[^a-zA-Z0-9 ]%'");
                t.HasCheckConstraint("CK_Enderecos_Cidade", "Cidade NOT LIKE '%[^a-zA-Z0-9 ]%'");
                t.HasCheckConstraint("CK_Enderecos_Estado", "Estado NOT LIKE '%[^a-zA-Z0-9 ]%'");
            });

            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("IDEndereco");
            e.Property(x => x.Cep).HasColumnName("CEP").HasMaxLength(8).IsUnicode(false);
            e.Property(x => x.Logradouro).HasMaxLength(70).IsUnicode(false);
            e.Property(x => x.Numero).HasMaxLength(5).IsUnicode(false);
            e.Property(x => x.Bairro).HasMaxLength(40).IsUnicode(false);
            e.Property(x => x.Cidade).HasMaxLength(40).IsUnicode(false);
            e.Property(x => x.Estado).HasMaxLength(40).IsUnicode(false);
        });

        // ---------- Quadras ----------
        modelBuilder.Entity<Quadra>(e =>
        {
            e.ToTable("Quadras", t =>
            {
                // A tabela tem duas triggers: log de exclusão e promoção a gestor.
                t.HasTrigger("trg_LogExclusaoQuadras");
                t.HasTrigger("trg_VirarGestor");

                t.HasCheckConstraint("CK_Quadras_Nome", "NomeQuadra NOT LIKE '%[^a-zA-Z0-9 ]%'");
                t.HasCheckConstraint("CK_Quadras_Categoria", "Categoria IN ('Volei', 'Beach Tennis')");
            });

            e.HasKey(q => q.Id);
            e.Property(q => q.Id).HasColumnName("IDQuadras");
            e.Property(q => q.NomeQuadra).HasMaxLength(50).IsUnicode(false);
            e.Property(q => q.PrecoHora).HasColumnType("decimal(8,2)");
            e.Property(q => q.Categoria).HasMaxLength(40).IsUnicode(false);
            e.Property(q => q.EnderecoId).HasColumnName("ID_endereco");
            e.Property(q => q.GestorId).HasColumnName("ID_gestor");

            e.HasOne(q => q.Endereco).WithMany().HasForeignKey(q => q.EnderecoId)
                .HasConstraintName("FK_Quadras_Enderecos");
            e.HasOne(q => q.Gestor).WithMany().HasForeignKey(q => q.GestorId)
                .HasConstraintName("FK_Quadra_Usuario");
        });

        // ---------- Pagamentos ----------
        modelBuilder.Entity<Pagamento>(e =>
        {
            e.ToTable("Pagamentos", t =>
            {
                t.HasCheckConstraint("CHK_Status_Pagamento", "StatusPagamento IN ('Pendente', 'Aprovado', 'Cancelado')");
            });

            e.HasKey(p => p.Id);
            e.Property(p => p.Id).HasColumnName("IDPagamento");
            e.Property(p => p.Valor).HasColumnType("decimal(10,2)");
            e.Property(p => p.DataPagamento).HasColumnType("datetime");
            e.Property(p => p.MetodoPagamento).HasMaxLength(20).IsUnicode(false).HasDefaultValue("PIX");
            e.Property(p => p.StatusPagamento).HasMaxLength(20).IsUnicode(false).HasDefaultValue("Pendente");
            e.Property(p => p.QuadraId).HasColumnName("ID_quadra");
            e.Property(p => p.UsuarioId).HasColumnName("ID_usuario");

            e.HasOne<Quadra>().WithMany().HasForeignKey(p => p.QuadraId)
                .OnDelete(DeleteBehavior.Restrict) // sem cascata: o banco recusa excluir quem tem registros ligados
                .HasConstraintName("FK_Quadras_Pagamentos");
            e.HasOne<Usuario>().WithMany().HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict) // sem cascata: o banco recusa excluir quem tem registros ligados
                .HasConstraintName("FK_Usuario_Pagamentos");
        });

        // ---------- Agendamentos ----------
        modelBuilder.Entity<Agendamento>(e =>
        {
            e.ToTable("Agendamentos", t =>
            {
                t.HasCheckConstraint("VF_Horario_Agendamentos", "HoraFim > HoraInicio");
                // 'Cancelado' é novo: o cliente agora pode cancelar (decisão do grupo).
                t.HasCheckConstraint("CHK_Status_Reserva",
                    "StatusReserva IN ('Em andamento', 'Finalizado', 'Pendente', 'Cancelado')");
            });

            e.HasKey(a => a.Id);
            e.Property(a => a.Id).HasColumnName("IDAgendamento");
            e.Property(a => a.DataReserva).HasColumnType("date");
            e.Property(a => a.HoraInicio).HasColumnType("time");
            e.Property(a => a.HoraFim).HasColumnType("time");
            e.Property(a => a.StatusReserva).HasMaxLength(40).IsUnicode(false).HasDefaultValue("Pendente");
            e.Property(a => a.UsuarioId).HasColumnName("ID_Usuario");
            e.Property(a => a.QuadraId).HasColumnName("ID_Quadra");
            e.Property(a => a.PagamentoId).HasColumnName("ID_Pagamento");

            e.HasOne(a => a.Usuario).WithMany().HasForeignKey(a => a.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict) // sem cascata: o banco recusa excluir quem tem registros ligados
                .HasConstraintName("FK_Usuario_Agendamento");
            e.HasOne(a => a.Quadra).WithMany().HasForeignKey(a => a.QuadraId)
                .OnDelete(DeleteBehavior.Restrict) // sem cascata: o banco recusa excluir quem tem registros ligados
                .HasConstraintName("FK_Quadra_Agendamento");
            e.HasOne(a => a.Pagamento).WithMany().HasForeignKey(a => a.PagamentoId)
                .OnDelete(DeleteBehavior.Restrict) // sem cascata: o banco recusa excluir quem tem registros ligados
                .HasConstraintName("FK_Pagamento_Agendamento");
        });

        // ---------- LogExclusaoQuadras ----------
        modelBuilder.Entity<LogExclusaoQuadra>(e =>
        {
            e.ToTable("LogExclusaoQuadras");
            e.HasKey(l => l.Id);
            e.Property(l => l.Id).HasColumnName("ID_Log");
            e.Property(l => l.QuadraId).HasColumnName("ID_Quadra");
            e.Property(l => l.NomeQuadra).HasColumnName("Nome_Quadra").HasMaxLength(100).IsUnicode(false);
            e.Property(l => l.UsuarioAlteracao).HasColumnName("Usuario_Alteracao").HasMaxLength(100).IsUnicode(false)
                .HasDefaultValueSql("SUSER_NAME()");
            e.Property(l => l.DataAlteracao).HasColumnName("Data_Alteracao").HasColumnType("datetime")
                .HasDefaultValueSql("GETDATE()");
        });
    }
}
