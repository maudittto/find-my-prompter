# Find My Prompter — Arquitetura e organização do código

## Objetivo

Este documento define onde cada responsabilidade deve ficar no backend e no frontend. A meta não é criar o maior número possível de camadas, mas manter cada vertical slice fácil de localizar, entender, testar e alterar.

As arquiteturas adotadas são:

- **backend:** monólito modular com vertical slices, preservando os projetos `Api`, `Application`, `Domain` e `Infrastructure`;
- **frontend:** Next.js App Router com organização por feature, usando Server Components por padrão.

Não devem ser adicionados MediatR, repositórios genéricos, uma interface para cada classe ou um projeto por módulo sem uma necessidade concreta.

## Princípios compartilhados

1. Organizar primeiro pelo negócio (`Jobs`, `Companies`, `Applications`), não por pastas genéricas como `Services`, `Managers` e `Helpers`.
2. Implementar uma feature de ponta a ponta antes de preparar infraestrutura para features futuras.
3. Manter contratos de API explícitos e não expor diretamente entidades do domínio ou do EF Core.
4. Colocar uma regra no lugar que é responsável por garanti-la:
   - integridade e invariantes na entidade/domínio;
   - coordenação do caso de uso na aplicação;
   - persistência e integrações na infraestrutura;
   - HTTP e apresentação nas bordas.
5. Criar abstrações somente depois de existir reutilização ou uma fronteira real a proteger.
6. Manter tipos próximos de quem é seu dono. Não criar arquivos globais que virem depósitos de tipos ou utilitários.

---

## Backend

### Arquitetura escolhida

O backend usa um **monólito modular orientado a vertical slices**. Os quatro projetos existentes continuam sendo limites técnicos, enquanto as pastas internas representam módulos e casos de uso.

```text
Api
 ├─ Application
 └─ Infrastructure
     ├─ Application
     └─ Domain

Application
 └─ Domain

Domain
 └─ nenhuma dependência dos demais projetos
```

Essa direção de dependências é obrigatória. `Domain` e `Application` não podem depender de `Api` ou `Infrastructure`.

### Responsabilidade de cada projeto

| Projeto | Responsabilidade | Não deve conter |
| --- | --- | --- |
| `Api` | composição da aplicação, controllers, contratos HTTP, autenticação/autorização HTTP e códigos de resposta | regra de negócio, consultas EF ou configuração detalhada de infraestrutura dentro de `Program.cs` |
| `Application` | casos de uso, coordenação do fluxo, validações do caso de uso e portas necessárias | tipos ASP.NET, detalhes de PostgreSQL/EF Core ou regras de apresentação |
| `Domain` | entidades, value objects, enums e invariantes do negócio | HTTP, EF Core, Identity, DTOs de API ou SDKs externos |
| `Infrastructure` | EF Core, PostgreSQL, Identity, implementações de persistência e integrações externas | decisões de apresentação ou regras de negócio que pertencem ao domínio |

### Estrutura alvo

As pastas abaixo são um mapa de crescimento, não um pedido para criá-las vazias. Cada pasta nasce quando sua primeira feature for implementada.

```text
backend/
├─ src/
│  ├─ FindMyPrompter.Api/
│  │  ├─ Controllers/
│  │  │  ├─ JobsController.cs
│  │  │  └─ HealthController.cs
│  │  ├─ Contracts/
│  │  │  └─ Jobs/
│  │  │     ├─ CreateJobRequest.cs
│  │  │     └─ JobResponse.cs
│  │  ├─ Program.cs
│  │  └─ appsettings*.json
│  │
│  ├─ FindMyPrompter.Application/
│  │  ├─ Identity/
│  │  ├─ Jobs/
│  │  │  ├─ CreateJob/
│  │  │  │  ├─ CreateJob.cs
│  │  │  │  └─ CreateJobHandler.cs
│  │  │  └─ GetJob/
│  │  │     └─ GetJob.cs
│  │  └─ DependencyInjection.cs
│  │
│  ├─ FindMyPrompter.Domain/
│  │  ├─ Jobs/
│  │  │  ├─ Job.cs
│  │  │  └─ JobStatus.cs
│  │  ├─ Companies/
│  │  └─ Applications/
│  │
│  └─ FindMyPrompter.Infrastructure/
│     ├─ Identity/
│     ├─ Persistence/
│     │  ├─ Configurations/
│     │  ├─ Migrations/
│     │  └─ AppDbContext.cs
│     ├─ Jobs/
│     └─ DependencyInjection.cs
│
└─ tests/
   ├─ FindMyPrompter.Domain.Tests/
   ├─ FindMyPrompter.Application.Tests/
   └─ FindMyPrompter.Api.IntegrationTests/
```

Os nomes ilustram a regra; não obrigam que todo caso de uso tenha dois arquivos. Um caso de uso pequeno pode começar em um único arquivo e ser separado quando crescer.

Um controller por módulo é o padrão. Ele só deve ser dividido quando ficar grande demais ou quando um subconjunto de rotas tiver política de acesso própria.

### O papel do `Program.cs`

`Program.cs` é apenas o **composition root**. Ele deve:

- criar o builder;
- registrar os projetos/capacidades;
- configurar o pipeline HTTP;
- mapear os controllers;
- executar a aplicação.

Ele não deve conter lambdas com comportamento de endpoints, consultas ao banco ou regras de negócio. A forma esperada é semelhante a:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddOpenApi();
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGroup("/api/auth").MapIdentityApi<ApplicationUser>();

app.Run();
```

`MapIdentityApi` é a única exceção ao padrão de controllers: são rotas prontas do ASP.NET Core Identity e não devem ser reescritas manualmente.

### Padrão de controller

Controllers são finos: recebem o request HTTP, delegam ao caso de uso e traduzem o resultado em status code. Não contêm regra de negócio nem consulta EF.

```csharp
[ApiController]
[Route("api/jobs")]
public class JobsController : ControllerBase
{
    [HttpPost]
    [Authorize]
    [ProducesResponseType<JobResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<JobResponse>> Create(
        CreateJobRequest request,
        CreateJobHandler handler,
        CancellationToken cancellationToken)
    {
        var job = await handler.HandleAsync(request.ToInput(), cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = job.Id }, JobResponse.From(job));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<JobResponse>> GetById(
        Guid id,
        GetJobHandler handler,
        CancellationToken cancellationToken)
    {
        var job = await handler.HandleAsync(id, cancellationToken);

        return job is null ? NotFound() : Ok(JobResponse.From(job));
    }
}
```

Convenções obrigatórias:

- `[ApiController]` em todos os controllers, aproveitando validação automática de model e `ProblemDetails` em `400`;
- herdar de `ControllerBase` (nunca de `Controller`, que carrega suporte a Views);
- rota explícita em `[Route("api/<recurso>")]`, no plural e em kebab-case quando houver mais de uma palavra;
- um `[Http*]` por action, retornando `Task<ActionResult<T>>`;
- injetar dependências por parâmetro da action ou pelo construtor; não usar service locator;
- `[Authorize]` no controller quando todo o recurso for protegido, e `[AllowAnonymous]` na exceção;
- receber e devolver DTOs de `Api/Contracts`, nunca entidades do domínio ou do EF.

```csharp
[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(AppDbContext dbContext)
    {
        var available = await dbContext.Database.CanConnectAsync();

        return Ok(new
        {
            status = available ? "healthy" : "unhealthy",
            database = available ? "connected" : "disconnected"
        });
    }
}
```

O health check pode acessar a infraestrutura por ser um diagnóstico da plataforma. Controllers de negócio devem delegar ao caso de uso da camada `Application`.

### Forma de uma vertical slice

Exemplo de `CreateJob`:

```text
HTTP request
  -> Api/Controllers/JobsController.cs (POST /api/jobs)
  -> Application/Jobs/CreateJob/CreateJobHandler.cs
  -> Domain/Jobs/Job.cs
  -> porta de persistência definida pela necessidade do caso de uso
  -> Infrastructure/Jobs/... implementação EF Core
  -> HTTP response
```

- A action do controller converte HTTP em input do caso de uso e o resultado em status/response HTTP.
- O handler coordena autorização contextual, carregamento, mudança do domínio e persistência.
- A entidade garante invariantes como transições de status válidas.
- A infraestrutura implementa persistência e integrações.
- DTOs HTTP pertencem à `Api`; inputs/results independentes de HTTP pertencem à `Application`.

### Regras de organização do backend

- Um controller por módulo/recurso; dividir só quando crescer ou quando houver política de acesso distinta.
- Manter controllers finos: sem regra de negócio, sem consulta EF, sem `try/catch` genérico repetido em cada action.
- Nomear classes pelo comportamento: `CreateJobHandler`, não `JobService`.
- Não criar `BaseRepository`, `GenericService`, `Manager` ou pasta global `Helpers`.
- Criar uma interface somente para uma fronteira real, como persistência ou serviço externo usado pela aplicação; não apenas para espelhar uma classe.
- Preferir repositórios/portas específicos do domínio quando necessários, não um CRUD genérico.
- Manter validação de request na borda e invariantes de negócio no domínio.
- Exigir autenticação e verificar ownership/policy nos recursos de usuário ou empresa.
- Não retornar entidades EF diretamente.
- Separar comandos e consultas por caso de uso, sem introduzir um framework de CQRS. CQRS aqui é apenas organização, não infraestrutura adicional.

### Testes do backend

- `Domain.Tests`: invariantes e transições importantes das entidades.
- `Application.Tests`: casos de uso com ramificações relevantes.
- `Api.IntegrationTests`: rotas dos controllers e contrato HTTP, autenticação, autorização, persistência e erros dos fluxos críticos.
- Espelhar nos testes o nome do módulo/caso de uso; não criar uma suíte vazia para cada camada antes de existir comportamento.

---

## Frontend

### Arquitetura escolhida

O frontend usa **App Router + organização por feature**.

- `app/` representa URLs, layouts e limites do Next.js.
- `features/` representa comportamentos do produto.
- `components/` contém apenas UI realmente compartilhada.
- `lib/` contém infraestrutura pequena e transversal, não regras de negócio.

O `page.tsx` deve ser um arquivo de composição: obter dados, montar a página com componentes da feature e declarar metadata quando necessário. Ele não deve concentrar formulários grandes, tipos de domínio, chamadas HTTP repetidas e várias responsabilidades visuais.

### Estrutura alvo

```text
apps/web/
├─ app/
│  ├─ (public)/
│  │  └─ jobs/
│  │     ├─ [slug]/
│  │     │  ├─ page.tsx
│  │     │  ├─ loading.tsx
│  │     │  └─ not-found.tsx
│  │     └─ page.tsx
│  ├─ (auth)/
│  │  ├─ login/page.tsx
│  │  └─ register/page.tsx
│  ├─ (dashboard)/
│  │  └─ dashboard/
│  ├─ error.tsx
│  ├─ globals.css
│  └─ layout.tsx
│
├─ features/
│  ├─ auth/
│  │  ├─ auth-form.tsx
│  │  ├─ auth.api.ts
│  │  └─ auth.types.ts
│  ├─ jobs/
│  │  ├─ job-card.tsx
│  │  ├─ job-list.tsx
│  │  ├─ jobs.api.ts
│  │  └─ job.types.ts
│  └─ applications/
│
├─ components/
│  ├─ ui/
│  └─ layout/
│
├─ lib/
│  ├─ api.ts
│  └─ env.ts
│
└─ public/
```

Route groups como `(public)`, `(auth)` e `(dashboard)` organizam layouts sem alterar a URL. Eles também só devem ser criados quando houver rotas que compartilhem de fato um layout ou uma política de acesso.

### Responsabilidade de cada lugar

| Local | Deve conter | Não deve conter |
| --- | --- | --- |
| `app/**/page.tsx` | composição da rota, carregamento server-side, metadata e componentes da feature | grandes blocos de UI, tipos de negócio ou cliente HTTP próprio |
| `app/**/_components` | componentes privados de uma única rota | componentes usados por outras rotas |
| `features/<feature>` | componentes, tipos e acesso à API pertencentes à feature | elementos genéricos ou código de outra feature |
| `components/ui` | primitives reutilizáveis, incluindo shadcn/ui quando adotado | regras de Jobs, Auth ou Applications |
| `components/layout` | header, footer e shells compartilhados | lógica de um fluxo de negócio |
| `lib` | configuração, ambiente e infraestrutura transversal comprovadamente compartilhada | `utils.ts` genérico ou tipos de domínio de todas as features |

### Onde colocar interfaces e tipos

A regra é **ownership**, não “todo tipo precisa de um arquivo”.

- Tipo usado apenas em um componente pequeno: pode ficar no mesmo arquivo.
- Tipo que representa uma feature ou é usado em mais de um arquivo dela: `features/jobs/job.types.ts`.
- Contrato específico de uma chamada: pode ficar junto de `jobs.api.ts` se não for reutilizado.
- Props de componente: ficam junto do componente.
- Props geradas ou específicas da rota: podem ficar no `page.tsx`.
- Tipo verdadeiramente transversal: mover para `lib` somente quando o compartilhamento já existir.

Não criar um `types/` global com todos os modelos da aplicação. Também não duplicar entidades C# integralmente no frontend: definir apenas o formato que a tela realmente recebe da API.

### Server e Client Components

- Componentes são Server Components por padrão.
- Adicionar `"use client"` somente no menor componente que precise de estado, evento ou API do navegador.
- Não transformar uma página inteira em Client Component só porque existe um formulário dentro dela.
- Buscar dados no servidor quando isso beneficiar renderização, segurança ou SEO.
- Não duplicar regras de autorização no frontend; o backend continua sendo a autoridade.

Exemplo de página enxuta:

```tsx
import { JobList } from "@/features/jobs/job-list";
import { getJobs } from "@/features/jobs/jobs.api";

export default async function JobsPage() {
  const jobs = await getJobs();

  return <JobList jobs={jobs} />;
}
```

Se `JobList` for usado apenas nessa rota, ele também pode começar em `app/(public)/jobs/_components/job-list.tsx`. Deve ser movido para `features/jobs` quando passar a representar a feature ou for reutilizado.

### Fluxo de dados

```text
page/layout (Server Component)
  -> função da feature em *.api.ts
  -> ASP.NET Core API
  -> resultado tipado
  -> componente da feature
  -> Client Component somente na parte interativa
```

Uma pequena função compartilhada de `fetch` pode viver em `lib/api.ts` quando já houver repetição de URL base, cookies e tradução de erros. Não criar antecipadamente um SDK interno, classes de repository ou uma camada genérica de hooks.

### Regras de organização do frontend

- `page.tsx` e `layout.tsx` compõem; componentes de feature renderizam e interagem.
- Preferir imports absolutos com `@/`.
- Não usar barrel files (`index.ts`) por padrão; imports diretos deixam dependências visíveis e evitam ciclos.
- Uma feature não deve importar arquivos de `app/`.
- Evitar que uma feature importe outra diretamente. Quando duas features precisarem do mesmo primitive, extrair somente a parte realmente compartilhada.
- Não criar `hooks/`, `services/`, `schemas/` e `types/` dentro de toda feature por convenção; criar o arquivo quando existir conteúdo real.
- Formulários devem tratar loading, sucesso, erro e acessibilidade.
- Validação no frontend melhora a experiência; a validação autoritativa permanece no backend.
- Segredos e credenciais nunca podem ser expostos por variáveis `NEXT_PUBLIC_*`.

### Testes do frontend

- Testes de componente podem ficar próximos do componente: `job-card.test.tsx`.
- Testes E2E ficam em uma pasta própria e cobrem jornadas críticas, não detalhes de implementação.
- Priorizar os fluxos `register/login`, candidatura e mudança de status quando estiverem funcionais.
- Não adicionar uma ferramenta de testes antes da primeira necessidade concreta.

---

## Como decidir quando separar um arquivo

Separar quando pelo menos uma destas condições existir:

- o bloco possui responsabilidade e nome próprios;
- é reutilizado em outro lugar;
- precisa ser testado isoladamente;
- possui fronteira server/client própria;
- seus tipos pertencem à feature e são usados por mais de um arquivo;
- o arquivo atual mistura HTTP, regra de negócio, persistência ou apresentação.

Não separar apenas para obedecer uma árvore ideal. Um arquivo curto, coeso e usado uma vez é melhor que cinco arquivos de uma linha.

## Dependências permitidas no frontend

```text
app
 ├─ features
 ├─ components
 └─ lib

features
 ├─ components
 └─ lib

components/ui e lib
 └─ não dependem de app ou de uma feature específica
```

## Checklist para uma nova feature

1. Identificar o comportamento mínimo da vertical slice.
2. Definir request e response mínimos da API.
3. Implementar domínio/persistência somente se a feature exigir.
4. Criar o caso de uso na pasta do módulo em `Application`.
5. Criar a action no controller do módulo, com DTOs em `Api/Contracts`.
6. Criar a rota Next.js como composição e manter UI/tipos na feature ou na pasta privada da rota.
7. Verificar validação, autorização, loading, empty, error e success conforme aplicável.
8. Executar build, testes e lint relevantes.

## Decisões adiadas intencionalmente

Adicionar somente quando houver necessidade demonstrada:

- MediatR ou outro mediator;
- FluentValidation;
- repositório genérico/unit of work próprio;
- projeto .NET separado para cada módulo;
- design system próprio;
- gerador automático de cliente OpenAPI;
- estado global no frontend;
- pastas compartilhadas abstratas sem reutilização real.

Até lá, métodos de extensão, classes de caso de uso explícitas, `fetch`, Server Components e composição normal do React são suficientes.
