# BrighterTools.ApiKeys

`BrighterTools.ApiKeys` provides reusable API key generation, hashing, rotation, storage abstractions, and authentication orchestration for .NET applications.

`BrighterTools.ApiKeys.AspNetCore` adds ASP.NET Core authentication handlers and registration helpers. `@brightertools/api-keys-react` adds adapter-first React management UI and localization helpers.

The host application owns persistence, owner resolution, tenancy rules, claims/principal shape, authorization policies, response envelopes, and secret management.

## Packages

```powershell
dotnet add package BrighterTools.ApiKeys
dotnet add package BrighterTools.ApiKeys.AspNetCore
npm install @brightertools/api-keys-react
```

## Repository Layout

- `src/BrighterTools.ApiKeys` - core API key abstractions, models, options, and orchestration services
- `src/BrighterTools.ApiKeys.AspNetCore` - ASP.NET Core authentication handlers and registration helpers
- `tests/BrighterTools.ApiKeys.Tests` - core and ASP.NET registration tests
- `react/brightertools-api-keys-react` - React management components, API client contracts, and localization helpers
- `docs` - public integration notes

## Documentation

- [usage.md](./usage.md) for consuming application guidance
- [docs/integration-guide.md](./docs/integration-guide.md) for integration details
- [publishing.md](./publishing.md) for maintainer release steps
- [RELEASE_NOTES.md](./RELEASE_NOTES.md) for release history

## Validation

```powershell
dotnet test .\BrighterTools.ApiKeys.sln
cd .\react\brightertools-api-keys-react
npm install
npm test
npm run build
npm run pack:dry-run
```
