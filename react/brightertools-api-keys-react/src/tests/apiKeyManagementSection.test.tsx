import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { ApiKeyManagementSection } from "../components/ApiKeyManagementSection";
import type { ApiKeysApiClient } from "../types/apiKeys";

describe("ApiKeyManagementSection", () => {
  it("loads keys and renders localized status labels", async () => {
    const apiClient = createApiClient();

    render(
      <ApiKeyManagementSection
        apiClient={apiClient}
        keyType="server"
        textOverrides={{ shared: { statusLabels: { "0": "Ready" } } }}
        formatDate={() => "Never"}
      />
    );

    expect(await screen.findByText("Primary")).toBeTruthy();
    expect(screen.getByText("Ready")).toBeTruthy();
    expect(apiClient.listServerKeys).toHaveBeenCalledWith({ page: 1, pageSize: 100 });
  });

  it("validates key names before creating", async () => {
    const apiClient = createApiClient();

    render(<ApiKeyManagementSection apiClient={apiClient} keyType="client" />);

    fireEvent.click(await screen.findByText("Create Client Key"));
    fireEvent.click(screen.getByText("Create"));

    expect(await screen.findByText("Please enter a key name.")).toBeTruthy();
    expect(apiClient.createClientKey).not.toHaveBeenCalled();
  });

  it("creates, rotates, and revokes through the adapter", async () => {
    const apiClient = createApiClient();

    render(<ApiKeyManagementSection apiClient={apiClient} keyType="server" formatDate={() => "Never"} />);

    fireEvent.click(await screen.findByText("Create Server Key"));
    fireEvent.change(screen.getByLabelText("Key name"), { target: { value: "Second" } });
    fireEvent.click(screen.getByText("Create"));

    expect(await screen.findByText("Server API Keys created.")).toBeTruthy();
    expect(apiClient.createServerKey).toHaveBeenCalledWith({ name: "Second", withSecret: false });
    expect(screen.getByDisplayValue("plain-key")).toBeTruthy();

    fireEvent.click(screen.getByText("Rotate"));
    expect(await screen.findByText("Primary rotated. Copy the new key now.")).toBeTruthy();
    expect(apiClient.rotateServerKey).toHaveBeenCalledWith({ guid: "key-1", graceDays: 30, withSecret: false });

    fireEvent.click(screen.getByText("Revoke"));
    expect(await screen.findByText("Primary revoked.")).toBeTruthy();
    expect(apiClient.revokeKey).toHaveBeenCalledWith({ guid: "key-1" });
  });
});

function createApiClient(): ApiKeysApiClient {
  return {
    listClientKeys: vi.fn(async () => ({ totalCount: 0, items: [] })),
    listServerKeys: vi.fn(async () => ({
      totalCount: 1,
      items: [{
        id: 1,
        guid: "key-1",
        name: "Primary",
        keyPreview: "bt_live_1234",
        securityLevel: 0,
        status: 0,
        lastUsedDate: null
      }]
    })),
    createClientKey: vi.fn(async request => ({
      id: 2,
      guid: "key-2",
      name: request.name,
      plainKey: "plain-key",
      keyPreview: "bt_live_5678",
      securityLevel: 0,
      status: 0
    })),
    createServerKey: vi.fn(async request => ({
      id: 2,
      guid: "key-2",
      name: request.name,
      plainKey: "plain-key",
      keyPreview: "bt_live_5678",
      securityLevel: 0,
      status: 0
    })),
    rotateClientKey: vi.fn(async () => ({
      id: 3,
      guid: "key-3",
      name: "Primary",
      plainKey: "rotated-key",
      keyPreview: "bt_live_9999",
      securityLevel: 0,
      status: 0
    })),
    rotateServerKey: vi.fn(async () => ({
      id: 3,
      guid: "key-3",
      name: "Primary",
      plainKey: "rotated-key",
      keyPreview: "bt_live_9999",
      securityLevel: 0,
      status: 0
    })),
    revokeKey: vi.fn(async () => ({
      id: 1,
      guid: "key-1",
      name: "Primary",
      keyPreview: "bt_live_1234",
      securityLevel: 0,
      status: 2
    }))
  };
}
