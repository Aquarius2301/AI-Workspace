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
