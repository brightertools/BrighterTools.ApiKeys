# BrighterTools.ApiKeys Usage

Install the backend packages:

```powershell
dotnet add package BrighterTools.ApiKeys
dotnet add package BrighterTools.ApiKeys.AspNetCore
```

Install the React companion package:

```powershell
npm install brightertools-api-keys-react
```

`BrighterTools.ApiKeys` owns reusable key generation, hashing, rotation, revocation, and validation orchestration. The host application owns persistence, owner resolution, tenancy, authorization, claims/principal shape, and any HTTP response envelope.

The React package is adapter-first. Implement `ApiKeysApiClient` by adapting your app endpoints, then render `ApiKeysManager` or `ApiKeyManagementSection`. For localization, seed `createApiKeysLocalizationManifest("common")` into the host app localization store, translate keys through your app mechanism, and pass the translated result into the component. Explicit `textOverrides` win over translated defaults.

See [docs/integration-guide.md](./docs/integration-guide.md) for the longer integration notes.
