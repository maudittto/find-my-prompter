# ADR-0004 — Next.js para frontend web

## Status
Accepted

## Context
O produto precisa de experiência web moderna, páginas públicas indexáveis e dashboards autenticados.

## Decision
Usar Next.js com React, TypeScript e App Router. Tailwind CSS e shadcn/ui serão usados para acelerar a UI sem criar um design system complexo prematuramente.

## Consequences
- páginas públicas e aplicação autenticada podem coexistir no mesmo frontend;
- Server Components serão preferidos por padrão, com Client Components somente onde necessário;
- SEO de vagas/perfis/empresas deve ser considerado durante a implementação das páginas públicas.
