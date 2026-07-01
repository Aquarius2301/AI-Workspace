export type LanguageDisplay = "vi" | "en";

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  name: string;
  email: string;
  password: string;
}

export interface AuthResponse {
  avatar: string;
  name: string;
  email: string;
  language: LanguageDisplay;
}

export interface SessionResult {
  id: string;
  deviceId: string | null;
  deviceInfo: string | null;
  browser: string | null;
  os: string | null;
  createdAt: string;
  expiresAt: string;
  isCurrent: boolean;
}
