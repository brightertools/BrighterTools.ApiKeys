export type ApiKeyStatus = number;
export type ApiKeySecurityLevel = number;
export type ApiKeyType = "client" | "server";

export interface ApiKeyRecord {
  id?: number;
  guid: string;
  ownerType?: string;
  ownerId?: string;
  name: string;
  keyPreview: string;
  securityLevel: ApiKeySecurityLevel;
  status: ApiKeyStatus;
  expiryDate?: string | null;
  lastUsedDate?: string | null;
  rotatedFromKeyGuid?: string | null;
  importedFromLegacy?: boolean;
}

export interface ApiKeyPlainResult {
  id?: number;
  guid: string;
  ownerType?: string;
  ownerId?: string;
  name: string;
  plainKey: string;
  plainSecret?: string | null;
  keyPreview: string;
  securityLevel: ApiKeySecurityLevel;
  status: ApiKeyStatus;
}

export interface ApiKeyCreateRequest {
  name: string;
  withSecret?: boolean;
}

export interface ApiKeyListRequest {
  page?: number;
  pageSize?: number;
  search?: string;
  status?: ApiKeyStatus;
  includeExpired?: boolean;
  sortDescending?: boolean;
}

export interface ApiKeyListResult {
  totalCount: number;
  items: ApiKeyRecord[];
}

export interface ApiKeyRotateRequest {
  guid: string;
  graceDays?: number;
  withSecret?: boolean;
}

export interface ApiKeyRevokeRequest {
  guid: string;
}

export interface ApiKeysApiClient {
  listClientKeys(request?: ApiKeyListRequest): Promise<ApiKeyListResult>;
  listServerKeys(request?: ApiKeyListRequest): Promise<ApiKeyListResult>;
  createClientKey(request: ApiKeyCreateRequest): Promise<ApiKeyPlainResult>;
  createServerKey(request: ApiKeyCreateRequest): Promise<ApiKeyPlainResult>;
  rotateClientKey(request: ApiKeyRotateRequest): Promise<ApiKeyPlainResult>;
  rotateServerKey(request: ApiKeyRotateRequest): Promise<ApiKeyPlainResult>;
  revokeKey(request: ApiKeyRevokeRequest): Promise<ApiKeyRecord>;
}
