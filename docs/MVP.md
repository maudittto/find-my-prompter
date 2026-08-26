# Find My Prompter — MVP

## Objetivo
Entregar o menor produto capaz de conectar um profissional de IA a uma empresa e permitir que um processo de contratação aconteça de ponta a ponta.

## Fluxo obrigatório do profissional
1. Criar conta.
2. Fazer login.
3. Criar/editar perfil profissional.
4. Adicionar skills e modelos de IA.
5. Adicionar informações essenciais de experiência.
6. Criar ao menos um projeto de portfólio (opcional para candidatura, mas suportado pelo MVP).
7. Listar/pesquisar vagas.
8. Visualizar uma vaga.
9. Candidatar-se.
10. Acompanhar o status da candidatura.

## Fluxo obrigatório da empresa
1. Criar conta/fazer login.
2. Criar empresa.
3. Criar vaga em rascunho.
4. Publicar vaga.
5. Visualizar candidaturas.
6. Abrir o perfil de um candidato.
7. Alterar o status da candidatura.
8. Conseguir marcar o processo como `Hired` ou `Rejected`.

## Funcionalidades P0
- autenticação;
- perfil profissional;
- skills e modelos de IA estruturados;
- experiência profissional básica;
- portfólio básico;
- empresa e membros essenciais;
- criação/publicação/encerramento de vaga;
- listagem e filtros básicos de vaga;
- candidatura;
- pipeline/status de candidatura;
- dashboards mínimos de candidato e recrutador;
- administração/moderação mínima necessária para operar;
- segurança, autorização e requisitos essenciais de LGPD;
- SEO básico para páginas públicas de vagas, empresas e profissionais.

## Fora do MVP
- microserviços;
- Kafka;
- Kubernetes;
- Elasticsearch/OpenSearch;
- aplicativo mobile nativo;
- chat em tempo real;
- videochamada;
- pagamentos e assinaturas;
- feed social;
- matching avançado via LLM;
- Prompter Score;
- certificação automatizada de habilidades;
- machine learning próprio.

## Critério de aceite do MVP
O MVP só deve ser considerado concluído quando os dois fluxos abaixo forem demonstráveis de ponta a ponta:

### Profissional
`Register -> Login -> Profile -> Skills -> Job Search -> Apply -> Application Status`

### Recrutador
`Register -> Login -> Company -> Create Job -> Publish -> Applicants -> Candidate -> Status Change -> Hire`

Além disso:
- autorização deve impedir acesso indevido a recursos de terceiros;
- fluxo crítico deve possuir testes automatizados adequados;
- aplicação deve possuir logs/erros minimamente operáveis;
- deploy de produção deve ser reproduzível.
