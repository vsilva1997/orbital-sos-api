#!/usr/bin/env bash
# Gera entrega.zip com o projeto versionado (código, Dockerfile, compose, workflows,
# .env.example, README, docs e scripts). Usa git archive, então bin/, obj/, .git/ e .env
# nunca entram: só o que está commitado.
#
#   scripts/empacotar.sh [ref]     (padrão: HEAD)
set -euo pipefail

cd "$(git rev-parse --show-toplevel)"
ref="${1:-HEAD}"

if [[ -n "$(git status --porcelain)" ]]; then
  echo "Aviso: há alterações não commitadas (ex.: prints novos). Elas NÃO entram no zip; faça commit antes." >&2
fi

rm -f entrega.zip
git archive --format=zip --prefix=orbital-sos-api/ -o entrega.zip "$ref"

# Conferência: nada de build, git ou .env dentro do pacote
if unzip -Z1 entrega.zip | grep -E '/(bin|obj|\.git)/|(^|/)\.env$'; then
  echo "ERRO: o zip contém arquivos que não deveriam ser entregues (listados acima)" >&2
  exit 1
fi

echo "entrega.zip gerado a partir de $(git rev-parse --short "$ref"): $(unzip -Z1 entrega.zip | grep -vc '/$') arquivos, $(du -h entrega.zip | cut -f1)"
