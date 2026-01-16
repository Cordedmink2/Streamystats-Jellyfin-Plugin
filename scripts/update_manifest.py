import hashlib
import json
import os
import sys

MANIFEST_PATH = "manifest.json"
TARGET_ABI = "10.11.5.0"
OWNER = "Cordedmink2"
REPO = "Streamystats-Jellyfin-Plugin"


def sha256sum(path: str) -> str:
    sha = hashlib.sha256()
    with open(path, "rb") as f:
        for chunk in iter(lambda: f.read(8192), b""):
            sha.update(chunk)
    return sha.hexdigest()


def main() -> None:
    if len(sys.argv) < 3:
        print("Usage: update_manifest.py <version> <timestamp>")
        sys.exit(1)

    version = sys.argv[1]
    timestamp = sys.argv[2]
    zip_name = f"Streamystats-{version}.zip"
    artifact_path = os.path.join("artifacts", zip_name)

    if not os.path.exists(MANIFEST_PATH):
        raise FileNotFoundError(MANIFEST_PATH)

    if not os.path.exists(artifact_path):
        raise FileNotFoundError(artifact_path)

    with open(MANIFEST_PATH, "r", encoding="utf-8") as f:
        manifest = json.load(f)

    checksum = sha256sum(artifact_path)
    source_url = f"https://github.com/{OWNER}/{REPO}/releases/download/{version}/{zip_name}"

    if manifest.get("version") != 1 or "plugins" not in manifest:
        raise ValueError("manifest.json must be the Jellyfin plugin repository format")

    plugins = manifest.get("plugins", [])
    if not plugins:
        raise ValueError("manifest.json must include at least one plugin")

    plugin = plugins[0]
    versions = plugin.get("versions", [])
    versions.insert(
        0,
        {
            "checksum": checksum,
            "changelog": f"Release {version}",
            "targetAbi": TARGET_ABI,
            "sourceUrl": source_url,
            "timestamp": timestamp,
            "version": version,
        },
    )

    plugin["versions"] = versions
    manifest["plugins"] = plugins

    with open(MANIFEST_PATH, "w", encoding="utf-8") as f:
        json.dump(manifest, f, indent=2)
        f.write("\n")


if __name__ == "__main__":
    main()
