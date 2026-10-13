#!/usr/bin/env bash
# Smoke test da API Orbital SOS (usado no pipeline e localmente).
#
#   scripts/smoke-test.sh <url-base> [versao-esperada] [--full]
#
#   versao-esperada  SHA que o /health deve informar (garante que a versão nova está no ar)
#   --full           também testa register, login, endpoint protegido e login do admin
#                    (admin só se ADMIN_EMAIL e ADMIN_PASSWORD estiverem definidos)
#
#   SMOKE_TIMEOUT    tempo máximo de espera do /health em segundos (padrão 600)
#   SMOKE_INTERVAL   intervalo entre tentativas em segundos (padrão 15)
#
# Requer curl e jq.
set -euo pipefail

BASE_URL="${1:?informe a URL base, ex.: http://localhost:8080}"
BASE_URL="${BASE_URL%/}"
EXPECTED_VERSION="${2:-}"
MODE="${3:-}"
TIMEOUT="${SMOKE_TIMEOUT:-600}"
INTERVAL="${SMOKE_INTERVAL:-15}"

log() { printf '[smoke] %s\n' "$*"; }
fail() { log "FALHA: $*"; exit 1; }

# 1) /health com retry: espera a API e o banco ficarem saudáveis na versão esperada
deadline=$(( $(date +%s) + TIMEOUT ))
attempt=0
while :; do
  attempt=$(( attempt + 1 ))
  body=$(curl -sS --max-time 60 "$BASE_URL/health" 2>/dev/null || true)
  status=$(jq -r '.status // empty' <<<"$body" 2>/dev/null || true)
  version=$(jq -r '.version // empty' <<<"$body" 2>/dev/null || true)
  if [[ "$status" == "Healthy" && ( -z "$EXPECTED_VERSION" || "$version" == "$EXPECTED_VERSION" ) ]]; then
    log "/health OK na tentativa $attempt: $body"
    break
  fi
  if (( $(date +%s) >= deadline )); then
    fail "/health não ficou Healthy na versão '${EXPECTED_VERSION:-qualquer}' em ${TIMEOUT}s. Última resposta: ${body:-<vazia>}"
  fi
  log "aguardando (tentativa $attempt): status='${status:-sem resposta}' versão='${version:-?}'"
  sleep "$INTERVAL"
done

# 2) Swagger ativo
code=$(curl -sS -o /dev/null -w '%{http_code}' "$BASE_URL/swagger/v1/swagger.json")
[[ "$code" == "200" ]] || fail "/swagger/v1/swagger.json retornou HTTP $code"
log "/swagger OK"

[[ "$MODE" == "--full" ]] || { log "smoke test concluído"; exit 0; }

post_json() { curl -sS -f -X POST "$BASE_URL$1" -H 'Content-Type: application/json' -d "$2"; }

# 3) register -> login -> endpoint protegido
email="smoke-$(date +%s)-$RANDOM@orbitalsos.dev"
password="Smoke-$(openssl rand -hex 8)"
reg=$(post_json /api/auth/register "$(jq -n --arg e "$email" --arg p "$password" '{name:"Smoke Test",email:$e,password:$p,role:0}')") \
  || fail "POST /api/auth/register"
[[ "$(jq -r .role <<<"$reg")" == "Usuario" ]] || fail "register deveria criar Usuario mesmo com role=0: $reg"
log "register OK (role=Usuario mesmo enviando role=0)"

token=$(post_json /api/auth/login "$(jq -n --arg e "$email" --arg p "$password" '{email:$e,password:$p}')" | jq -r .token) \
  || fail "POST /api/auth/login"
[[ -n "$token" && "$token" != "null" ]] || fail "login não retornou token"
log "login OK"

code=$(curl -sS -o /dev/null -w '%{http_code}' "$BASE_URL/api/sosrequests")
[[ "$code" == "401" ]] || fail "GET /api/sosrequests sem token deveria ser 401, veio $code"
code=$(curl -sS -o /dev/null -w '%{http_code}' -H "Authorization: Bearer $token" "$BASE_URL/api/sosrequests")
[[ "$code" == "200" ]] || fail "GET /api/sosrequests com token retornou HTTP $code"
log "endpoint protegido OK (401 sem token, 200 com token)"

# 4) Admin inicial criado a partir de Admin__Email / Admin__Password
if [[ -n "${ADMIN_EMAIL:-}" && -n "${ADMIN_PASSWORD:-}" ]]; then
  role=$(post_json /api/auth/login "$(jq -n --arg e "$ADMIN_EMAIL" --arg p "$ADMIN_PASSWORD" '{email:$e,password:$p}')" | jq -r .role) \
    || fail "login do admin"
  [[ "$role" == "Admin" ]] || fail "login do admin retornou role '$role'"
  log "login do admin OK"
fi

log "smoke test completo concluído"
