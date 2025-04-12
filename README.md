# 📌 Documentação do MVP - Health&Med (Hackathon FIAP 4NETT)

## 📚 Índice

- [🚀 Introdução](#-introdução)
- [📌 Arquitetura da Solução](#-arquitetura-da-solução)
- [📦 Arquitetura do Projeto](#-arquitetura-do-projeto)
- [📌 Requisitos Funcionais](#-requisitos-funcionais)
  - [🛠️ Endpoints principais](#️-endpoints-principais)
- [📈 Requisitos Não-Funcionais](#-requisitos-não-funcionais)
- [🧪 Testes](#-testes)
- [📦 Infraestrutura e CI/CD](#-infraestrutura-e-cicd)
- [📌 Modelagem de Dados](#-modelagem-de-dados)
  - [🎯 Benefícios dessa Estrutura](#-benefícios-dessa-estrutura)
- [🔐 Sistema de Autenticação e Segurança](#-sistema-de-autenticação-e-segurança)
- [🚀 How to Use – Health&Med MVP](#-how-to-use--healthmed-mvp)


# 🚀 **Introdução**

A Health&Med é uma startup inovadora no setor de saúde, focada em Telemedicina. Este documento descreve o MVP desenvolvido como solução proprietária para agendamento e gerenciamento de consultas online, visando segurança, escalabilidade e eficiência.

---

## 📌 **Arquitetura da Solução**

**Tecnologias:**

- **Backend:** ASP.NET Core (.NET 9)
- **Banco de Dados:** SQL Server
- **Comunicação:** REST API com Swagger
- **Containerização e Orquestração:** Docker e Kubernetes
- **Mensageria:** RabbitMQ
- **Cache**: Redis
- **Monitoramento:** Prometheus e Grafana
- **Testes:** xUnit para testes unitários e integração
- **Pipeline CI/CD:** GitHub Actions

---

## 📦 **Arquitetura do Projeto**

O projeto segue a arquitetura em camadas com a seguinte estrutura:

- **Api:** Controllers e endpoints
- **Application:** DTOs, regras de negócio, serviços
- **Domain:** Entidades, enums e interfaces de repositório
- **Infrastructure:** Implementação de persistência com EF Core, migrations
- **IoC:** Inversão de Controle, registro de dependências
- Microsserviço Agendar:
    
    Mesma arquitetura em camada, mas sem a camada de Api e com o Service que contém o Worker
    
    - Implementado como **Worker Service**, escuta fila do **RabbitMQ**.
    - Responsável por processar os agendamentos de forma **assíncrona e desacoplada**.

**Benefícios:**

- Escalabilidade
- Resiliência
- Fácil manutenção
- Clara divisão de responsabilidades

---
![image](https://github.com/user-attachments/assets/f9df5ada-2262-4b2e-be06-d6582a0f28ba)

Diagrama da arquitetura

---

# 📌 **Requisitos Funcionais**

| Funcionalidade | Status |
| --- | --- |
| ✅ Autenticação Médico | Login por CRM e senha |
| ✅ Cadastro/Edição de Horários Disponíveis (Médico) | CRUD de horários |
| ✅ Aceite ou Recusa de Consultas Médicas (Médico) | Atualização do status |
| ✅ Autenticação do Usuário (Paciente) | Login por email ou CPF |
| ✅ Busca por Médicos (Paciente) | Filtro por especialidade |
| ✅ Agendamento de Consultas (Paciente) | Escolha de horário disponível e agendamento |
| ✅ Cancelamento da Consulta (Paciente) | Obrigatório justificar cancelamento |
| ✅ Cancelamento pelo Médico | Opcional com justificativa |

## 🛠️ **Endpoints principais**

| Método | Endpoint | Descrição | Permissões |
| --- | --- | --- | --- |
| POST | `/api/login` | Autenticação usuário | Médico e Paciente |
| GET | `/api/medicos` | Lista médicos disponíveis | Paciente |
| GET | `/api/horarios` | Busca horários disponíveis do médico selecionado | Paciente |
| POST | `/api/agendamentos` | Agendar consulta | Paciente |
| PUT | `/api/agendamentos/confirmar` | Aceitar/recusar consulta | Médico |
| PUT | `/api/agendamentos/cancelar-paciente` | Cancelar consulta | Paciente |
| PUT | `/api/agendamentos/cancelar-medico` | Cancelar consulta | Médico |

---

# 📈 **Requisitos Não-Funcionais**

## ✅ Alta Disponibilidade

- A arquitetura em **Kubernetes** assegura disponibilidade 24/7, com distribuição da aplicação em **múltiplos pods**, permitindo **escalabilidade horizontal automática**.
- A replicação de pods garante **tolerância a falhas** e **redundância**, mantendo o sistema operacional mesmo em caso de falhas pontuais.

## 📊 **Escalabilidade**

- O uso de **Kubernetes** possibilita o escalonamento dinâmico para suportar picos de acesso, com capacidade projetada para até **20.000 usuários simultâneos**.
- A integração com **RabbitMQ** (para processar agendamentos de forma assíncrona) e **Redis** (para cache de horários) **alivia a carga sobre o banco de dados** e **reduz a necessidade de escalar novos pods**, otimizando recursos e mantendo alta performance.

## 🔒 **Segurança**

- Autenticação JWT com claims específicas para médico e paciente.
- Senhas protegidas com BCrypt.
- Dados sensíveis tratados conforme boas práticas (hashing e validação rigorosa de permissões).

---

# 🧪 **Testes**

- Testes unitários e integração implementados usando xUnit para garantir robustez e qualidade do código.

---

# 📦 **Infraestrutura e CI/CD**

- **Docker Compose:** Ambiente local com SQL Server, Redis, RabbitMQ e API.
- **Kubernetes:** Escalabilidade e alta disponibilidade para produção.
- **Pipeline CI/CD:** Automação de builds e deploy, garantindo agilidade e qualidade na entrega.

---

# 📌 **Modelagem de Dados**

**Tabelas principais:**

- **Usuarios** (`Id`, `Email`, `SenhaHash`, `TipoUsuario`)
- **Medicos** (`CRM`, `Especialidade`)
- **Pacientes** (`CPF`)
- **HorariosDisponiveis** (`MedicoId`, `DataHorario`, `Status`, `Valor`)
- **Agendamentos** (`PacienteId`, `HorarioId`, `Status`, `JustificativaCancelamento`)

📌 **Relacionamentos:**

- **Usuario** 1:1 Médico
- **Usuario** 1:1 Paciente
- **Médico** 1:N HoráriosDisponíveis
- **Horário** 1:1 Agendamento
- **Paciente** 1:N Agendamentos

## **Tabelas e Relacionamentos**

### **1️⃣ Tabela `Usuarios`**

Armazena informações básicas e credenciais de acesso.

| Campo | Tipo de Dado | Descrição |
| --- | --- | --- |
| `Id` | `GUID` (PK) | Identificador único do usuário |
| `Email` | `string` (Único / Opcional) | Apenas para pacientes (médicos não usam para login) |
| `SenhaHash` | `string` | Armazena a senha criptografada |
| `TipoUsuario` | `enum (Medico, Paciente)` | Define o tipo de usuário |

📌 **Regras:**

- **Login deve receber o TipoUsuario para diferenciar**
- **Médico faz login com CRM** (não usa e-mail, mas pode ter cadastrado).
- **Paciente faz login com e-mail ou CPF**.
- **Senha deve ser armazenada com hash seguro**.

---

### **2️⃣ Tabela `Medicos`**

Contém dados específicos dos médicos.

| Campo | Tipo de Dado | Descrição |
| --- | --- | --- |
| `Id` | `GUID` (PK) | Identificador único do médico |
| `UsuarioId` | `GUID` (FK → [Usuarios.Id](http://usuarios.id/))` | Referência ao usuário |
| `CRM` | `string (Único)` | Registro profissional obrigatório |
| `Especialidade` | `enum` | Exemplo: Cardiologia, Ortopedia |

📌 **Regras:**

- O **CRM deve ser único** e será usado no login.
- A **especialidade deve ser um `enum`**, para facilitar filtros.
- O Médico **pode ter e-mail na tabela `Usuarios`**.

---

### **3️⃣ Tabela `Pacientes`**

Contém dados específicos dos pacientes.

| Campo | Tipo de Dado | Descrição |
| --- | --- | --- |
| `Id` | `GUID` (PK) | Identificador único do paciente |
| `UsuarioId` | `GUID` (FK → [Usuarios.Id](http://usuarios.id/))` | Referência ao usuário |
| `CPF` | `string (Único)` | Número do CPF, obrigatório |

📌 **Regras:**

- **CPF é único** e pode ser usado como login.
- O **Paciente pode ter e-mail na tabela `Usuarios`**.

---

### **4️⃣ Tabela `HorariosDisponiveis`**

Guarda os horários que os médicos disponibilizam para consultas.

| Campo | Tipo de Dado | Descrição |
| --- | --- | --- |
| `Id` | `GUID` (PK) | Identificador do horário |
| `MedicoId` | `GUID` (FK → [Medicos.Id](http://medicos.id/))` | Médico responsável pelo horário |
| `DataHorario` | `DateTime` | Data e hora da consulta |
| `Status` | `enum (Disponível, Reservado)` | Indica se o horário está livre |
| `Valor` | `decimal` | Valor da consulta |

📌 **Regras:**

- O **médico define seus horários** disponíveis.
- O **status muda para "Reservado" quando um paciente agenda**.
- É ideal que os horários de um médico não sejam duplicados.
- O médico deve poder editar o horário, mesmo assim deve-se evitar a duplicidade

---

### **5️⃣ Tabela `Agendamentos`**

Registra o vínculo entre médicos e pacientes para uma consulta agendada.

| Campo | Tipo de Dado | Descrição |
| --- | --- | --- |
| `Id` | `GUID` (PK) | Identificador do agendamento |
| `PacienteId` | `GUID` (FK → [Pacientes.Id](http://pacientes.id/))` | Paciente que marcou a consulta |
| `HorarioId` | `GUID` (FK → [Horarios.Id](http://horarios.id/))` | Horário da consulta |
| `Status` | `enum (Pendente, Agendado, Cancelado, Realizado)` | Estado da consulta |
| `JustificativaCancelamento` | `string (Opicional)` | Motivo do cancelamento, se aplicável |

📌 **Regras:**

- O **Paciente escolhe um horário disponível do Médico**.
- O Médico deve confirmar ou recusar o agendamento.
- A consulta pode ser **cancelada pelo Paciente, com uma justificativa obrigatória**.

---

## 🎯 **Benefícios dessa Estrutura**

✅ **Simplicidade**: Mantém usuários, médicos e pacientes organizados.

✅ **Eficiência**: `Especialidade` é um `enum`, facilitando filtros e buscas.

✅ **Facilidade de Expansão**: Possível adicionar novos tipos de usuários no futuro.

✅ **Boas Práticas**: Relacionamentos bem definidos evitam redundâncias.

✅ **Segurança**: Senhas armazenadas como hash e controle de acesso por `TipoUsuario`.

✅ **Escalabilidade**: Estrutura permite crescer sem alterações drásticas no banco.

✅ **Flexibilidade**: Pacientes podem fazer login com **CPF ou e-mail**, e médicos com **CRM**.

✅ **Performance**: Uso de `enum` e chaves estrangeiras evita dados duplicados e melhora buscas.

✅ **Registros de Agendamentos**: Possibilita controle completo do histórico de consultas.

---

# 📌 Sistema de Autenticação e Segurança

## 1. Autenticação de Usuários

### Descrição

O sistema possui dois fluxos de autenticação, diferenciando os tipos de usuários:

- **Médicos:**
    
    Efetua login utilizando o número do CRM e a senha cadastrada.
    
    *Regras:* O CRM é único e obrigatório para o login do médico.
    
- **Pacientes:**
    
    Efetua login utilizando e-mail ou CPF e senha.
    
    *Regras:* O e-mail e o CPF são únicos. O sistema deve permitir a escolha de um desses campos para autenticação.
    

### Fluxo de Autenticação

1. **Requisição de Login:**
    
    O usuário envia uma requisição para o endpoint de login com suas credenciais (CRM para médicos ou e-mail/CPF para pacientes) e a senha, além do tipo do usuário (médico ou paciente).
    
2. **Validação das Credenciais:**
    
    O backend pesquisa o usuário no banco de dados e valida a senha utilizando a comparação com o hash armazenado.
    
3. **Geração de Token JWT:**
    
    Se as credenciais forem válidas, o sistema gera um token JWT que contém informações essenciais (como o ID do usuário e o tipo de usuário) e o retorna ao cliente.
    
4. **Acesso aos Endpoints Protegidos:**
    
    O token JWT deve ser enviado no header das requisições (por exemplo, `Authorization: Bearer <token>`) e é validado a cada acesso aos endpoints protegidos.
    

---

## 2. Banco de Dados (SQL Server e Redis)

### Estrutura e Tecnologias

O sistema utiliza o **SQL Server** como banco de dados relacional principal. Essa escolha garante **alta performance**, **escalabilidade** e suporte a **funcionalidades avançadas**, essenciais para o setor de saúde, como transações, integridade referencial e modelagem relacional robusta.

Além disso, a aplicação também conta com o **Redis**, um banco de dados em memória utilizado como **cache para horários disponíveis**. Essa abordagem reduz a carga no SQL Server, **melhora significativamente a performance em horários de pico** e contribui para uma melhor experiência do usuário, com respostas mais rápidas nas buscas por médicos e horários.

Essa combinação oferece o melhor dos dois mundos:

- **SQL Server**: persistência de dados estruturados e críticos.
- **Redis**: performance e redução de latência em leituras de dados voláteis.

---

## 3. Mensageria (RabbitMQ)

### Estrutura e Tecnologias

O sistema utiliza o **RabbitMQ** como broker de mensagens para garantir um fluxo assíncrono no processo de **agendamento de consultas**. A utilização de mensageria traz **resiliência, desacoplamento e escalabilidade** ao sistema, permitindo que operações críticas não fiquem diretamente dependentes da API principal.

Quando um paciente realiza um agendamento, a requisição é publicada em uma fila no RabbitMQ. Um **worker (microserviço Agendar)** consome essa fila de forma independente, processando o agendamento e atualizando o banco de dados conforme necessário.

---

## 4. Criptografia de Senha

### Algoritmo Utilizado

Para proteger os dados sensíveis dos usuários, o sistema utiliza o algoritmo **BCrypt** para o armazenamento das senhas.

### Funcionamento

- **Geração do Hash:**
No cadastro ou atualização de senha, o sistema gera um hash:
    
    ```csharp
    string hashedPassword = BCrypt.Net.BCrypt.HashPassword("senha_do_usuario");
    ```
    
- **Verificação da Senha:**
Durante o login, a senha informada é comparada com o hash armazenado:
    
    ```csharp
    bool senhaValida = BCrypt.Net.BCrypt.Verify("senha_do_usuario", hashedPassword);
    ```
    

Essa abordagem assegura que as senhas nunca sejam armazenadas em texto puro, aumentando a segurança dos dados dos usuários.

---

## 5. Funcionamento do Login

### Endpoint de Login

- **Rota:** `/api/login`
- **Método HTTP:** POST

### Exemplo de Payload

- **Médico:**
    
    ```json
    {
      "crm": "123456",
      "senha": "senha_do_medico"
    }
    
    ```
    
- **Paciente:**
    
    ```json
    {
      "emailOuCpf": "usuario@exemplo.com",
      "senha": "senha_do_paciente"
    }
    
    ```
    

### Processo de Autenticação

1. **Recepção da Requisição:**
    
    O endpoint recebe as credenciais e o tipo de usuário.
    
2. **Busca e Validação:**
    - Para médicos, o sistema pesquisa com base no CRM.
    - Para pacientes, a busca é feita utilizando o e-mail ou CPF.
    - Em ambos os casos, a senha informada é validada com o hash armazenado.
3. **Geração e Retorno do Token JWT:**
    
    Se a validação for bem-sucedida, um token JWT é gerado e retornado, permitindo o acesso aos endpoints protegidos.
    
4. **Tratamento de Erros:**
    
    Se as credenciais estiverem incorretas, o sistema retorna uma resposta com status 403 (Forbiden) ou caso esteja sem credencial nenhuma retorna um 401 (Unauthorized).
    
---

# 🚀 How to Use – Health&Med MVP

Este repositório contém o projeto completo do MVP de Telemedicina da **Health&Med**, estruturado com duas soluções principais e arquivos prontos para execução em **Kubernetes**.

---

## 📁 Estrutura do Projeto

```
Hackathon.HealthMed/     → API principal (.NET)
├── Hackathon.HealthMed/
├── kubernetes/                     → Arquivos YAML para Kubernetes
│   └── ...                             
│
└── Hackathon.HorarioService/
    └── Hackathon.HealthMed.Agendar/  → Microserviço de Agendamento (Worker via RabbitMQ)

```

---

## 🧩 Pré-requisitos

- Docker Desktop com Kubernetes habilitado
- `kubectl` instalado e funcionando

---

## 📥 Clonando o Projeto

```bash
git clone https://github.com/LuisFernandoFernandes/Hackathon.HealthMed.git
cd Hackathon.HealthMed/kubernetes
```

---

## ☸️ Subindo o sistema no Kubernetes

Com o Kubernetes ativo no Docker Desktop, rode o comando abaixo para criar todos os serviços:

```
kubectl apply -f .
```

Isso vai subir os seguintes componentes:

- API principal (healthmed-api)
- Microserviço Agendar (worker)
- SQL Server
- Redis
- RabbitMQ
- Prometheus
- Grafana

---

## 🔌 Acessando os serviços via `port-forward`

Com os pods rodando (`kubectl get pods`), execute os seguintes comandos em terminais diferentes:

```
kubectl port-forward svc/healthmed-api 8080:8080
```

```
kubectl port-forward svc/prometheus 9090:9090
```

```
kubectl port-forward svc/grafana 3000:3000
```

```
kubectl port-forward svc/rabbitmq 15672:15672
```


---

## 🌐 Endpoints úteis

- 🧪 **Swagger API**: [http://localhost:8080/swagger](http://localhost:8080/index.html)
- 📊 **Prometheus**: http://localhost:9090
- 📈 **Grafana**: http://localhost:3000
    
    > Usuário: admin | Senha: admin (ou conforme definido no YAML)
    > 
- 📨 **RabbitMQ Admin**: http://localhost:15672
    
    > Usuário: guest | Senha: guest
    > 

# Passo a passo no Swagger

## Antes do Login

Antes do primeiro login podemos testar todas as rotas (com exceção do login claro) para que elas retornem um `401` ou `Unauthorized`,

Somente a rota de `Horario/medico/ {medicoId}` não poderá ser testada sem passar um id no formato correto, então podemos passar o seguinte id:

```jsx
11111111-1111-1111-1111-111111111111
```

## Login Médico

Em `Auth`, na rota de login, primeiro logamos com um médico.

Esse médico é cadastrado ao iniciar o sistema para fim de testes, pode ser passado assim:

```jsx
{
  "login": "123456",
  "senha": "123456",
  "tipoUsuario": 0
}
```

Aqui ele utiliza o `CRM` como login e a senha utilizada é criptografada para ser comparada com o hash salvo no banco.

o `tipoUsuario` 0 representa o médico.

Então copiamos o token gerado no login, que deverá aparecer nesse formato abaixo:

```jsx
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjExMTExMTExLTExMTEtMTExMS0xMTExLTExMTExMTExMTExMSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6Ik1lZGljbyIsImV4cCI6MTc0Mzg2NTMxMiwiaXNzIjoiSGFja2F0aG9uLkhlYWx0aE1lZCIsImF1ZCI6IkhhY2thdGhvbi5IZWFsdGhNZWQuVXNlcnMifQ.mYqaPhTKo5vttOWKYGsDZonsrYRiYeMudh8xQmaewYY"
}
```

Clicando no botão `Authorize` no canto superior direito do Swagger, devemos inserir o token no formato abaixo:

```jsx
Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjExMTExMTExLTExMTEtMTExMS0xMTExLTExMTExMTExMTExMSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6Ik1lZGljbyIsImV4cCI6MTc0Mzg4MzczMywiaXNzIjoiSGFja2F0aG9uLkhlYWx0aE1lZCIsImF1ZCI6IkhhY2thdGhvbi5IZWFsdGhNZWQuVXNlcnMifQ.1VsO1P2hX6K0ibxdXMWOrIk4LF6-98JGG3d8fUFQcL4
```

Agora, logado como médico podemos testar as outras rotas.

## Rotas Proibidas para Médicos

Podemos testar as rotas proibidas para que retornem um `403` ou `Forbidden`

## Em Agendamento:

`Post` /api/Agendamento

`Put` /api/Agendamento/cancelar-paciente

## Em Horario:

`Get` /api/Horario/medico/{medicoId}

Pode usar o id abaixo para testar.

```jsx
11111111-1111-1111-1111-111111111111
```

## Em Medico:

`Get` /api/Medico

## Horários para Médicos

No `Post` podemos cadastrar 2 horários iguais mostrando o erro.

```jsx
{
  "medicoId": null,
  "dataHorario": "2025-03-27T14:38:32.542Z",
  "valor": 0
}
```

Também podemos cadastrar 2 horários diferentes, depois tentar alterar para que fiquem iguais no put, mostrando erro.

```jsx
{
  "medicoId": null,
  "dataHorario": "2025-03-28T14:38:32.542Z",
  "valor": 0
}
```

Cadastrar mais horários para que possam ser agendados posteriormente e cancelados por médicos e pacientes, além de confirmado por médicos.

Utilizar o `Get` para pegar os visualizar os horários do médico. Essa rota não mostra horários de dias anteriores.

## Login Paciente

Em `Auth`, na rota de login, logamos com um paciente.

Esse paciente é cadastrado ao iniciar o sistema para fim de testes, pode ser passado assim:

```jsx
{
  "login": "maria@paciente.com",
  "senha": "123456",
  "tipoUsuario": 1
}
```

O login do `Paciente` também pode ser feito pelo `CPF` .

```jsx
{
  "login": "12345678900",
  "senha": "123456",
  "tipoUsuario": 1
}
```

Aqui ele utiliza o `e-mail`  ou `CPF` como `login` e a `senha` utilizada é criptografada para ser comparada com o hash salvo no banco.

o `tipoUsuario` 1 representa o paciente.

Então copiamos o token gerado no login, que deverá aparecer nesse formato abaixo:

```jsx
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjIyMjIyMjIyLTIyMjItMjIyMi0yMjIyLTIyMjIyMjIyMjIyMiIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlBhY2llbnRlIiwiZXhwIjoxNzQzODcyMDU2LCJpc3MiOiJIYWNrYXRob24uSGVhbHRoTWVkIiwiYXVkIjoiSGFja2F0aG9uLkhlYWx0aE1lZC5Vc2VycyJ9.iscM_y9R33-ugbMlc-fPtFg1PSBRUyKQ-jGNIlgNJJ4"
}
```

Clicando no botão `Authorize` no canto superior direito do Swagger, devemos inserir o token no formato abaixo:

```jsx
Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjIyMjIyMjIyLTIyMjItMjIyMi0yMjIyLTIyMjIyMjIyMjIyMiIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlBhY2llbnRlIiwiZXhwIjoxNzQzODgzNzkxLCJpc3MiOiJIYWNrYXRob24uSGVhbHRoTWVkIiwiYXVkIjoiSGFja2F0aG9uLkhlYWx0aE1lZC5Vc2VycyJ9.v-a2a2H4tMLe6YwppKyk7YBUyamG47_4fNJX3-O-IBQ
```

Agora, logado como paciente podemos testar as outras rotas.

## Rotas Proibidas para Pacientes

Podemos testar as rotas proibidas para que retornem um 403 ou Forbidden

### Em Agendamento:

`Put` /api/Agendamento/confirmar

`Put` /api/Agendamento/cancelar-medico

### Em Horario:

`Post` /api/Horario

`Put` /api/Horario

`Get` /api/Horario

## Pesquisa de médicos pelo Pacientes

O `Get` da rota de médicos pode ser utilizado pelo paciente para retornar a lista de médicos. Funciona tanto passando uma `especialidade` ou não, que no caso retorna todos os médicos.

Aqui, podemos pegar o Id do  médico que utilizamos para cadastrar o horário, basta procurar o médico com o `CRM` “123456”

```jsx
{
    "id": "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb",
    "nome": "João Médico",
    "crm": "123456",
    "especialidade": 4
}
```

## Pesquisa de Horário de um Médico pelo Paciente

No `Get **/api/Horario/medico/{medicoId}`** podemos pegar os horários disponíveis de um médico.

Podemos usar o Id do médico abaixo.

```jsx
bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb
```

A resposta mostrará uma lista de horários disponíveis.

```jsx
[ 
  {
    "id": "c046ff04-f61c-4444-b51a-7605efddcce1",
    "dataHorario": "2025-04-16T12:12:28.123",
    "status": 0,
    "valor": 0
  }
]
```

Então podemos copiar o `Id` do horário que usaremos para o agendamento.

## Agendamento pelo Paciente

No `Post **/api/Agendamento**` o Paciente pode agendar um horário passando o **Id** do horário disponível desejado. 

```jsx
{
  "horarioId": "73874BBE-83D5-4A1E-B40F-6BFCD3F9CA38"
}
```

Horários não disponíveis retornarão um `400` avisando que o horário não está disponível.

Caso o horário esteja disponível para o agendamento, será retornada uma mensagem informando que o agendamento está sendo realizado.

No `Get` é possível consultar os agendamentos do Paciente, esse mesmo método pode ser chamado por um médico e retornara os agendamentos do médico.

Aqui podemos agendar ao menos 2 horários, cancelar um deles, tornando o horário disponível novamente pelo `Put **/api/Agendamento/cancelar-medico**`

O cancelamento pelo paciente, requer uma `justificativa`.

```jsx
{
  "agendamentoId": "7B11E151-E281-4CAA-BCC5-C900549A2EB6",
  "justificativa": "Esse campo é obrigatório para essa rota"
}
```

Então agendamos o mesmo horário novamente.

Então com 2 agendamentos do paciente para o médico, podemos logar novamente com o médico.

## Confirmar/Cancelar Agendamento pelo Médico

Após logar novamente com o médico podemos consultar os agendamentos do médico.

No `Get **/api/Agendamento**` quando chamado com um usuário médico, conseguimos visualizar os agendamentos do médico, então podemos pegar os `Id` de agendamentos.

O médico pode cancelar um agendamento.

No `Put **/api/Agendamento/cancelar-medico`** conseguimos passar um `Id` de `Agendamento` para ser cancelado, além disso é possível, mas não obrigatório passar uma `justificativa`.

```jsx
{
  "agendamentoId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "justificativa": null
}
```

Por fim, é possível confirmar um **agendamento**.

No `Put **/api/Agendamento/confirmar**` é possível confirmar um `Agendamento` .

```jsx
{
  "agendamentoId": "7B11E151-E281-4CAA-BCC5-C900549A2EB6",
  "aceitar": true,
  "justificativa": "string"
}
```
