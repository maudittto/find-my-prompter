# Find My Prompter — Roadmap de Desenvolvimento

A ordem abaixo é a ordem padrão. Não significa calendário fixo.

## Estado inicial esperado
- organização/repositório Git criado;
- frontend Next.js criado e executando;
- backend .NET criado e executando;
- estrutura base versionada;
- PostgreSQL/EF Core e Identity devem ser verificados no código, não presumidos pela documentação.

## M0 — Foundation
Objetivo: cadeia técnica mínima confiável.

- solution .NET;
- app Next.js;
- PostgreSQL local via Docker;
- EF Core/Npgsql;
- health endpoint;
- frontend alcança backend;
- configuração de desenvolvimento;
- CI básico;
- tratamento global de erros/logging essencial.

Saída: `Browser -> Next.js -> ASP.NET -> PostgreSQL` funcionando.

## M1 — Identity
Objetivo: sessão autenticada real.

- ASP.NET Core Identity;
- register;
- login;
- logout;
- current user/session;
- endpoint protegido;
- integração de cookies com Next.js;
- páginas `/register` e `/login`;
- testes essenciais.

Deixar email verification/reset password para o mesmo milestone apenas quando o fluxo básico já estiver estável.

## M2 — Professional Profile
- criar perfil;
- editar perfil;
- username público;
- headline/about;
- skills;
- modelos/ferramentas de IA;
- idiomas quando necessário;
- experiência;
- página pública `/prompters/[username]`.

## M3 — Portfolio
- criar/editar/remover projeto;
- visibilidade Public/Partial/Private;
- input/output de exemplo;
- links externos/repositório.

## M4 — Companies
- criar empresa;
- editar perfil;
- membership Owner/Recruiter;
- página pública `/companies/[slug]`.

## M5 — Jobs
- criar rascunho;
- editar;
- publicar;
- pausar/fechar;
- skills/modelos estruturados;
- página pública `/jobs/[slug]`.

## M6 — Search
- `/jobs`;
- busca textual;
- filtros por skill/model/seniority/work mode/location/salary/employment type;
- PostgreSQL Full-Text Search se necessário para qualidade/performance.

## M7 — Applications
- aplicar;
- impedir candidatura duplicada;
- retirar candidatura;
- listar candidaturas do profissional;
- listar candidatos da vaga;
- mudar status;
- registrar histórico;
- autorização/ownership rigorosos.

## M8 — Dashboards
Professional:
- perfil;
- candidaturas;
- vagas salvas.

Recruiter:
- vagas;
- candidatos;
- status/pipeline.

## M9 — Notifications
- eventos importantes;
- email/background jobs apenas quando necessário;
- notificações internas simples se trouxerem valor.

## M10 — Administration
- usuários;
- empresas;
- vagas;
- denúncias;
- moderação mínima.

## M11 — Stabilization / Launch
- revisão de autorização e IDOR/BOLA;
- rate limiting;
- validação de uploads;
- LGPD essencial;
- SEO e JobPosting estruturado;
- testes E2E dos dois fluxos principais;
- observabilidade;
- deploy reproduzível;
- performance dos principais endpoints.

## Depois do MVP
Somente depois de usuários/empresas reais e métricas:

1. matching determinístico;
2. pgvector e embeddings;
3. matching híbrido;
4. parsing assistido por IA de CV/vagas;
5. explicações de match;
6. skill verification;
7. monetização B2B;
8. escala infra conforme gargalos medidos.
