import type { AuthResponse, LoginRequest, RegisterRequest, SessionResult } from "@/types";
import axiosClient from "./config.api";

const baseUrl = "/api/auth";

export const authApi = {
  login: (data: LoginRequest): Promise<string> => {
    return axiosClient.post(`${baseUrl}/login`, data);
  },

  register: (data: RegisterRequest): Promise<string> => {
    return axiosClient.post(`${baseUrl}/register`, data);
  },

  refresh: (): Promise<string> => {
    return axiosClient.post(`${baseUrl}/refresh`);
  },

  logout: (): Promise<string> => {
    return axiosClient.post(`${baseUrl}/logout`);
  },

  revokeAllRefresh: (): Promise<string> => {
    return axiosClient.post(`${baseUrl}/revoke-all-refresh`);
  },

  getSessions: (): Promise<SessionResult[]> => {
    return axiosClient.get(`${baseUrl}/sessions`);
  },

  revokeSession: (deviceId: string): Promise<string> => {
    return axiosClient.delete(`${baseUrl}/sessions/${deviceId}`);
  },

  me: (): Promise<AuthResponse> => {
    return axiosClient.get(`${baseUrl}/me`);
  },

  updateActive: (): Promise<string> => {
    return axiosClient.post(`${baseUrl}/me/active`);
  },
};
