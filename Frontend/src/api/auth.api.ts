import type {
  AuthResponse,
  LoginRequest,
  RegisterRequest,
  SessionResult,
} from "@/types";
import axiosClient from "./config.api";
import { ENDPOINTS } from "@/constants";

export const authApi = {
  login: (data: LoginRequest): Promise<void> => {
    return axiosClient.post(ENDPOINTS.AUTH.LOGIN, data);
  },

  register: (data: RegisterRequest): Promise<void> => {
    return axiosClient.post(ENDPOINTS.AUTH.REGISTER, data);
  },

  refresh: (): Promise<void> => {
    return axiosClient.post(ENDPOINTS.AUTH.REFRESH);
  },

  logout: (): Promise<void> => {
    return axiosClient.post(ENDPOINTS.AUTH.LOGOUT);
  },

  revokeAllRefresh: (): Promise<void> => {
    return axiosClient.delete(ENDPOINTS.AUTH.SESSION_ALL);
  },

  getSessions: (): Promise<SessionResult[]> => {
    return axiosClient.get(ENDPOINTS.AUTH.SESSION);
  },

  revokeSession: (deviceId: string): Promise<void> => {
    return axiosClient.delete(ENDPOINTS.AUTH.SESSION_DEVICE(deviceId));
  },

  me: (): Promise<AuthResponse> => {
    return axiosClient.get(ENDPOINTS.AUTH.ME);
  },
};
