/**
 * Parsed user agent info containing browser and OS names.
 */
export interface ParsedUserAgent {
  browser: string;
  os: string;
}

/**
 * Map of OS names used by detectOS().
 * Useful for UI components to render OS-specific icons.
 */
export const OS_MAP = {
  WINDOWS: "Windows",
  MACOS: "macOS",
  IOS: "iOS",
  ANDROID: "Android",
  LINUX: "Linux",
  CHROME_OS: "Chrome OS",
  XBOX: "Xbox",
  PLAYSTATION: "PlayStation",
  NINTENDO: "Nintendo",
  UNKNOWN: "Unknown",
} as const;

/**
 * Extracts browser and OS names from a raw User-Agent string.
 *
 * Detection is based on common User-Agent patterns and is intentionally
 * simple — no external library (UAParser, etc.) is required.
 *
 * @param userAgent - The raw User-Agent header value.
 * @returns Parsed browser + OS, or null if the string is empty/undefined.
 *
 * @example
 * parseUserAgent(
 *   "Mozilla/5.0 (Linux; Android 10; K) AppleWebKit/537.36 ... Mobile Safari/537.36 EdgA/150.0.0.0"
 * )
 * // => { browser: "Edge", os: "Android" }
 */
export function parseUserAgent(userAgent: string): ParsedUserAgent | null {
  if (!userAgent) return null;

  return {
    browser: detectBrowser(userAgent),
    os: detectOS(userAgent),
  };
}

// ─── Browser detection ────────────────────────────────────────────

function detectBrowser(ua: string): string {
  // Order matters: check more specific patterns first

  if (/EdgA|EdgiOS|Edg\//i.test(ua)) return "Edge";
  if (/OPR|Opera/i.test(ua)) return "Opera";
  if (/CriOS|Chrome/i.test(ua)) return "Chrome";
  if (/FxiOS|Firefox/i.test(ua)) return "Firefox";
  if (/Vivaldi/i.test(ua)) return "Vivaldi";
  if (/SamsungBrowser/i.test(ua)) return "Samsung Internet";
  if (/UCBrowser/i.test(ua)) return "UC Browser";
  // Safari must be checked after Chrome (Safari shares some patterns)
  if (/Version\/[\d.]+.*Safari/i.test(ua)) return "Safari";
  if (/MSIE|Trident/i.test(ua)) return "Internet Explorer";

  return "Unknown";
}

// ─── OS detection ─────────────────────────────────────────────────

function detectOS(ua: string): string {
  if (/Windows\s*(NT|Phone)/i.test(ua)) return "Windows";
  if (/Mac OS X|Macintosh/i.test(ua)) {
    // iOS check must come before macOS (iPad / iPhone contain "Mac OS X")
    if (/iPad|iPhone|iPod/i.test(ua)) return "iOS";
    return "macOS";
  }
  if (/Android/i.test(ua)) return "Android";
  if (/Linux/i.test(ua)) return "Linux";
  if (/CrOS|Chromium OS/i.test(ua)) return "Chrome OS";
  if (/Xbox/i.test(ua)) return "Xbox";
  if (/PlayStation/i.test(ua)) return "PlayStation";
  if (/Nintendo/i.test(ua)) return "Nintendo";

  return "Unknown";
}
