# Find My Prompter — Desenvolvimento

## Princípio
Entregar vertical slices pequenas e demonstráveis. O progresso é medido pelo comportamento do produto, não por quantidade de infraestrutura ou abstrações.

## Ambiente local
A estrutura esperada inclui:
- `apps/web` para Next.js;
- `backend` para a solution .NET;
- PostgreSQL via Docker Compose.

Os comandos abaixo devem ser ajustados se os scripts/paths reais do repositório divergirem.

## Backend
Da raiz do repositório:

```bash
dotnet restore backend/FindMyPrompter.sln
dotnet build backend/FindMyPrompter.sln
dotnet test backend/FindMyPrompter.sln
```

Executar API:

```bash
dotnet run --project backend/src/FindMyPrompter.Api
```

## Frontend

```bash
cd apps/web
npm install
npm run dev
```

Antes de concluir mudanças:

```bash
npm run lint
npm run build
```

Use o lockfile já escolhido pelo repositório. Não troque npm/pnpm/yarn sem decisão explícita.

## Docker
Na raiz, quando o `docker-compose.yml` estiver configurado:

```bash
docker compose up -d
```

Não apague volumes de banco automaticamente em scripts de rotina.

## EF Core
Exemplo de migration a partir da raiz, adaptando paths se necessário:

```bash
dotnet ef migrations add <MigrationName> \
  --project backend/src/FindMyPrompter.Infrastructure \
  --startup-project backend/src/FindMyPrompter.Api \
  --output-dir Persistence/Migrations
```

Atualizar banco local conscientemente:

```bash
dotnet ef database update \
  --project backend/src/FindMyPrompter.Infrastructure \
  --startup-project backend/src/FindMyPrompter.Api
```

## Branches e commits
Branches curtas quando usadas:
- `feature/FMP-123-create-job`
- `fix/FMP-456-duplicate-application`

Commits preferenciais:
- `feat: add user registration`
- `fix: prevent duplicate applications`
- `test: add login integration tests`
- `docs: document authentication flow`

## Workflow de uma feature
1. Ler issue/objetivo.
2. Inspecionar código existente.
3. Identificar vertical slice mínima.
4. Implementar.
5. Executar build/lint/testes relevantes.
6. Revisar autorização e validação.
7. Atualizar docs apenas se comportamento/arquitetura mudou.
8. Manter diff pequeno e focado.

## Definition of Done
- build passa;
- testes relevantes passam;
- lint passa no frontend;
- migrations incluídas quando necessárias;
- autorização validada;
- estados de erro tratados;
- sem secrets;
- nenhuma feature não solicitada adicionada "aproveitando" a mudança.
