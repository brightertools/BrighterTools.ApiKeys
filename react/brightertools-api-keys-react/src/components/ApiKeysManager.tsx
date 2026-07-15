import { useMemo } from "react";
import {
  defaultApiKeysManagerText,
  type ApiKeysUiTextOverrides
} from "../apiKeysUi";
import type { ApiKeysApiClient } from "../types/apiKeys";
import { ApiKeyManagementSection } from "./ApiKeyManagementSection";

export interface ApiKeysManagerProps {
  apiClient: ApiKeysApiClient;
  enabled?: boolean;
  clientKeysEnabled?: boolean;
  includeServerSecret?: boolean;
  username?: string;
  usernameHeaderName?: string;
  apiKeyHeaderName?: string;
  textOverrides?: ApiKeysUiTextOverrides;
}

export function ApiKeysManager({
  apiClient,
  enabled = true,
  clientKeysEnabled = false,
  includeServerSecret = false,
  username,
  usernameHeaderName,
  apiKeyHeaderName,
  textOverrides
}: ApiKeysManagerProps) {
  const managerText = useMemo(() => ({ ...defaultApiKeysManagerText, ...textOverrides?.manager }), [textOverrides]);

  if (!enabled) {
    return (
      <div className="card">
        <div className="card-body">
          <h5 className="mb-2">{managerText.disabledTitle}</h5>
          <p className="mb-0 text-muted">{managerText.disabledMessage}</p>
        </div>
      </div>
    );
  }

  return (
    <>
      <div className="card mb-4">
        <div className="card-body">
          <h5 className="mb-2">{managerText.integrationTitle}</h5>
          <p className="mb-0 text-muted">{managerText.integrationDescription}</p>
        </div>
      </div>

      <ApiKeyManagementSection
        apiClient={apiClient}
        keyType="server"
        includeSecret={includeServerSecret}
        username={username}
        usernameHeaderName={usernameHeaderName}
        apiKeyHeaderName={apiKeyHeaderName}
        textOverrides={textOverrides}
      />

      {clientKeysEnabled && (
        <ApiKeyManagementSection
          apiClient={apiClient}
          keyType="client"
          includeSecret={false}
          textOverrides={textOverrides}
        />
      )}
    </>
  );
}
