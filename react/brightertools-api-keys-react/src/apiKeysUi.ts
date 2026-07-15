export interface LocalizationManifestEntry {
  key: string;
  defaultValue: string;
}

export type ApiKeysUiTranslate = (key: string, fallback?: string) => string;

export interface SharedApiKeysTextOverrides {
  statusLabels?: Record<string, string>;
  legacyBadgeLabel?: string;
  neverLabel?: string;
}

export interface ApiKeysManagerTextOverrides {
  disabledTitle?: string;
  disabledMessage?: string;
  integrationTitle?: string;
  integrationDescription?: string;
}

export interface ApiKeySectionTextOverrides {
  serverTitle?: string;
  serverDescription?: string;
  clientTitle?: string;
  clientDescription?: string;
  createServerKeyLabel?: string;
  createClientKeyLabel?: string;
  serverInstructionsHeading?: string;
  usernameFallback?: string;
  serverKeyInstruction?: string;
  loadFailedMessage?: string;
  nameRequiredMessage?: string;
  createdMessage?: string;
  createFailedMessage?: string;
  missingIdentifierRotateMessage?: string;
  rotatedMessage?: string;
  rotateFailedMessage?: string;
  missingIdentifierRevokeMessage?: string;
  revokedMessage?: string;
  revokeFailedMessage?: string;
  copyNowTitle?: string;
  copyNowDescription?: string;
  apiKeyLabel?: string;
  secretLabel?: string;
  keyNameLabel?: string;
  createLabel?: string;
  creatingLabel?: string;
  cancelLabel?: string;
  noKeysMessage?: string;
  lastUsedLabel?: string;
  rotateLabel?: string;
  revokeLabel?: string;
  copyLabel?: string;
  copiedLabel?: string;
}

export interface ApiKeysUiTextOverrides {
  shared?: SharedApiKeysTextOverrides;
  manager?: ApiKeysManagerTextOverrides;
  section?: ApiKeySectionTextOverrides;
}

export const defaultStatusLabels: Record<string, string> = {
  "0": "Active",
  "1": "Retiring",
  "2": "Revoked",
  unknown: "Unknown"
};

export const defaultSharedApiKeysText: Required<SharedApiKeysTextOverrides> = {
  statusLabels: defaultStatusLabels,
  legacyBadgeLabel: "Legacy",
  neverLabel: "Never"
};

export const defaultApiKeysManagerText: Required<ApiKeysManagerTextOverrides> = {
  disabledTitle: "Integration",
  disabledMessage: "API access is not enabled for this user.",
  integrationTitle: "API Integration",
  integrationDescription: "Manage keys for public API integrations."
};

export const defaultApiKeySectionText: Required<ApiKeySectionTextOverrides> = {
  serverTitle: "Server API Keys",
  serverDescription: "Manage server API keys.",
  clientTitle: "Client API Keys",
  clientDescription: "Client keys are available for browser-safe integration flows where required.",
  createServerKeyLabel: "Create Server Key",
  createClientKeyLabel: "Create Client Key",
  serverInstructionsHeading: "Send API keys in HTTPS headers:",
  usernameFallback: "(your username)",
  serverKeyInstruction: "A server API key created below",
  loadFailedMessage: "Error loading API keys.",
  nameRequiredMessage: "Please enter a key name.",
  createdMessage: "{title} created.",
  createFailedMessage: "Error creating {title}.",
  missingIdentifierRotateMessage: "Unable to rotate key: key identifier is missing.",
  rotatedMessage: "{name} rotated. Copy the new key now.",
  rotateFailedMessage: "Error rotating API key.",
  missingIdentifierRevokeMessage: "Unable to revoke key: key identifier is missing.",
  revokedMessage: "{name} revoked.",
  revokeFailedMessage: "Error revoking API key.",
  copyNowTitle: "Copy this key now.",
  copyNowDescription: "It will not be shown again.",
  apiKeyLabel: "API Key",
  secretLabel: "Secret",
  keyNameLabel: "Key name",
  createLabel: "Create",
  creatingLabel: "Creating...",
  cancelLabel: "Cancel",
  noKeysMessage: "No API keys have been created.",
  lastUsedLabel: "Last used: {date}",
  rotateLabel: "Rotate",
  revokeLabel: "Revoke",
  copyLabel: "Copy",
  copiedLabel: "Copied"
};

export const defaultApiKeysUiText = {
  shared: defaultSharedApiKeysText,
  manager: defaultApiKeysManagerText,
  section: defaultApiKeySectionText
};

const flattenLocalizationManifestEntries = (prefix: string, source: Record<string, unknown>): LocalizationManifestEntry[] => {
  return Object.entries(source).flatMap(([key, value]) => {
    const nextKey = `${prefix}.${key}`;

    if (typeof value === "string") {
      return [{ key: nextKey, defaultValue: value }];
    }

    return flattenLocalizationManifestEntries(nextKey, value as Record<string, unknown>);
  });
};

export function createApiKeysLocalizationManifest(namespaceName?: string): LocalizationManifestEntry[] {
  const prefix = namespaceName ? `${namespaceName}.apiKeys` : "apiKeys";
  return flattenLocalizationManifestEntries(prefix, defaultApiKeysUiText as Record<string, unknown>);
}

export const apiKeysLocalizationManifest = createApiKeysLocalizationManifest();

function localizeRecord<T extends Record<string, string>>(prefix: string, defaults: T, translate: ApiKeysUiTranslate): T {
  return Object.fromEntries(
    Object.entries(defaults).map(([key, defaultValue]) => [key, translate(`${prefix}.${key}`, defaultValue)])
  ) as T;
}

export function formatApiKeysText(template: string, values: Record<string, string | number | undefined>) {
  return template.replace(/\{(\w+)\}/g, (_, key: string) => String(values[key] ?? ""));
}

export function mergeApiKeysUiTextOverrides(base: ApiKeysUiTextOverrides, overrides?: ApiKeysUiTextOverrides): ApiKeysUiTextOverrides {
  if (!overrides) {
    return base;
  }

  return {
    ...base,
    ...overrides,
    shared: {
      ...base.shared,
      ...overrides.shared,
      statusLabels: {
        ...base.shared?.statusLabels,
        ...overrides.shared?.statusLabels
      }
    },
    manager: {
      ...base.manager,
      ...overrides.manager
    },
    section: {
      ...base.section,
      ...overrides.section
    }
  };
}

export function createLocalizedApiKeysUiText(translate: ApiKeysUiTranslate, overrides?: ApiKeysUiTextOverrides): ApiKeysUiTextOverrides {
  const localized: ApiKeysUiTextOverrides = {
    shared: {
      legacyBadgeLabel: translate("apiKeys.shared.legacyBadgeLabel", defaultApiKeysUiText.shared.legacyBadgeLabel),
      neverLabel: translate("apiKeys.shared.neverLabel", defaultApiKeysUiText.shared.neverLabel),
      statusLabels: localizeRecord("apiKeys.shared.statusLabels", defaultApiKeysUiText.shared.statusLabels, translate)
    },
    manager: localizeRecord("apiKeys.manager", defaultApiKeysUiText.manager, translate),
    section: localizeRecord("apiKeys.section", defaultApiKeysUiText.section, translate)
  };

  return mergeApiKeysUiTextOverrides(localized, overrides);
}


