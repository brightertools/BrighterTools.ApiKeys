import { FormEvent, useCallback, useEffect, useMemo, useState } from "react";
import {
  defaultApiKeySectionText,
  defaultSharedApiKeysText,
  formatApiKeysText,
  type ApiKeysUiTextOverrides
} from "../apiKeysUi";
import type { ApiKeyPlainResult, ApiKeyRecord, ApiKeysApiClient, ApiKeyType } from "../types/apiKeys";

export interface ApiKeyManagementSectionProps {
  apiClient: ApiKeysApiClient;
  keyType: ApiKeyType;
  includeSecret?: boolean;
  username?: string;
  usernameHeaderName?: string;
  apiKeyHeaderName?: string;
  title?: string;
  description?: string;
  createButtonText?: string;
  pageSize?: number;
  rotationGraceDays?: number;
  textOverrides?: ApiKeysUiTextOverrides;
  formatDate?: (value?: string | null) => string;
}

type AlertTone = "success" | "warning" | "danger";

interface AlertState {
  tone?: AlertTone;
  messages: string[];
}

export function ApiKeyManagementSection({
  apiClient,
  keyType,
  includeSecret = false,
  username,
  usernameHeaderName,
  apiKeyHeaderName,
  title,
  description,
  createButtonText,
  pageSize = 100,
  rotationGraceDays = 30,
  textOverrides,
  formatDate
}: ApiKeyManagementSectionProps) {
  const sectionText = useMemo(() => ({ ...defaultApiKeySectionText, ...textOverrides?.section }), [textOverrides]);
  const sharedText = useMemo(() => ({
    ...defaultSharedApiKeysText,
    ...textOverrides?.shared,
    statusLabels: {
      ...defaultSharedApiKeysText.statusLabels,
      ...textOverrides?.shared?.statusLabels
    }
  }), [textOverrides]);
  const resolvedTitle = title ?? (keyType === "server" ? sectionText.serverTitle : sectionText.clientTitle);
  const resolvedDescription = description ?? (keyType === "server" ? sectionText.serverDescription : sectionText.clientDescription);
  const resolvedCreateButtonText = createButtonText ?? (keyType === "server" ? sectionText.createServerKeyLabel : sectionText.createClientKeyLabel);
  const [keys, setKeys] = useState<ApiKeyRecord[]>([]);
  const [name, setName] = useState("");
  const [showForm, setShowForm] = useState(false);
  const [isBusy, setIsBusy] = useState(false);
  const [newKey, setNewKey] = useState<ApiKeyPlainResult | null>(null);
  const [alert, setAlert] = useState<AlertState>({ messages: [] });

  const loadKeys = useCallback(async () => {
    try {
      const response = keyType === "server"
        ? await apiClient.listServerKeys({ page: 1, pageSize })
        : await apiClient.listClientKeys({ page: 1, pageSize });
      setKeys(response.items ?? []);
    } catch (error) {
      setAlert({ tone: "danger", messages: [getErrorMessage(error, sectionText.loadFailedMessage)] });
    }
  }, [apiClient, keyType, pageSize, sectionText.loadFailedMessage]);

  useEffect(() => {
    void loadKeys();
  }, [loadKeys]);

  const createKey = async (event: FormEvent) => {
    event.preventDefault();
    const trimmedName = name.trim();

    if (!trimmedName) {
      setAlert({ tone: "warning", messages: [sectionText.nameRequiredMessage] });
      return;
    }

    setIsBusy(true);
    setAlert({ messages: [] });

    try {
      const response = keyType === "server"
        ? await apiClient.createServerKey({ name: trimmedName, withSecret: includeSecret })
        : await apiClient.createClientKey({ name: trimmedName, withSecret: false });

      setNewKey(response);
      setName("");
      setShowForm(false);
      setAlert({ tone: "success", messages: [formatApiKeysText(sectionText.createdMessage, { title: resolvedTitle })] });
      await loadKeys();
    } catch (error) {
      setAlert({
        tone: "danger",
        messages: [getErrorMessage(error, formatApiKeysText(sectionText.createFailedMessage, { title: resolvedTitle.toLowerCase() }))]
      });
    } finally {
      setIsBusy(false);
    }
  };

  const rotateKey = async (key: ApiKeyRecord) => {
    if (!key.guid) {
      setAlert({ tone: "danger", messages: [sectionText.missingIdentifierRotateMessage] });
      return;
    }

    setIsBusy(true);
    setAlert({ messages: [] });

    try {
      const response = keyType === "server"
        ? await apiClient.rotateServerKey({ guid: key.guid, graceDays: rotationGraceDays, withSecret: includeSecret })
        : await apiClient.rotateClientKey({ guid: key.guid, graceDays: rotationGraceDays });

      setNewKey(response);
      setAlert({ tone: "success", messages: [formatApiKeysText(sectionText.rotatedMessage, { name: key.name })] });
      await loadKeys();
    } catch (error) {
      setAlert({ tone: "danger", messages: [getErrorMessage(error, sectionText.rotateFailedMessage)] });
    } finally {
      setIsBusy(false);
    }
  };

  const revokeKey = async (key: ApiKeyRecord) => {
    if (!key.guid) {
      setAlert({ tone: "danger", messages: [sectionText.missingIdentifierRevokeMessage] });
      return;
    }

    setIsBusy(true);
    setAlert({ messages: [] });

    try {
      await apiClient.revokeKey({ guid: key.guid });
      setAlert({ tone: "success", messages: [formatApiKeysText(sectionText.revokedMessage, { name: key.name })] });
      await loadKeys();
    } catch (error) {
      setAlert({ tone: "danger", messages: [getErrorMessage(error, sectionText.revokeFailedMessage)] });
    } finally {
      setIsBusy(false);
    }
  };

  return (
    <section className="card mb-4">
      <div className="card-body">
        <h5 className="mb-1">{resolvedTitle}</h5>
        <p className="text-muted mb-0">{resolvedDescription}</p>

        {keyType === "server" && usernameHeaderName && apiKeyHeaderName && (
          <div className="border rounded bg-light p-3 mt-3">
            <div className="fw-semibold mb-2">{sectionText.serverInstructionsHeading}</div>
            <div className="font-monospace small">
              <div><span className="text-secondary">{usernameHeaderName}</span> = {username || sectionText.usernameFallback}</div>
              <div><span className="text-secondary">{apiKeyHeaderName}</span> = {sectionText.serverKeyInstruction}</div>
            </div>
          </div>
        )}

        <AlertMessage alert={alert} />

        {newKey && (
          <div className="alert alert-warning mt-3">
            <strong>{sectionText.copyNowTitle}</strong>
            <div>{sectionText.copyNowDescription}</div>
            <CopyableValueField caption={sectionText.apiKeyLabel} value={newKey.plainKey} textOverrides={textOverrides} />
            {newKey.plainSecret && <CopyableValueField caption={sectionText.secretLabel} value={newKey.plainSecret} textOverrides={textOverrides} />}
          </div>
        )}

        {showForm && (
          <form className="mt-3" onSubmit={createKey}>
            <label className="form-label" htmlFor={`${keyType}-api-key-name`}>{sectionText.keyNameLabel}</label>
            <div className="input-group">
              <input id={`${keyType}-api-key-name`} className="form-control" value={name} onChange={event => setName(event.target.value)} maxLength={256} />
              <button type="submit" className="btn btn-primary" disabled={isBusy}>
                {isBusy ? sectionText.creatingLabel : sectionText.createLabel}
              </button>
              <button type="button" className="btn btn-outline-secondary" onClick={() => setShowForm(false)} disabled={isBusy}>
                {sectionText.cancelLabel}
              </button>
            </div>
          </form>
        )}

        <div className="list-group mt-3">
          {keys.length === 0 && <div className="list-group-item text-muted">{sectionText.noKeysMessage}</div>}
          {keys.map(key => (
            <div key={key.guid} className="list-group-item">
              <div className="d-flex flex-column flex-lg-row justify-content-between gap-3">
                <div>
                  <div className="d-flex align-items-center gap-2 flex-wrap">
                    <strong>{key.name}</strong>
                    <span className={getStatusClass(key.status)}>{getStatusText(key.status, sharedText.statusLabels)}</span>
                    {key.importedFromLegacy && <span className="badge bg-info">{sharedText.legacyBadgeLabel}</span>}
                  </div>
                  <small className="text-muted d-block font-monospace">{key.keyPreview}</small>
                  <small className="text-muted d-block">
                    {formatApiKeysText(sectionText.lastUsedLabel, { date: toLocalDateText(key.lastUsedDate, sharedText.neverLabel, formatDate) })}
                  </small>
                </div>
                <div className="d-flex gap-2 align-items-start">
                  <button type="button" className="btn btn-outline-secondary btn-sm" onClick={() => void rotateKey(key)} disabled={isBusy || key.status === 2}>
                    {sectionText.rotateLabel}
                  </button>
                  <button type="button" className="btn btn-outline-danger btn-sm" onClick={() => void revokeKey(key)} disabled={isBusy || key.status === 2}>
                    {sectionText.revokeLabel}
                  </button>
                </div>
              </div>
            </div>
          ))}
        </div>

        {!showForm && (
          <div className="mt-3">
            <button type="button" className="btn btn-primary" onClick={() => setShowForm(true)}>
              {resolvedCreateButtonText}
            </button>
          </div>
        )}
      </div>
    </section>
  );
}

interface CopyableValueFieldProps {
  caption: string;
  value: string;
  textOverrides?: ApiKeysUiTextOverrides;
}

function CopyableValueField({ caption, value, textOverrides }: CopyableValueFieldProps) {
  const sectionText = useMemo(() => ({ ...defaultApiKeySectionText, ...textOverrides?.section }), [textOverrides]);
  const [copied, setCopied] = useState(false);

  const copy = async () => {
    await navigator.clipboard.writeText(value);
    setCopied(true);
    window.setTimeout(() => setCopied(false), 1500);
  };

  return (
    <div className="mt-2">
      <label className="form-label mb-1">{caption}</label>
      <div className="input-group">
        <input type="text" className="form-control font-monospace" value={value} readOnly />
        <button type="button" className="btn btn-outline-secondary" onClick={() => void copy()}>
          {copied ? sectionText.copiedLabel : sectionText.copyLabel}
        </button>
      </div>
    </div>
  );
}

function AlertMessage({ alert }: { alert: AlertState }) {
  if (alert.messages.length === 0) {
    return null;
  }

  return (
    <div className={`alert alert-${alert.tone ?? "info"} mt-3`} role="alert">
      {alert.messages.map((message, index) => <div key={`${message}-${index}`}>{message}</div>)}
    </div>
  );
}

function getStatusClass(status: number) {
  if (status === 0) return "badge bg-success";
  if (status === 1) return "badge bg-warning text-dark";
  if (status === 2) return "badge bg-danger";
  return "badge bg-secondary";
}

function getStatusText(status: number, labels?: Record<string, string>) {
  return labels?.[String(status)] ?? labels?.unknown ?? String(status);
}

function toLocalDateText(value: string | null | undefined, neverLabel: string, formatDate?: (value?: string | null) => string) {
  if (formatDate) {
    return formatDate(value);
  }

  if (!value) return neverLabel;
  const date = new Date(value);
  return Number.isNaN(date.getTime()) ? neverLabel : date.toLocaleString();
}

function getErrorMessage(error: unknown, fallback: string) {
  return error instanceof Error && error.message.trim().length > 0 ? error.message : fallback;
}

