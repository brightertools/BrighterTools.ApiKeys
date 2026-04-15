# Integration Guide

`BrighterTools.ApiKeys` is split into:

- `BrighterTools.ApiKeys`
  - core models, options, store abstractions, and orchestration services
- `BrighterTools.ApiKeys.AspNetCore`
  - ASP.NET Core authentication handlers and scheme registration

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
