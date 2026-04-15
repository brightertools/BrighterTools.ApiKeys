# BrighterTools.ApiKeys

Core API key abstractions and orchestration services.

The package provides:
- API key models and options
- key generation, hashing, authentication, and rotation services
- store and owner-context abstractions
- dependency injection registration for the core service layer

The host application must provide:
- an `IApiKeyStore`
- an `IApiKeyOwnerContext`
- any custom usage tracking
- application-specific authorization decisions

Minimal registration:

```csharp
services.AddScoped<IApiKeyStore, MyApiKeyStore>();
services.AddScoped<IApiKeyOwnerContext, MyApiKeyOwnerContext>();
services.AddBrighterToolsApiKeys(configuration);
```
