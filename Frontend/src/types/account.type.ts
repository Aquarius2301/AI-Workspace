import type { LanguageDisplay } from "./auth.type";

export interface UserItem {
  id: string;
  name: string;
  email: string;
}

export interface UpdateProfileRequest {
  name?: string;
  avatarUrl?: string;
  language?: LanguageDisplay;
}

export interface ChangePasswordRequest {
  oldPassword: string;
  newPassword: string;
}

export interface UploadAvatarResponse {
  avatarUrl: string;
}
