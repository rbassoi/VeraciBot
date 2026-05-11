# VeraciBot — O Bot de Verificação de Fatos Aprovado pelo Ministério da Verdade

> *"Quem ousa discordar da narrativa oficial está sujeito a 17 anos de cadeia sem anistia."*

VeraciBot é um bot para X (antigo Twitter) que monitora menções a `@veracibot`, usa GPT-4o para analisar discussões e atribui pontuações humorísticas aos participantes com base em como a narrativa se sai contra a lógica. O projeto inclui um bot de console em loop contínuo e um painel web em Blazor Server para administração.

---

## Estrutura do Projeto

O repositório é uma solução .NET 9 com três projetos:

| Projeto | Tipo | Descrição |
|---|---|---|
| `VeraciBot` | Console App | Bot que monitora o X e processa comandos a cada 60 segundos |
| `VeraciApp` | Blazor Server | Painel web com autenticação via X (OAuth 1.0a) |
| `VeraciLib` | Class Library | Modelos de dados, DbContext, configuração de chaves |

---

## Como Funciona

### 1. Monitoramento de Menções

O bot roda em loop infinito verificando as menções ao `@veracibot` via API do X v2. A cada iteração:

- Busca menções desde o último tweet processado (persistido no banco)
- Filtra tweets já processados (deduplicação por ID)
- Verifica se o autor está autorizado a usar o bot

### 2. Sistema de Convites

O acesso ao bot é fechado — funciona por convite:

- Usuários autorizados podem convidar outros via `@veracibot @usuario convite`
- O convidado recebe uma notificação e pode aceitar ou recusar
- Cada usuário tem até **5 convites** para distribuir
- Status possíveis: `Não Autorizado (0)`, `Autorizado (1)`, `Convidado aguardando resposta (2)`

### 3. Identificação de Comandos via GPT-4o

O texto da menção é enviado ao GPT-4o para identificar o comando e o idioma. Comandos disponíveis:

| Código | Comando | Descrição |
|---|---|---|
| `1` | Ajuda | Explica como o bot funciona |
| `2` | Pontuação | Mostra a pontuação do usuário |
| `3` | Placar | Mostra o ranking geral |
| `10` | Convidar | Convida outro usuário para o jogo |
| `20` | Aceitar convite | Aceita o convite recebido |
| `21` | Recusar convite | Recusa o convite recebido |
| `30` | Contestar informação | Contesta o conteúdo de um tweet (em desenvolvimento) |
| `31` | Argumentar | Pede ao bot para avaliar uma discussão em thread |
| `32` | Quem está certo | Pede o veredito sobre uma thread (em desenvolvimento) |

### 4. Análise de Threads e Pontuação

Quando o comando é `Argumentar (31)`, o bot:

1. Reconstrói toda a thread recursivamente via API do X
2. Envia o diálogo completo ao GPT-4o com o prompt de juiz severo e sarcástico
3. Recebe uma nota de **1 a 5** e uma resposta irônica
4. Publica a resposta com uma imagem temática como reply
5. Atualiza os pontos dos dois autores envolvidos

**Tabela de pontuação por resultado:**

| Resultado | Quem chamou o bot | Autor original |
|---|---|---|
| 1 (Autor A totalmente certo) | +4 | -5 |
| 2 (Autor A tem mais razão) | +1 | -2 |
| 3 (Empate) | -1 | 0 |
| 4 (Autor B tem mais razão) | -3 | +2 |
| 5 (Autor A totalmente errado) | -6 | +5 |

---

## Stack Tecnológica

- **.NET 9** — runtime e linguagem C#
- **ASP.NET Core Blazor Server** — painel web interativo
- **MudBlazor 8** — componentes de UI
- **Entity Framework Core 9** + **SQL Server** — persistência de dados
- **X (Twitter) API v2** — monitoramento de menções e publicação de replies
- **Tweetinvi** — upload de imagens via API v1.1
- **OpenAI GPT-4o** — identificação de comandos e análise de threads
- **ASP.NET Core Identity** — autenticação no painel com login via X (OAuth 1.0a)
- **SendGrid** — envio de e-mails transacionais

---

## Configuração

### Pré-requisitos

- .NET 9 SDK
- SQL Server (local ou remoto)
- Conta de desenvolvedor no X com acesso à API v2 (Basic ou Pro para leitura de menções)
- Chave de API da OpenAI (acesso ao modelo `gpt-4o`)
- Conta SendGrid (para e-mails do painel)

### Arquivo `appkeys.json`

Crie o arquivo `VeraciLib/appkeys.json` (não versionado) com o seguinte formato:

```json
{
  "xClientId": "<<CLIENT_ID>>",
  "xClientSecret": "<<CLIENT_SECRET>>",
  "xApiKey": "<<API_KEY>>",
  "xApiSecret": "<<API_SECRET>>",
  "xAccessToken": "<<ACCESS_TOKEN>>",
  "xAccessSecret": "<<ACCESS_SECRET>>",
  "xBearerToken": "<<BEARER_TOKEN>>",

  "xUserId": "<<ID_NUMERICO_DO_BOT>>",
  "xUserName": "<<USERNAME_DO_BOT>>",

  "openAIKey": "<<OPEN_AI_API_KEY>>",

  "sendGridKey": "<<SEND_GRID_API>>",

  "dbConnection": "<<SQL_SERVER_CONNECTION_STRING>>"
}
```

> O `xUserId` é o ID numérico da conta `@veracibot` no X. O `xBearerToken` é usado para leitura; `xApiKey/Secret` e `xAccessToken/Secret` são usados para publicar tweets.

### Banco de Dados

O bot cria o banco automaticamente na primeira execução (`EnsureCreated`). Para o painel web, execute as migrations do EF Core:

```bash
cd VeraciApp
dotnet ef database update
```

---

## Executando

### Bot (console)

```bash
cd VeraciBot
dotnet run
```

O bot imprime logs no console e entra em loop verificando menções a cada 60 segundos.

### Painel Web

```bash
cd VeraciApp
dotnet run
```

Acesse em `https://localhost:PORT`. O login é feito via conta do X (OAuth).

---

## Banco de Dados — Tabelas Principais

| Tabela | Descrição |
|---|---|
| `Tweets` | Registro de todos os tweets processados e seus resultados |
| `TweetAuthors` | Perfis dos usuários com pontuação acumulada |
| `AuthorizedUsers` | Controle de acesso (status: não autorizado, autorizado, convidado) |
| `Configs` | Configurações persistidas (ex: último timestamp verificado) |
| `AspNetUsers` | Usuários do painel (ASP.NET Core Identity) |

---

## Observações

- As respostas do bot são sempre acompanhadas de imagens temáticas localizadas em `img/`
- O tom das respostas é deliberadamente sarcástico e irônico, parodiando o discurso de "Ministério da Verdade"
- A função `GetFullScoreBoard` ainda está pendente de implementação
- Os comandos `CMD_THREAD_FALSE (30)` e `CMD_THREAD_WHOISRIGHT (32)` estão mapeados mas ainda sem lógica implementada
- A tradução automática via GPT-4o está temporariamente desativada (`TranslatePhrase` retorna a frase original)
