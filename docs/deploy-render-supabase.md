# Runbook: deploy em staging e produção (Render + Supabase)

Arquitetura dos ambientes na nuvem:

| Ambiente | API (container) | Banco (PostgreSQL no Supabase) | Branch | Aprovação |
|---|---|---|---|---|
| staging | Render Web Service `orbital-sos-api-staging` (Free, virginia) | schema `staging`, usuário `orbital_staging` | `develop` | automática |
| production | Render Web Service `orbital-sos-api-prod` (Free, virginia) | schema `production`, usuário `orbital_prod` | `main` | manual (environment `production`) |

A imagem é a mesma nos dois ambientes (`ghcr.io/vsilva1997/orbital-sos-api:<sha>`). O que muda são as variáveis de
ambiente de cada serviço. Os dois ambientes ficam no mesmo projeto Supabase (plano Free, região us-east-1), isolados
por schema. Cada usuário de banco só tem permissão no próprio schema, e o `search_path` dele aponta para esse schema.
Nenhum segredo fica no Git.

## 1. Supabase (projeto e usuários)

1. Em <https://supabase.com/dashboard>, clique em **New project**:
   - Name: `orbital-sos-api`
   - Database password: clique em **Generate a password**. A API não usa essa senha, então não precisa guardá-la.
   - Region: **East US (North Virginia)**, a mais próxima do Render (virginia)
   - Plano **Free**
2. Quando o projeto terminar de subir, abra **SQL Editor > New query**. Cole o conteúdo de `.segredos/supabase-setup.sql`,
   que é gerado localmente e fica fora do Git, e clique em **Run**. O script cria:
   - os schemas `staging` e `production`;
   - os usuários `orbital_staging` e `orbital_prod`, com as senhas como hash SCRAM-SHA-256;
   - as permissões de cada usuário no próprio schema.
3. Clique em **Connect** (topo da página) > **Session pooler** e anote o *host* (ex.: `aws-0-us-east-1.pooler.supabase.com`)
   e o *project ref* (o trecho depois de `postgres.` no usuário). A conexão direta do plano Free é só IPv6, e o Render
   precisa de IPv4. Por isso a API usa o **Session pooler**, na porta 5432.
4. Com o ref e o host, gere localmente as variáveis de cada ambiente. Elas ficam em `.segredos/render-staging.env` e
   `.segredos/render-production.env`, e a connection string tem o formato:

   ```text
   Host=<host-do-pooler>;Port=5432;Database=postgres;Username=orbital_staging.<ref>;Password=<senha>;SSL Mode=Require;Maximum Pool Size=5;Timeout=30
   ```

## 2. Imagem no GHCR

O job **docker** publica `ghcr.io/vsilva1997/orbital-sos-api`. Como o repositório é público, o pacote também é, e o
Render baixa a imagem sem credencial. Confira se precisar: GitHub > Packages > `orbital-sos-api` > visibilidade *Public*.

## 3. Render (Blueprint)

1. No Render, use o **workspace pessoal**, não o de outro projeto. Abra **New > Blueprint** e conecte o repositório
   `vsilva1997/orbital-sos-api`, branch `develop`. O Render lê o [`render.yaml`](../render.yaml).
2. Na criação, o Render pede as variáveis `sync: false` de cada serviço. Copie os valores de `.segredos/render-<ambiente>.env`:
   - `ConnectionStrings__DefaultConnection`
   - `Admin__Email` / `Admin__Password`

   O `Jwt__Key` é gerado pelo próprio Render (`generateValue: true`). O `Database__Schema` já vem do `render.yaml`.
3. O Render valida se as imagens existem: enquanto a tag `:main` não existir (antes do primeiro merge na `main`), crie o
   serviço de produção depois que o job **docker** da `main` publicar a imagem. O deploy de produção espera a aprovação
   manual, e dá tempo de criar o serviço nesse intervalo. Alternativa sem Blueprint: **New > Web Service > Existing Image**
   com os mesmos valores do `render.yaml` (plano Free, região Virginia, health check `/health`).

## 4. Secrets e variáveis no GitHub (environments)

O deploy hook fica em cada serviço, em **Settings > Deploy Hook**. Ele é segredo: o comando abaixo pede o valor sem mostrá-lo.

```bash
gh secret set RENDER_DEPLOY_HOOK_URL --env staging --repo vsilva1997/orbital-sos-api
```

```bash
gh secret set RENDER_DEPLOY_HOOK_URL --env production --repo vsilva1997/orbital-sos-api
```

A URL pública de cada serviço não é segredo. Ela fica em variáveis do repositório, usadas pelo smoke test do deploy,
pelo link do environment e pelo health check diário:

```bash
gh variable set STAGING_URL --repo vsilva1997/orbital-sos-api --body "https://orbital-sos-api-staging.onrender.com"
```

```bash
gh variable set PRODUCTION_URL --repo vsilva1997/orbital-sos-api --body "https://orbital-sos-api-prod.onrender.com"
```

## 5. Fluxo de deploy

1. Merge na `develop`: build, testes, compose e imagem; depois o **deploy-staging** chama o deploy hook com
   `imgURL=ghcr.io/vsilva1997/orbital-sos-api:<sha>` e espera o `/health` responder `Healthy` com `version = <sha>`.
   Na primeira inicialização, a API aplica as migrations no schema do ambiente.
2. PR `develop -> main` e merge: mesmo pipeline. O **deploy-production** fica *Waiting* até alguém aprovar em
   **Actions > run > Review deployments**.
3. Para reexecutar um deploy que falhou por configuração: `gh run rerun <run-id> --failed`.

## Observações do plano Free

- Os serviços Free do Render dormem depois de 15 minutos sem tráfego. O primeiro acesso leva de 30 a 60 s.
- Projetos Free do Supabase são pausados depois de 7 dias sem atividade. O workflow
  [`healthcheck.yml`](../.github/workflows/healthcheck.yml) chama o `/health` dos dois ambientes todo dia, o que mantém
  API e banco ativos e avisa se algum ambiente cair.
- A API usa `EnableRetryOnFailure`, e o smoke test do pipeline tem retry de até 10 minutos.
