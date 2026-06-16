export interface UserItem {
  id: string;
  name: string;
  email: string;
}

export interface UpdateProfileRequest {
  name?: string;
  avatarUrl?: string;
}

export interface ChangePasswordRequest {
  oldPassword: string;
  newPassword: string;
}