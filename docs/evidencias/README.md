# Evidências (prints)

Salve cada print nesta pasta, em PNG, **com exatamente estes nomes**: o README principal e a documentação técnica apontam para eles.
Depois rode `git add docs/evidencias && git commit` antes de gerar o `entrega.zip`.

| Arquivo | O que deve aparecer | Onde tirar |
|---|---|---|
| `01-pr-checks.png` | PR `feature/devops -> develop` com os checks **Build e testes** e **Docker Compose (smoke test)** verdes | GitHub > Pull requests > PR #1 (rodapé com os checks) |
| `02-pipeline-develop.png` | Grafo do run na `develop`: build-test → compose-smoke → docker → deploy-staging, todos verdes | Actions > run "CI/CD" da branch `develop` |
| `03-testes-resumo.png` | Tabela "Testes: 14/14 aprovados" e o artefato `test-results` | Mesmo run > Summary (role até o resumo do job Build e testes) |
| `04-compose-smoke.png` | Log com `Container ... Healthy` e as linhas `[smoke] ... OK` | Run > job **Docker Compose (smoke test)** > passos "Subir API + SQL Server" e "Smoke test" |
| `05-ghcr-imagem.png` | Pacote `orbital-sos-api` com as tags `<sha>`, `develop` e `main` | GitHub > perfil > Packages > orbital-sos-api |
| `06-aprovacao-producao.png` | Tela **Review deployments** com o environment `production` aguardando aprovação | Run da `main` antes de aprovar |
| `07-pipeline-main.png` | Grafo do run na `main` até **Deploy em produção**, verde | Actions > run da branch `main` |
| `08-environments.png` | Environments `staging` e `production` com as regras (branch e revisor obrigatório) | Settings > Environments |
| `09-staging-health.png` | JSON do `/health` de staging: `"status":"Healthy"`, `"environment":"Staging"`, `"version":"<sha>"` | Navegador: URL de staging + `/health` |
| `10-staging-swagger.png` | Swagger UI de staging | URL de staging + `/swagger` |
| `11-producao-health.png` | JSON do `/health` de produção com `"environment":"Production"` | URL de produção + `/health` |
| `12-producao-swagger.png` | Swagger de produção com **POST /api/auth/register** e **/login** executados (200 e token) | URL de produção + `/swagger` |
| `13-render-servicos.png` | Dashboard do Render com os dois serviços (staging e prod) no status *Live* | dashboard.render.com |
| `14-azure-sql.png` | Bancos `orbitalsos-staging` e `orbitalsos-prod` com a oferta gratuita | Portal do Azure > SQL databases |
