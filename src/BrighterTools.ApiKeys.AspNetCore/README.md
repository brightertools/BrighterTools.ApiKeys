# BrighterTools.ApiKeys.AspNetCore

ASP.NET Core integration package for `BrighterTools.ApiKeys`.

The package adds:
- client and server API key authentication handlers
- authentication scheme registration helpers
- extension points for principal creation and request-context application

The host application must provide:
- an `IApiKeyPrincipalFactory`
- an `IApiKeyRequestContextApplier`
- store and owner-context services from `BrighterTools.ApiKeys`

Minimal registration:

```csharp
services.AddScoped<IApiKeyStore, MyApiKeyStore>();
services.AddScoped<IApiKeyOwnerContext, MyApiKeyOwnerContext>();
services.AddScoped<IApiKeyPrincipalFactory, MyApiKeyPrincipalFactory>();
services.AddScoped<IApiKeyRequestContextApplier, MyRequestContextApplier>();

services.AddAuthentication()
    .AddBrighterToolsApiKeyAuthentication(configuration);
```
