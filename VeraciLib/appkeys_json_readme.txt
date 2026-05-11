ATENÇÃO:

Para o projeto funcionar, você precisa ter acesso à API do X (antigo Twitter), à API do OpenAI (ChatGPT) e a um banco de dados.
Você pode criar uma conta no site oficial de cada um e gerar as chaves necessárias no X e na OpenAI.
As chaves devem ser colocadas no arquivo appkeys.json, que deve estar na mesma pasta deste arquivo readme.txt.
Esse arquivo appkeys.json não é versionado no github para não expor as chaves.
O build copia automaticamente o appkeys.json para os diretórios de saída de todos os projetos.

==========appkeys.json===========
{

  "xClientId": "<<CLIENT_ID>>",
  "xClientSecret": "<<CLIENT_SECRET>>",
  "xApiKey": "<<API_KEY>>",
  "xApiSecret": "<<API_SECRET>>",
  "xAccessToken": "<<ACCESS_TOKEN>>",
  "xAccessSecret": "<<ACCESS_SECRET>>",
  "xBearerToken": "<<BEARER_TOKEN>>",

  "xUserId": "<<ID_NUMERICO_DA_CONTA_DO_BOT>>",
  "xUserName": "<<USERNAME_DO_BOT>>",

  "openAIKey": "<<OPEN_AI_API_KEY>>",

  "sendGridKey": "<<SEND_GRID_API>>",

  "dbConnection": "<<CONNECTION_STRING>>"

}
================================


API DO X (TWITTER)
------------------
O App do X deve estar associado a um Project no Developer Portal (developer.twitter.com).
As chaves xApiKey, xApiSecret, xAccessToken e xAccessSecret são usadas para postar tweets (OAuth 1.0a).
O xBearerToken é usado para leitura (busca de menções, threads, perfis de usuário).
O xUserId é o ID numérico da conta do bot (não o username). O xUserName é o username sem @.

TIERS DE ACESSO:
  - Free (gratuito): o bot usa GET /2/tweets/search/recent para buscar menções.
    Limite: 1 requisição a cada 15 minutos. O bot verifica menções a cada 16 min.
    Máximo de 10 menções por ciclo.

  - Basic ($100/mês) ou superior: permite usar GET /2/users/:id/mentions.
    Para usar, troque a URL de busca de volta para o endpoint de menções em
    VeraciBot/Program.cs e reduza o Thread.Sleep para 60000 (1 minuto).


BANCO DE DADOS
--------------
O projeto suporta dois modos:

  1. SQLite (desenvolvimento local — sem instalação):
     "dbConnection": "Data Source=veracibot.db"
     O arquivo veracibot.db é criado automaticamente na primeira execução.

  2. SQL Server (produção):
     "dbConnection": "Server=<<SERVIDOR>>;Database=VeraciBot;Trusted_Connection=True;MultipleActiveResultSets=true"
     Para usar SQL Server, troque UseSqlite por UseSqlServer nos arquivos:
       - VeraciBot/Program.cs
       - VeraciApp/Program.cs
     E substitua o pacote NuGet Microsoft.EntityFrameworkCore.Sqlite por
     Microsoft.EntityFrameworkCore.SqlServer nos três projetos (.csproj).

  Após configurar o banco, rode as migrations pelo VeraciApp para criar as tabelas:
     cd VeraciApp
     dotnet ef database update


SENDGRID (e-mail)
-----------------
Usado apenas pelo VeraciApp para envio de e-mails de confirmação de cadastro.
Se a chave estiver vazia, o envio é ignorado silenciosamente (apenas loga no console).
Para desenvolvimento local, pode deixar o campo vazio:
  "sendGridKey": ""
Para produção, crie uma conta gratuita em sendgrid.com e gere a chave de API.


OPENAI
------
Usado pelo bot para identificar comandos e avaliar threads.
Modelo utilizado: gpt-4o.
Crie uma chave em platform.openai.com.
