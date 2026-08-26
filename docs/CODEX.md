# Usando Codex no Find My Prompter

## Fonte de contexto
O arquivo `AGENTS.md` da raiz contém regras gerais. Existem instruções adicionais em:
- `backend/AGENTS.md`
- `apps/web/AGENTS.md`

O Codex deve consultar documentos de `docs/` conforme a tarefa, em vez de carregar ou reescrever todo o roadmap em cada mudança.

## Agentes do projeto
Perfis em `.codex/agents/`:
- `explorer` — exploração somente leitura;
- `backend_engineer` — implementação .NET/backend;
- `frontend_engineer` — implementação Next.js/frontend;
- `reviewer` — revisão somente leitura.

A disponibilidade de custom subagents pode variar conforme a versão/surface do Codex. Mesmo quando um agente customizado não puder ser selecionado diretamente, suas instruções podem servir como contrato de trabalho para o agente principal.

## Como pedir uma feature
Prefira prompts com resultado verificável.

Exemplo:

```text
Implemente a tela de cadastro do Find My Prompter.
Antes de editar, leia AGENTS.md, apps/web/AGENTS.md e docs/MVP.md.
O backend usa ASP.NET Core Identity.
Mantenha o escopo apenas em /register e integração com o endpoint existente.
Execute lint/build e reporte os arquivos alterados e verificações realizadas.
```

## Como pedir exploração

```text
Não altere arquivos. Mapeie o fluxo atual de autenticação do backend até o frontend, identifique endpoints, configuração de cookies/CORS e arquivos que precisariam mudar para implementar login e cadastro no Next.js.
```

## Como pedir revisão

```text
Revise as alterações atuais contra AGENTS.md e docs/ARCHITECTURE.md. Priorize bugs, autorização, regressões, segurança e testes ausentes. Não faça comentários apenas de estilo.
```

## Como pedir uma vertical slice

```text
Implemente <feature> como a menor vertical slice completa. Não crie infraestrutura para funcionalidades futuras. Preserve as decisões arquiteturais documentadas e execute os checks relevantes antes de concluir.
```

## Regras para tarefas maiores
Para tarefas que cruzam backend e frontend:
1. explorar o fluxo atual;
2. definir o menor contrato de API necessário;
3. implementar backend;
4. implementar frontend;
5. validar o fluxo de ponta a ponta;
6. revisar autorização/erros;
7. executar testes/build/lint.

## Atualização da documentação
Atualize documentação somente quando houver decisão ou comportamento durável. Não transforme `AGENTS.md` em diário de progresso.

Mudanças arquiteturais importantes devem gerar/atualizar um ADR em `docs/adr/`.
