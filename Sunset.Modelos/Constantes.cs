namespace Sunset.Modelos;

// Textos fixos usados em vários lugares (banco, negócio, telas).
// Deixar tudo aqui evita errar a digitação de "Gestor" ou "Pendente" em cada arquivo.
public static class Perfis
{
    public const string Admin = "Admin";
    public const string Gestor = "Gestor";
    public const string Cliente = "Cliente";
}

public static class StatusPagamento
{
    public const string Pendente = "Pendente";
    public const string Aprovado = "Aprovado";
    public const string Cancelado = "Cancelado";
}

public static class StatusReserva
{
    public const string Pendente = "Pendente";
    public const string EmAndamento = "Em andamento";
    public const string Finalizado = "Finalizado";
    public const string Cancelado = "Cancelado";
}

public static class Categorias
{
    public const string Volei = "Volei";
    public const string BeachTennis = "Beach Tennis";

    public static readonly string[] Todas = { Volei, BeachTennis };
}
