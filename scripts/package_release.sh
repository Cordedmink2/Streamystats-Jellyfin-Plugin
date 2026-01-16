#!/usr/bin/env bash
set -euo pipefail

VERSION="${1:-}"
if [[ -z "$VERSION" ]]; then
  echo "Usage: package_release.sh <version>"
  exit 1
fi

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
ARTIFACT_DIR="$ROOT_DIR/artifacts/Streamystats"
ZIP_NAME="Streamystats-${VERSION}.zip"

rm -rf "$ARTIFACT_DIR"
mkdir -p "$ARTIFACT_DIR"

cp "$ROOT_DIR/Jellyfin.Plugin.Streamystats/bin/Release/net9.0/Jellyfin.Plugin.Streamystats.dll" "$ARTIFACT_DIR/"
if [[ -f "$ROOT_DIR/Jellyfin.Plugin.Streamystats/bin/Release/net9.0/Jellyfin.Plugin.Streamystats.pdb" ]]; then
  cp "$ROOT_DIR/Jellyfin.Plugin.Streamystats/bin/Release/net9.0/Jellyfin.Plugin.Streamystats.pdb" "$ARTIFACT_DIR/"
fi

(
  cd "$ROOT_DIR/artifacts"
  rm -f "$ZIP_NAME"
  zip -r "$ZIP_NAME" Streamystats
)

echo "Created artifacts/$ZIP_NAME"
