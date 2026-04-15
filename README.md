# BrighterTools.ApiKeys

`BrighterTools.ApiKeys` provides reusable API key generation, hashing, rotation, storage abstractions, and authentication orchestration for .NET applications.

`BrighterTools.ApiKeys.AspNetCore` adds ASP.NET Core authentication handlers and registration helpers for using those API keys at the HTTP boundary.

The host application owns:
- persistence for API keys and key usage
- owner resolution and tenancy rules
- claims creation and principal shape
- request-context application and tenant binding
- authorization policies and endpoint protection
- secret management and environment-specific configuration

## Packages

```powershell
dotnet add package BrighterTools.ApiKeys
dotnet add package BrighterTools.ApiKeys.AspNetCore
```

## Repository Layout

- `src/BrighterTools.ApiKeys`
  - core API key abstractions, models, options, and orchestration services
- `src/BrighterTools.ApiKeys.AspNetCore`
  - ASP.NET Core authentication handlers and registration helpers
- `tests/BrighterTools.ApiKeys.Tests`
  - focused unit tests for core service behavior and ASP.NET scheme registration
- `docs`
  - public integration notes

## Validation

```powershell
dotnet test .\BrighterTools.ApiKeys.sln
```

## Documentation

- [`docs/integration-guide.md`](docs/integration-guide.md)
