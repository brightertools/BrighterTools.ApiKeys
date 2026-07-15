import type {
  ApiKeyCreateRequest,
  ApiKeyListRequest,
  ApiKeyListResult,
  ApiKeyPlainResult,
  ApiKeyRecord,
  ApiKeyRevokeRequest,
  ApiKeyRotateRequest,
  ApiKeysApiClient
} from "../types/apiKeys";

export interface CreateApiKeysApiOptions {
  baseUrl: string;
  getHeaders?: () => HeadersInit | Promise<HeadersInit>;
  fetchImpl?: typeof fetch;
}

export function createApiKeysApi(options: CreateApiKeysApiOptions): ApiKeysApiClient {
  const fetcher = options.fetchImpl ?? fetch;
  const baseUrl = options.baseUrl.replace(/\/$/, "");

  async function request<T>(path: string, init?: RequestInit): Promise<T> {
    const headers = new Headers(init?.headers);
    headers.set("Accept", "application/json");

    if (init?.body && !headers.has("Content-Type")) {
      headers.set("Content-Type", "application/json");
    }

    const configuredHeaders = await options.getHeaders?.();
    if (configuredHeaders) {
      new Headers(configuredHeaders).forEach((value, key) => headers.set(key, value));
    }

    const response = await fetcher(`${baseUrl}${path}`, { ...init, headers });
    if (!response.ok) {
      throw new Error(`API keys request failed with ${response.status}`);
    }

    return response.json() as Promise<T>;
  }

  const post = <TRequest, TResponse>(path: string, body: TRequest) => request<TResponse>(path, {
    method: "POST",
    body: JSON.stringify(body)
  });

  const put = <TRequest, TResponse>(path: string, body: TRequest) => request<TResponse>(path, {
    method: "PUT",
    body: JSON.stringify(body)
  });

  return {
    listClientKeys: requestBody => post<ApiKeyListRequest | undefined, ApiKeyListResult>("/apikeys/listClientApiKeys", requestBody ?? {}),
    listServerKeys: requestBody => post<ApiKeyListRequest | undefined, ApiKeyListResult>("/apikeys/listServerApiKeys", requestBody ?? {}),
    createClientKey: requestBody => post<ApiKeyCreateRequest, ApiKeyPlainResult>("/apikeys/createClientApiKey", requestBody),
    createServerKey: requestBody => post<ApiKeyCreateRequest, ApiKeyPlainResult>("/apikeys/createServerApiKey", requestBody),
    rotateClientKey: requestBody => put<ApiKeyRotateRequest, ApiKeyPlainResult>("/apikeys/rotateClientApiKey", requestBody),
    rotateServerKey: requestBody => put<ApiKeyRotateRequest, ApiKeyPlainResult>("/apikeys/rotateServerApiKey", requestBody),
    revokeKey: requestBody => put<ApiKeyRevokeRequest, ApiKeyRecord>("/apikeys/revokeApiKey", requestBody)
  };
}
