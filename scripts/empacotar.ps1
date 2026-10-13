# Gera entrega.zip com o projeto versionado (código, Dockerfile, compose, workflows,
# .env.example, README, docs e scripts). Usa git archive, então bin/, obj/, .git/ e .env
# nunca entram: só o que está commitado.
#
#   pwsh scripts/empacotar.ps1 [-Ref HEAD]
param([string]$Ref = 'HEAD')
$ErrorActionPreference = 'Stop'

Set-Location (git rev-parse --show-toplevel)

if (git status --porcelain) {
    Write-Warning 'Há alterações não commitadas (ex.: prints novos). Elas NÃO entram no zip; faça commit antes.'
}

Remove-Item entrega.zip -ErrorAction SilentlyContinue
git archive --format=zip --prefix=orbital-sos-api/ -o entrega.zip $Ref
if ($LASTEXITCODE -ne 0) { throw 'git archive falhou' }

# Conferência: nada de build, git ou .env dentro do pacote
Add-Type -AssemblyName System.IO.Compression.FileSystem
$zip = [System.IO.Compression.ZipFile]::OpenRead((Resolve-Path entrega.zip))
try {
    $bad = $zip.Entries | Where-Object { $_.FullName -match '/(bin|obj|\.git)/' -or $_.FullName -match '(^|/)\.env$' }
    if ($bad) { throw "O zip contém arquivos que não deveriam ser entregues: $($bad.FullName -join ', ')" }
    $files = ($zip.Entries | Where-Object { -not $_.FullName.EndsWith('/') }).Count
    $size = [math]::Round((Get-Item entrega.zip).Length / 1KB)
    Write-Host "entrega.zip gerado a partir de $(git rev-parse --short $Ref): $files arquivos, $size KB"
} finally {
    $zip.Dispose()
}
