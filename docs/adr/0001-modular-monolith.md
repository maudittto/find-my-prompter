# ADR-0001 — Modular Monolith

## Status
Accepted

## Context
O Find My Prompter precisa crescer por módulos de negócio, mas ainda está em fase inicial/MVP e não possui requisitos que justifiquem custos operacionais de microserviços.

## Decision
Começar como monólito modular, com limites de domínio claros dentro de uma única aplicação backend/deploy principal.

## Consequences
Positivas:
- menor complexidade operacional;
- transações e desenvolvimento local mais simples;
- evolução rápida do MVP;
- permite extrair serviços futuramente se métricas justificarem.

Trade-off:
- disciplina arquitetural será necessária para impedir acoplamento excessivo entre módulos.
