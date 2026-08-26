# ADR-0003 — REST API

## Status
Accepted

## Context
O frontend Next.js precisa consumir operações claras de autenticação, perfis, empresas, vagas e candidaturas. O MVP não apresenta necessidade concreta de GraphQL.

## Decision
Usar ASP.NET Core REST API como interface principal entre frontend e backend.

## Consequences
- contratos HTTP simples e explícitos;
- menor complexidade de infraestrutura e tooling;
- GraphQL só será reconsiderado caso apareça um requisito comprovado que REST não atenda adequadamente.
