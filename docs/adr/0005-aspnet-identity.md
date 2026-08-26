# ADR-0005 — ASP.NET Core Identity

## Status
Accepted

## Context
O MVP precisa de cadastro, login, sessão, recuperação/validação de conta e autorização sem construir primitivas de segurança do zero.

## Decision
Usar ASP.NET Core Identity integrado ao Entity Framework Core/PostgreSQL como base de identidade e autenticação.

Para a aplicação web, preferir autenticação baseada em cookie seguro em vez de persistir tokens em `localStorage`.

## Consequences
- segurança e lifecycle de identidade se apoiam em componentes do framework;
- `ApplicationUser` permanece focado em identidade; perfil profissional e memberships de empresa são modelos de negócio separados;
- OAuth/MFA podem ser adicionados posteriormente sem fazer parte do primeiro fluxo de login/cadastro.
