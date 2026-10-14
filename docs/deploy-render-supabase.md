# Runbook: deploy em staging e produção (Render + Azure SQL)

Arquitetura dos ambientes na nuvem:

| Ambiente | API (container) | Banco | Branch | Aprovação |
|---|---|---|---|---|
| staging | Render Web Service `orbital-sos-api-staging` (Free, virginia) | Azure SQL `orbitalsos-staging` (free offer) | `develop` | automática |
| production | Render Web Service `orbital-sos-api-prod` (Free, virginia) | Azure SQL `orbitalsos-prod` (free offer) | `main` | manual (environment `production`) |

A imagem é a mesma nos dois ambientes (`ghcr.io/vsilva1997/orbital-sos-api:<sha>`). O que muda são as
variáveis de ambiente de cada serviço. Nenhum segredo fica no Git.

## 1. Azure SQL Database (free offer)

1. Ative o **Azure for Students** em <https://azure.microsoft.com/free/students> com o e-mail da FIAP (não pede cartão).
2. No portal, abra **SQL databases > Create**. No banner *"Want to try Azure SQL Database for free?"*, clique em **Apply offer**.
   - Resource group: `rg-orbital-sos` (novo)
   - Database name: `orbitalsos-staging`
   - Server: **Create new**
     - nome único, ex.: `orbital-sos-sql-<seu-rm>`
     - região **East US 2** (ou East US; se a política do Azure for Students bloquear, escolha outra região dos EUA)
     - autenticação **SQL authentication**, com login de admin e senha forte (guarde num gerenciador de senhas)
   - *Behavior when free limit is reached*: **Auto-pause the database until next month** (garante custo zero)
   - Aba **Networking**: Public endpoint. Em *Add current client IP address*, marque **Yes** (para usar o Query editor).
3. Repita o passo 2 para o banco `orbitalsos-prod` **no mesmo servidor**. Cada banco tem a própria cota gratuita.
4. Em cada banco, abra **Connection strings > ADO.NET**. Troque `{your_password}` e acrescente `Connect Timeout=60;`, porque o banco
   serverless pode levar até 1 minuto para retomar depois de uma pausa. Exemplo:

   ```text
   Server=tcp:orbital-sos-sql-xxx.database.windows.net,1433;Initial Catalog=orbitalsos-staging;User ID=<login>;Password=<senha>;Encrypt=True;TrustServerCertificate=False;Connect Timeout=60;
   ```

> Opcional (boa prática): em vez do admin do servidor, crie um usuário por banco no **Query editor**
> (`CREATE USER orbital_app WITH PASSWORD = '...'; ALTER ROLE db_owner ADD MEMBER orbital_app;`).
> O `db_owner` é necessário porque a API aplica as migrations na inicialização.

## 2. Imagem pública no GHCR

Depois do primeiro push na `develop`, o job **docker** publica `ghcr.io/vsilva1997/orbital-sos-api`.
Para o Render baixar a imagem sem credencial, abra GitHub > seu perfil > **Packages** > `orbital-sos-api` > **Package settings** >
**Change visibility** > **Public**. O repositório já é público.

## 3. Render (Blueprint)

1. No Render, use o **workspace pessoal**, não o de outro projeto. Abra **New > Blueprint** e conecte o repositório
   `vsilva1997/orbital-sos-api`, branch `develop`. O Render lê o [`render.yaml`](../render.yaml).
2. Na criação, o Render pede as variáveis `sync: false` de cada serviço:
   - `ConnectionStrings__DefaultConnection`: a connection string do banco do ambiente (staging ou prod)
   - `Admin__Email` / `Admin__Password`: credenciais do administrador inicial de cada ambiente

   O `Jwt__Key` é gerado pelo próprio Render (`generateValue: true`).
3. O primeiro deploy do serviço de produção pode falhar, porque a tag `:main` ainda não existe. Isso é esperado:
   o deploy de verdade é feito pelo pipeline.
4. Em cada serviço, abra **Connect > Outbound** e copie as faixas de IP de saída. No Azure, em **SQL server > Networking >
   Firewall rules**, adicione cada faixa (Start IP / End IP).

## 4. Secrets e variáveis no GitHub (environments)

O deploy hook fica em cada serviço, em **Settings > Deploy Hook**. Ele é segredo: o comando abaixo pede o valor sem mostrá-lo.

```bash
gh secret set RENDER_DEPLOY_HOOK_URL --env staging --repo vsilva1997/orbital-sos-api
```

```bash
gh secret set RENDER_DEPLOY_HOOK_URL --env production --repo vsilva1997/orbital-sos-api
```

A URL pública de cada serviço não é segredo. Ela é usada no smoke test e no link do environment:

```bash
gh variable set APP_URL --env staging --repo vsilva1997/orbital-sos-api --body "https://orbital-sos-api-staging.onrender.com"
```

```bash
gh variable set APP_URL --env production --repo vsilva1997/orbital-sos-api --body "https://orbital-sos-api-prod.onrender.com"
```

## 5. Fluxo de deploy

1. Merge na `develop`: build, testes, compose e imagem; depois o **deploy-staging** chama o deploy hook com
   `imgURL=ghcr.io/vsilva1997/orbital-sos-api:<sha>` e espera o `/health` responder `Healthy` com `version = <sha>`.
2. PR `develop -> main` e merge: mesmo pipeline. O **deploy-production** fica *Waiting* até alguém aprovar em
   **Actions > run > Review deployments**.
3. Para reexecutar um deploy que falhou por configuração: `gh run rerun <run-id> --failed`.

## Observações do plano Free

- Os serviços Free do Render dormem depois de 15 minutos sem tráfego. O primeiro acesso leva de 30 a 60 s.
- O Azure SQL serverless pausa quando fica ocioso. A API usa `EnableRetryOnFailure` e o smoke test tem retry de até 10 min.
- A cota do Azure SQL free é de 100.000 vCore-segundos e 32 GB por banco por mês. Com auto-pause não há cobrança.
