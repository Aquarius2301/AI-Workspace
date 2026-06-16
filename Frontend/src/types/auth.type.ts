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
  name: string;
  email: string;
  avatarUrl: string;
  createdAt: string;
}

export interface UserResponse {
  avatar: string;
  name: string;
  email: string;
}