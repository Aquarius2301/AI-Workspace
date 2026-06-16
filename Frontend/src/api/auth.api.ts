import type {
  LoginRequest,
  RegisterRequest,
  AuthResponse,
  UserResponse,
} from "@/types";
import axiosClient from "./config.api";

const baseUrl = "/api/auth";

export const authApi = {
  login: (data: LoginRequest): Promise<AuthResponse> => {
    return axiosClient.post(`${baseUrl}/login`, data);
  },

  register: (data: RegisterRequest): Promise<AuthResponse> => {
    return axiosClient.post(`${baseUrl}/register`, data);
  },

  refresh: (): Promise<AuthResponse> => {
    return axiosClient.post(`${baseUrl}/refresh`);
  },

  logout: (): Promise<void> => {
    return axiosClient.post(`${baseUrl}/logout`);
  },

  revokeAllRefresh: (): Promise<void> => {
    return axiosClient.post(`${baseUrl}/revoke-all-refresh`);
  },

  me: (): Promise<UserResponse> => {
    return axiosClient.get(`${baseUrl}/me`);
  },

  updateActive: (): Promise<void> => {
    return axiosClient.post(`${baseUrl}/me/active`);
  },
};
