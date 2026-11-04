# Injeção de dependência no SUNSET

O manual do PIM IV pede que as dependências entre as camadas sejam configuradas por injeção de dependência no `Program.cs` da Web e da Api. Este arquivo explica como isso funciona no nosso projeto.

## A ideia

Cada camada precisa da camada de baixo, e cada classe recebe a que precisa no construtor, sem usar `new`:

- o `ReservasController` precisa do `AgendamentoNegocio`
- o `AgendamentoNegocio` precisa dos repositórios (`IAgendamentoRepositorio`, `IPagamentoRepositorio`, `IQuadraRepositorio` e `IUsuarioRepositorio`)
- o `AgendamentoRepositorio` precisa do `AppDbContext`
- o `AppDbContext` precisa da connection string

```csharp
public class ReservasController : Controller
{
    private readonly AgendamentoNegocio _negocio;

    public ReservasController(AgendamentoNegocio negocio, RelatorioNegocio relatorio)
    {
        _negocio = negocio;
    }
}
```

Quem chama esse construtor e entrega os objetos prontos é o ASP.NET Core, seguindo uma lista que escrevemos uma única vez no `Program.cs`.

## Por que não usar `new` no controller

Se o controller criasse tudo na mão, a connection string ia ficar espalhada pelo código (o manual manda deixar só no `appsettings.json`) e a Web e a Api estariam acessando o Repositório direto, o que o manual proíbe. Também ninguém ficaria responsável por fechar a conexão com o banco.

## A lista do Program.cs

O bloco é igual na Web e na Api:

```csharp
builder.Services.AddDbContext<AppDbContext>(opcoes => opcoes.UseSqlServer(connectionString));

builder.Services.AddScoped<IQuadraRepositorio, QuadraRepositorio>();
builder.Services.AddScoped<IAgendamentoRepositorio, AgendamentoRepositorio>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
builder.Services.AddScoped<IPagamentoRepositorio, PagamentoRepositorio>();

builder.Services.AddScoped<QuadraNegocio>();
builder.Services.AddScoped<AgendamentoNegocio>();
builder.Services.AddScoped<UsuarioNegocio>();
builder.Services.AddScoped<RelatorioNegocio>();
```

| Linha | O que quer dizer |
|---|---|
| `AddDbContext<AppDbContext>(...)` | Quando alguém pedir um `AppDbContext`, criar um que usa o SQL Server com a connection string do `appsettings.json` |
| `AddScoped<IQuadraRepositorio, QuadraRepositorio>()` | Quando alguém pedir um `IQuadraRepositorio`, entregar um `QuadraRepositorio` |
| `AddScoped<QuadraNegocio>()` | Quando alguém pedir um `QuadraNegocio`, criar um |

A segunda linha é polimorfismo: o Negócio só conhece a interface, e o `Program.cs` decide qual classe entra no lugar. Os `Program.cs` da Web e da Api são dois programas separados, então toda linha nova vai para os dois.

`AddScoped` quer dizer um objeto novo para cada requisição, descartado quando a resposta sai. É assim que a conexão com o banco é fechada.

## O que acontece quando o usuário abre uma tela

1. O navegador pede, por exemplo, `/Reservas/Minhas`.
2. O ASP.NET Core vai criar o `ReservasController` e vê que o construtor pede um `AgendamentoNegocio`.
3. Procura na lista do `Program.cs`, acha, e vê que o `AgendamentoNegocio` pede os repositórios.
4. Os repositórios pedem o `AppDbContext`, que a lista sabe criar.
5. Ele monta tudo de baixo para cima (contexto, repositórios, negócio e controller) e chama a action.
6. Quando a resposta sai, tudo que foi criado é descartado.

## Para criar uma entidade nova

1. A classe de repositório recebe o `AppDbContext` no construtor.
2. A classe de Negócio recebe a interface do repositório.
3. O controller recebe a classe de Negócio.
4. Duas linhas novas em cada `Program.cs` (Web e Api):

```csharp
builder.Services.AddScoped<IExemploRepositorio, ExemploRepositorio>();
builder.Services.AddScoped<ExemploNegocio>();
```

## Erros comuns

- `Unable to resolve service for type 'X'`: falta registrar o `X` no `Program.cs`. Confira se a linha está nos dois projetos.
- Registrar só a classe do repositório, sem a interface (`AddScoped<QuadraRepositorio>()`): o Negócio pede a interface e dá o mesmo erro. A linha precisa ter as duas: `AddScoped<IQuadraRepositorio, QuadraRepositorio>()`.
- Usar `new` no controller: não compila, porque o construtor exige o que vem da injeção.
