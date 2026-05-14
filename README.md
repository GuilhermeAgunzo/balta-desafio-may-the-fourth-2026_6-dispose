# Dispose

Dispose e um app fullstack de coleta urbana com IA, construido em .NET 10 para o desafio May The Fourth 2026.
Ele mostra os dias de coleta por tipo de residuo para cada bairro, destaca pontos de descarte especial e arma lembretes por proximidade em um frontend Blazor WASM com tema sci-fi inspirado em Star Wars.

## Stack

- .NET 10
- Blazor WebAssembly
- ASP.NET Core Minimal API
- Microsoft Agent Framework + OpenAI
- EF Core + SQLite
- Aspire 13.3.1
- xUnit + bUnit

## Estrutura da solucao

| Projeto | Responsabilidade |
| --- | --- |
| `Dispose.API` | Endpoints HTTP, composicao de dependencias, CORS, OpenAPI e bootstrap do banco |
| `Dispose.Application` | Regras de negocio para agenda, pontos de coleta, proximidade e montagem de contexto para IA |
| `Dispose.Core` | Entidades, enums, DTOs e contratos compartilhados |
| `Dispose.Infra` | `DbContext`, seeding SQLite e implementacoes de repositorios |
| `Dispose.AI` | Agente do Microsoft Agent Framework, prompt e invoker OpenAI |
| `Dispose.Web` | Frontend Blazor WASM com dashboard, consultas, assistente e configuracoes |
| `Dispose.AppHost` | Orquestracao Aspire da API e do frontend |
| `Dispose.ServiceDefaults` | Defaults de telemetria, health checks e service discovery do Aspire |
| `Dispose.UnitTests` | Testes unitarios de regras de negocio e da camada AI |
| `Dispose.API.Tests` | Testes de integracao dos endpoints principais |
| `Dispose.Web.Tests` | Testes bUnit para paginas criticas do frontend |

## Funcionalidades implementadas

- Dashboard inicial com resumo das proximas coletas, status do assistente e pontos especiais
- Pagina de coletas com filtro por bairro e tipo de residuo
- Pagina de descarte especial com ecopontos, geolocalizacao opcional e criacao de lembretes
- Pagina de assistente IA para orientar descarte e navegação dentro do app
- Pagina de configuracoes para bairro padrao, callsign, raio do lembrete e notificacoes
- Monitor de proximidade no frontend com alertas em HUD e uso opcional da Notification API
- API seedada com bairros, agendas e pontos especiais tematicos
- Camada `Dispose.AI` isolada apenas com o agente, sem serviços de negocio

## Como executar

### 1. Restaurar dependencias

```powershell
dotnet restore .\Dispose.slnx
```

### 2. Configurar a chave OpenAI

O assistente funciona com `OpenAI:ApiKey` e `OpenAI:Model`.
O jeito mais seguro e usar user secrets no projeto da API:

```powershell
dotnet user-secrets set "OpenAI:ApiKey" "<sua-chave>" --project .\src\Dispose.API
dotnet user-secrets set "OpenAI:Model" "gpt-4.1-mini" --project .\src\Dispose.API
```

Sem chave, o app continua operacional para coleta, pontos e lembretes; apenas o endpoint do assistente fica indisponivel.

### 3. Subir a solucao com Aspire

```powershell
dotnet run --project .\src\Dispose.AppHost
```

O AppHost orquestra:

- `dispose-api`
- `dispose-web`

### 4. Rodar API ou frontend isoladamente

```powershell
dotnet run --project .\src\Dispose.API
dotnet run --project .\src\Dispose.Web
```

O frontend usa `https://localhost:7023` como base padrao da API em `src\Dispose.Web\wwwroot\appsettings.json`.

## Banco e dados iniciais

O banco SQLite e criado automaticamente na primeira execucao da API via `EnsureCreated`.
O seed inclui:

- bairros: `Coruscant Centro`, `Jardim Naboo`, `Setor Tatooine`, `Bosque Endor`
- agendas para `organico`, `reciclavel`, `vidro` e `poda`
- pontos especiais para `pilhas`, `eletronicos`, `medicamentos` e `oleo`

## Testes

```powershell
dotnet test .\Dispose.slnx -v minimal
```

A cobertura atual prioriza:

- ordenacao e montagem do dashboard
- validacao e disparo de lembretes por proximidade
- composicao da resposta do agente
- endpoints de catalogo, status do assistente e fluxo de lembretes
- renderizacao do dashboard e comportamento da pagina do assistente no Blazor

## Design

O frontend reaproveita os tokens de `design.md` e os reinterpretou como um painel de comando:

- paineis HUD com cantos chanfrados
- tipografia Space Grotesk + Manrope
- fundo espacial com glow neon
- badges e alerts com leitura rapida para estados de coleta e proximidade
