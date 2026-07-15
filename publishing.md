# BrighterTools.ApiKeys Publishing

This guide is for maintainers packaging and publishing `BrighterTools.ApiKeys`, `BrighterTools.ApiKeys.AspNetCore`, and `brightertools-api-keys-react`.

## Package Pages

- NuGet: https://www.nuget.org/packages/BrighterTools.ApiKeys
- NuGet: https://www.nuget.org/packages/BrighterTools.ApiKeys.AspNetCore
- npm: https://www.npmjs.com/package/brightertools-api-keys-react

## Local Packaging

```text
PackageToolForNuGet.bat
```

React package validation:

```text
cd ./react/brightertools-api-keys-react
npm install
npm test
npm run build
npm run pack:dry-run
```

Equivalent NuGet flow:

```text
dotnet restore ./BrighterTools.ApiKeys.sln --configfile ./NuGet.config
dotnet build ./BrighterTools.ApiKeys.sln -c Release --no-restore
dotnet test ./BrighterTools.ApiKeys.sln -c Release --no-build
dotnet pack ./src/BrighterTools.ApiKeys/BrighterTools.ApiKeys.csproj -c Release --no-build --output ./artifacts/nuget --configfile ./NuGet.config
dotnet pack ./src/BrighterTools.ApiKeys.AspNetCore/BrighterTools.ApiKeys.AspNetCore.csproj -c Release --no-build --output ./artifacts/nuget --configfile ./NuGet.config
```

## GitHub Actions Publishing

Publishing is handled by `.github/workflows/publish-tool.yml`.

Workflow inputs:

- `version` optionally overrides NuGet package versions.
- `publish_to_nuget` controls whether `.nupkg` files are pushed to nuget.org.
- `publish_to_npm` controls whether the React package is published to npm.

NuGet uses `NuGet/login@v1` and GitHub OIDC. npm uses trusted publishing from GitHub Actions. No long-lived NuGet or npm publish token is required after registry policies are configured.

## Registry Checklist

- NuGet package owner has Trusted Publishing policies for this repository, `publish-tool.yml`, and the `production` environment.
- npm package has a Trusted Publisher entry for this repository, `publish-tool.yml`, and the `production` environment.
- Package metadata uses the `MIT-0` license.
- Version is `1.0.0` for the first stable publish.

## Related Docs

- [README.md](./README.md) for overview and package layout
- [usage.md](./usage.md) for consuming application guidance
- [docs/integration-guide.md](./docs/integration-guide.md) for integration details
