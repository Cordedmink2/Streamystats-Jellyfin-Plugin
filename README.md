# Streamystats Jellyfin Plugin

Streamystats integration for Jellyfin Web UI. Adds Streamystats-powered recommendations and promoted watchlists to the Jellyfin home screen using Jellyfin-authenticated Streamystats API calls.

## Features

- Movie and series recommendations powered by Streamystats
- Promoted watchlists from Streamystats
- Server-side proxy that authenticates with Jellyfin tokens (no API keys required)
- Admin configuration page in Jellyfin dashboard

## Requirements

- Jellyfin Server 10.11.5
- Streamystats instance with external API enabled

## Install

1. Build the plugin:
   ```bash
   dotnet build Jellyfin.Plugin.Streamystats.sln
   ```
2. Copy the plugin output to your Jellyfin plugins directory:
   ```bash
   cp -r Jellyfin.Plugin.Streamystats/bin/Debug/net9.0/ /path/to/jellyfin/plugins/Streamystats/
   ```
3. Restart Jellyfin.

## Configure

1. Open Jellyfin Dashboard → Plugins → Streamystats.
2. Set **Streamystats server URL** (e.g. `https://stats.example.com`).
3. Toggle movie/series recommendations and promoted watchlists.
4. Save and reload the Jellyfin home page.

## How it works

- The plugin uses the Jellyfin user token to call Streamystats endpoints:
  - `/api/recommendations` (IDs format)
  - `/api/watchlists/promoted`
- IDs returned from Streamystats are resolved back to Jellyfin items and rendered as cards on the home screen.
- Promoted watchlists show as lightweight cards that link back to Streamystats.

## Release & manifest

- Plugin archive name: `Streamystats-<version>.zip`
- Manifest file: `manifest.json`
- Release workflow updates the manifest with checksum + source URL

## Development

Run tests:
```bash
dotnet test Jellyfin.Plugin.Streamystats.sln
```

Run integration stack (requires Docker):
```bash
scripts/run_integration.sh
```

## Troubleshooting

### No recommendations show up
- Verify Streamystats URL is reachable from your Jellyfin server.
- Ensure Streamystats has completed sync + embeddings/recommendations.
- Check Streamystats settings for the server ID (Jellyfin’s public server ID).

### Promoted watchlists empty
- In Streamystats, mark at least one watchlist as promoted for the server.

### Errors in logs
- Jellyfin logs will show warnings if:
  - Streamystats URL is invalid
  - Jellyfin public server id is missing
  - Streamystats API returns non-200 responses
