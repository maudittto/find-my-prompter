# Find My Prompter — Arquitetura

## Estratégia
O sistema começa como um monólito modular. Não adotar microserviços antes de existir necessidade operacional comprovada.

## Stack principal
### Frontend
- Next.js
- React
- TypeScript
- Tailwind CSS
- shadcn/ui

### Backend
- .NET 10
- ASP.NET Core
- Entity Framework Core
- ASP.NET Core Identity

### Dados
- PostgreSQL como banco principal
- PostgreSQL Full-Text Search para busca inicial
- pgvector futuramente para busca/matching semântico

### Infraestrutura evolutiva
- Docker / Docker Compose no desenvolvimento
- Redis quando cache/coordenação justificar
- Hangfire/Worker para tarefas assíncronas quando necessário
- Azure Blob Storage ou S3 para arquivos em produção
- OpenTelemetry/Sentry/Grafana conforme maturidade operacional

## Dependências do backend
Direção esperada:

```text
Api
 ├─ Application
 └─ Infrastructure
       ├─ Application
       └─ Domain

Application
 └─ Domain

Domain
 └─ nenhuma dependência de infraestrutura
```

O Domain não conhece ASP.NET, Entity Framework, PostgreSQL ou SDKs externos.

## Módulos de negócio planejados
- Identity
- Professionals
- Portfolio
- Companies
- Jobs
- Search
- Applications
- Notifications
- Administration

Não criar todos antecipadamente. Cada módulo nasce conforme sua vertical slice entra no roadmap.

## Runtime inicial

```text
Browser
   |
   v
Next.js
   |
   v
ASP.NET Core API
   |
   +--> PostgreSQL
   |
   +--> serviços adicionais somente quando necessários
```

## Autenticação
ASP.NET Core Identity será a base de identidade.

Diretrizes:
- `ApplicationUser` representa a identidade/autenticação, não o perfil profissional inteiro;
- dados de Prompter/Professional pertencem ao domínio de Professionals;
- para a aplicação web, a preferência arquitetural é autenticação por cookie seguro;
- autorização precisa validar ownership/policies no backend.

## Busca
Evolução planejada:

### V1
PostgreSQL + filtros estruturados + Full-Text Search.

### V2
Matching determinístico por skills, modelos, senioridade, idioma, salário e localização.

### V3
pgvector para similaridade semântica.

### V4
Ranking híbrido estruturado + semântico e, quando houver valor comprovado, explicações via LLM.

## Assíncrono
Não adicionar broker no MVP sem necessidade.
Quando emails/notificações e tarefas demoradas justificarem processamento fora do request, usar Hangfire/worker inicialmente.

## Arquivos
Currículos, imagens e anexos não devem depender do filesystem local em produção. Evoluir para object storage (Azure Blob/S3) com URLs controladas.

## Observabilidade
Desde cedo, erros devem ser tratados e logados. Instrumentação distribuída completa pode crescer com OpenTelemetry conforme os fluxos principais estabilizarem.
