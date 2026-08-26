# ADR-0002 — PostgreSQL como banco principal

## Status
Accepted

## Context
O produto é fortemente relacional: usuários, empresas, vagas, skills, candidaturas e históricos. Também necessita de busca textual e poderá adotar busca vetorial posteriormente.

## Decision
Usar PostgreSQL como banco transacional principal com Entity Framework Core/Npgsql.

## Consequences
- relacionamentos e integridade permanecem em um banco principal;
- Full-Text Search pode atender a busca inicial;
- pgvector poderá ser adicionado posteriormente sem introduzir um segundo datastore imediatamente;
- Elasticsearch/OpenSearch não faz parte da arquitetura inicial.
