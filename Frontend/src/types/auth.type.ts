export const LANGUAGE_DISPLAY = ["vi", "en"] as const;
export type LanguageDisplay = (typeof LANGUAGE_DISPLAY)[number];

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
  id: string;
  avatarUrl: string;
  name: string;
  email: string;
  language: LanguageDisplay;
}

export interface SessionResult {
  deviceId: string;
  deviceInfo: string;
  createdAt: string;
  expiresAt: string;
  isCurrent: boolean;
}

export interface UpdateProfileRequest {
  name?: string;
  fileId?: string;
  language?: LanguageDisplay;
}

export interface UploadPictureResult {
  fileId: string;
  url: string;
}
