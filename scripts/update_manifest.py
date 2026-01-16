import hashlib
import json
import os
import sys

MANIFEST_PATH = "manifest.json"
TARGET_ABI = "10.11.5.0"
OWNER = "Cordedmink2"
REPO = "Streamystats-Jellyfin-Plugin"


def md5sum(path: str) -> str:
    md5 = hashlib.md5()
    with open(path, "rb") as f:
        for chunk in iter(lambda: f.read(8192), b""):
            md5.update(chunk)
    return md5.hexdigest()


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

    if not isinstance(manifest, list) or not manifest:
        raise ValueError("manifest.json must be a non-empty list of plugins")

    checksum = md5sum(artifact_path)
    source_url = f"https://github.com/{OWNER}/{REPO}/releases/download/{version}/{zip_name}"

    plugin = manifest[0]
    versions = plugin.get("versions", [])

    cleaned = []
    for v in versions:
        if not v.get("timestamp") or not v.get("sourceUrl") or not v.get("checksum"):
            continue
        cleaned.append(v)

    cleaned.insert(
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

    plugin["versions"] = cleaned
    manifest[0] = plugin

    with open(MANIFEST_PATH, "w", encoding="utf-8") as f:
        json.dump(manifest, f, indent=2)
        f.write("\n")


if __name__ == "__main__":
    main()
