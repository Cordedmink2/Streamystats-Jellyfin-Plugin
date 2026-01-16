#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

compose_file="$ROOT_DIR/docker-compose.integration.yml"

if [[ ! -f "$compose_file" ]]; then
  echo "Missing docker-compose.integration.yml"
  exit 1
fi

pushd "$ROOT_DIR" >/dev/null

if ! command -v docker >/dev/null 2>&1; then
  echo "Docker not found. Please install Docker to run integration tests."
  exit 1
fi

if ! command -v docker-compose >/dev/null 2>&1 && ! docker compose version >/dev/null 2>&1; then
  echo "docker compose not available. Please install Docker Compose."
  exit 1
fi

echo "Starting integration stack..."
if command -v docker-compose >/dev/null 2>&1; then
  docker-compose -f "$compose_file" up -d
else
  docker compose -f "$compose_file" up -d
fi

echo "Waiting for services to stabilize..."
sleep 10

JELLYFIN_URL="http://localhost:8096/System/Info/Public"
STREAMYSTATS_URL="http://localhost:3000/api/version"

echo "Checking Jellyfin public info..."
if ! curl -fsS "$JELLYFIN_URL" >/dev/null; then
  echo "Jellyfin not ready."
  exit 1
fi

echo "Checking Streamystats version endpoint..."
if ! curl -fsS "$STREAMYSTATS_URL" >/dev/null; then
  echo "Streamystats not ready."
  exit 1
fi

echo "Integration stack is up."

echo "Stopping integration stack..."
if command -v docker-compose >/dev/null 2>&1; then
  docker-compose -f "$compose_file" down
else
  docker compose -f "$compose_file" down
fi

popd >/dev/null
