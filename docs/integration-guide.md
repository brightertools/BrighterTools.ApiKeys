# Integration Guide

`BrighterTools.ApiKeys` is split into:

- `BrighterTools.ApiKeys`
  - core models, options, store abstractions, and orchestration services
- `BrighterTools.ApiKeys.AspNetCore`
  - ASP.NET Core authentication handlers and scheme registration
- `brightertools-api-keys-react`
  - React management components, API adapter contracts, and localization manifests/defaults

The host application should provide:

- `IApiKeyStore` for persistence
- `IApiKeyOwnerContext` for key ownership when creating or rotating keys
- `IApiKeyPrincipalFactory` for translating authenticated keys into claims principals
- `IApiKeyRequestContextApplier` for request-scoped tenant or owner binding
- any custom usage tracking via `IApiKeyUsageTracker`

Registration outline:

```csharp
services.AddScoped<IApiKeyStore, MyApiKeyStore>();
services.AddScoped<IApiKeyOwnerContext, MyApiKeyOwnerContext>();
services.AddScoped<IApiKeyPrincipalFactory, MyPrincipalFactory>();
services.AddScoped<IApiKeyRequestContextApplier, MyRequestContextApplier>();

services.AddAuthentication()
    .AddBrighterToolsApiKeyAuthentication(configuration);
```

## React companion

The React package is adapter-first. The host application supplies an `ApiKeysApiClient` that unwraps its own HTTP response envelope and returns package DTOs directly.

```tsx
import {
  ApiKeysManager,
  createApiKeysLocalizationManifest,
  createLocalizedApiKeysUiText
} from "brightertools-api-keys-react";

const textOverrides = createLocalizedApiKeysUiText(t);

<ApiKeysManager
  apiClient={apiKeysClient}
  enabled={user.adfastApiEnabled}
  clientKeysEnabled={configuration.apiKeys.clientKeysEnabled}
  username={user.username || user.email}
  usernameHeaderName={configuration.apiKeys.usernameHeaderName}
  apiKeyHeaderName={configuration.apiKeys.apiKeyHeaderName}
  textOverrides={textOverrides}
/>;
```

For localization discovery, seed `createApiKeysLocalizationManifest("common")` or the namespace used by the host app. Runtime translation remains host-owned.
