import { describe, expect, it } from "vitest";
import {
  apiKeysLocalizationManifest,
  createApiKeysLocalizationManifest,
  createLocalizedApiKeysUiText,
  mergeApiKeysUiTextOverrides
} from "../apiKeysUi";

describe("createLocalizedApiKeysUiText", () => {
  it("uses apiKeys.* keys and English fallbacks", () => {
    const calls: Array<{ key: string; fallback?: string; }> = [];
    const translate = (key: string, fallback?: string) => {
      calls.push({ key, fallback });
      return `translated:${key}`;
    };

    const result = createLocalizedApiKeysUiText(translate);

    expect(result.manager?.integrationTitle).toBe("translated:apiKeys.manager.integrationTitle");
    expect(result.section?.createServerKeyLabel).toBe("translated:apiKeys.section.createServerKeyLabel");
    expect(result.shared?.statusLabels?.["0"]).toBe("translated:apiKeys.shared.statusLabels.0");
    expect(calls).toContainEqual({ key: "apiKeys.section.copyNowTitle", fallback: "Copy this key now." });
    expect(calls).toContainEqual({ key: "apiKeys.shared.statusLabels.2", fallback: "Revoked" });
  });

  it("applies explicit overrides after localization", () => {
    const result = createLocalizedApiKeysUiText(key => `translated:${key}`, {
      section: {
        createLabel: "Make"
      },
      shared: {
        statusLabels: {
          "1": "Sunsetting"
        }
      }
    });

    expect(result.section?.createLabel).toBe("Make");
    expect(result.section?.cancelLabel).toBe("translated:apiKeys.section.cancelLabel");
    expect(result.shared?.statusLabels?.["0"]).toBe("translated:apiKeys.shared.statusLabels.0");
    expect(result.shared?.statusLabels?.["1"]).toBe("Sunsetting");
  });
});

describe("createApiKeysLocalizationManifest", () => {
  it("exports flattened apiKeys defaults", () => {
    expect(apiKeysLocalizationManifest).toContainEqual({
      key: "apiKeys.section.copyLabel",
      defaultValue: "Copy"
    });
    expect(apiKeysLocalizationManifest).toContainEqual({
      key: "apiKeys.shared.statusLabels.0",
      defaultValue: "Active"
    });
  });

  it("supports host namespace prefixes", () => {
    const manifest = createApiKeysLocalizationManifest("common");

    expect(manifest).toContainEqual({
      key: "common.apiKeys.manager.integrationTitle",
      defaultValue: "API Integration"
    });
  });
});

describe("mergeApiKeysUiTextOverrides", () => {
  it("preserves nested status labels while applying overrides", () => {
    const result = mergeApiKeysUiTextOverrides(
      {
        shared: {
          statusLabels: {
            "0": "Active",
            "1": "Retiring"
          }
        }
      },
      {
        shared: {
          statusLabels: {
            "1": "Sunsetting"
          }
        }
      }
    );

    expect(result.shared?.statusLabels?.["0"]).toBe("Active");
    expect(result.shared?.statusLabels?.["1"]).toBe("Sunsetting");
  });
});
