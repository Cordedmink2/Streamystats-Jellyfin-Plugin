# Repository Guidelines

## Project Structure & Module Organization
- `Jellyfin.Plugin.Streamystats/` contains the plugin source (API, services, helpers, models, configuration UI assets).
- `Jellyfin.Plugin.Streamystats.Tests/` holds xUnit tests; test files follow `*Tests.cs` naming.
- `scripts/` provides release and integration helpers.
- `manifest.json` is the Jellyfin plugin repository manifest used for releases.
- `docker-compose.integration.yml` defines the local integration stack.

## Build, Test, and Development Commands
- `dotnet build Jellyfin.Plugin.Streamystats.sln` builds the plugin and tests.
- `dotnet test Jellyfin.Plugin.Streamystats.sln` runs all unit tests.
- `scripts/run_integration.sh` brings up Jellyfin + Streamystats via Docker, checks readiness, then tears down.
- `scripts/package_release.sh <version>` packages the release zip from the Release build output.
- `python3 scripts/update_manifest.py <version> <timestamp>` updates `manifest.json` with checksum and source URL.

## Coding Style & Naming Conventions
- Indentation: 4 spaces for C#; 2 spaces for `*.csproj`, `*.xml`, and YAML (see `.editorconfig`).
- C# conventions use `var` where appropriate and prefer braces; analyzers run with `TreatWarningsAsErrors=true`.
- Naming: `PascalCase` for public members and constants; instance fields are `_{camelCase}`; static fields are `_{camelCase}` (see `.editorconfig`).
- StyleCop + .NET analyzers are enabled via `jellyfin.ruleset`.

## Testing Guidelines
- Frameworks: xUnit with `Microsoft.NET.Test.Sdk` and NSubstitute for mocking.
- Test naming: `*Tests.cs` per class or feature (e.g., `StreamystatsUrlValidatorTests.cs`).
- Run tests via `dotnet test Jellyfin.Plugin.Streamystats.sln`; no explicit coverage threshold is enforced.

## Commit & Pull Request Guidelines
- Commit history shows no strict convention; keep messages short and imperative (e.g., "Add streamystats url validation").
- Release/version bumps often use the version as the subject; follow that pattern for release commits.
- PRs should include a clear description, testing notes (commands run), and screenshots if UI assets change.

## Configuration & Release Notes
- The plugin embeds UI assets from `Jellyfin.Plugin.Streamystats/Configuration/`.
- Ensure the Streamystats server URL is configurable and tested against Jellyfin 10.11.5.
- Release artifacts are `Streamystats-<version>.zip` and `manifest.json`.
