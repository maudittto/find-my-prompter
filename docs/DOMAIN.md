# Find My Prompter — Domínio

Este documento descreve o modelo pretendido. Ele não é autorização para criar todas as entidades imediatamente.

## Identity
Responsável por autenticação e identidade técnica.

Conceitos:
- ApplicationUser
- roles/policies quando necessárias
- email verification
- password reset

Regra: identidade técnica é separada do perfil profissional e de memberships de empresas.

## Professionals
### PrompterProfile / ProfessionalProfile
Campos planejados:
- UserId
- Username
- DisplayName
- Headline
- About
- Location
- RemotePreference
- SalaryExpectation
- Availability
- Seniority
- ProfileImage
- CreatedAt
- UpdatedAt

### Skills
Taxonomia estruturada, por exemplo:
- Prompt Engineering
- RAG
- Agents
- Tool Calling
- Structured Outputs
- Embeddings
- Evaluation
- Fine-tuning
- AI Automation
- LLM APIs

### AI Models / Tools
Exemplos:
- OpenAI / GPT
- Claude
- Gemini
- Llama
- LangChain
- Semantic Kernel
- n8n
- Cursor

### Experience
- Company
- Position
- Description
- StartDate
- EndDate
- CurrentlyWorking
- Location

### Education / Languages
Adicionar quando a vertical slice correspondente for implementada.

## Portfolio
### PortfolioProject
Campos planejados:
- Title
- Description
- Objective
- Prompt ou conteúdo demonstrativo
- PromptVisibility: Public | Partial | Private
- AIModel
- Techniques
- ExampleInput
- ExampleOutput
- RepositoryUrl
- ExternalUrl

Nunca forçar a exposição pública de propriedade intelectual para que o perfil funcione.

## Companies
### Company
- Name
- Slug
- Logo
- Description
- Website
- Industry
- CompanySize
- Location

### CompanyMember
Relaciona usuários a empresas.
Papéis iniciais possíveis:
- Owner
- Recruiter

## Jobs
### Job
- CompanyId
- Title
- Slug
- Description
- Seniority
- EmploymentType
- WorkMode
- Location
- SalaryMin
- SalaryMax
- Currency
- Status
- PublishedAt

### JobStatus
- Draft
- Published
- Paused
- Closed

### Requisitos estruturados
Usar relações como JobSkill/JobAIModel em vez de depender apenas de texto livre na descrição.

## Applications
### Application
- JobId
- ProfessionalId
- Status
- AppliedAt
- UpdatedAt

### ApplicationStatus
- Applied
- Viewed
- Screening
- Interview
- TechnicalChallenge
- Offer
- Hired
- Rejected
- Withdrawn

### ApplicationStatusHistory
Registrar transições importantes com:
- OldStatus
- NewStatus
- ChangedBy
- ChangedAt

Não perder histórico substituindo apenas o status atual.

## Search
Primeiro, busca por texto e filtros estruturados. Matching é uma camada posterior e não deve contaminar prematuramente as entidades centrais.

## Notifications
Eventos planejados:
- ApplicationReceived
- ApplicationViewed
- ApplicationStatusChanged
- JobPublished
- NewJobMatch (posterior)

## Administration / Moderation
Conceitos futuros do MVP operacional:
- Report
- ModerationAction
- AuditLog quando necessário

Tipos de denúncia possíveis:
- FakeJob
- Spam
- Fraud
- Harassment
- InappropriateContent
- Other
