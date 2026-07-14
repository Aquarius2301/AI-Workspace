import axios, {
  type AxiosInstance,
  type AxiosError,
  type InternalAxiosRequestConfig,
  type AxiosResponse,
} from "axios";
import { ENDPOINTS, ROUTE } from "@/constants";
import type { ApiResponse, ApiErrorResponse } from "@/types/common.type";

// ---------- Types ----------
interface QueueItem {
  resolve: (value: InternalAxiosRequestConfig) => void;
  reject: (reason: unknown) => void;
  config: InternalAxiosRequestConfig;
}

// ---------- Constants ----------
const AUTH_WHITELIST = [
  ENDPOINTS.AUTH.LOGIN,
  ENDPOINTS.AUTH.REGISTER,
  ENDPOINTS.AUTH.REFRESH,
  ENDPOINTS.AUTH.LOGOUT,
] as const;

const isAuthEndpoint = (url: string): boolean => {
  return AUTH_WHITELIST.some((endpoint) => url.includes(endpoint));
};

// ---------- Queue management ----------
let isRefreshing = false;
let failedQueue: QueueItem[] = [];

const processQueue = (error: AxiosError | null): void => {
  failedQueue.forEach((item) => {
    if (error) {
      item.reject(error);
    } else {
      item.resolve(item.config);
    }
  });
  failedQueue = [];
};

// ---------- Axios instance ----------
const axiosClient: AxiosInstance = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5000/api",
  withCredentials: true,
  headers: {
    "Content-Type": "application/json",
  },
  timeout: 30000,
});

// ---------- Request Interceptor ----------
axiosClient.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    return config;
  },
  (error: AxiosError) => Promise.reject(error),
);

// ---------- Response Interceptor - Success ----------
axiosClient.interceptors.response.use(
  (response: AxiosResponse<ApiResponse<any>>) => {
    return response.data.data;
  },
  // ---------- Response Interceptor - Error ----------
  async (error: AxiosError<ApiErrorResponse>) => {
    const originalRequest = error.config as InternalAxiosRequestConfig & {
      _retry?: boolean;
    };

    if (!error.response) {
      return Promise.reject(error);
    }

    const { status, config } = error.response;
    const requestUrl = config?.url ?? "";

    // 401 Unauthorized
    if (status === 401) {
      // No refresh for auth endpoints (login, register, refresh, logout)
      if (isAuthEndpoint(requestUrl)) {
        return Promise.reject(error.response.data);
      }

      // Avoid looping
      if (originalRequest._retry) {
        return Promise.reject(error.response.data);
      }

      // Refreshing -> queue this request
      if (isRefreshing) {
        return new Promise<InternalAxiosRequestConfig>((resolve, reject) => {
          failedQueue.push({
            resolve,
            reject,
            config: originalRequest,
          });
        })
          .then((config) => axiosClient(config))
          .catch((err) => Promise.reject(err));
      }

      // Start refreshing
      originalRequest._retry = true;
      isRefreshing = true;

      try {
        // Fetch refresh endpoint
        await axiosClient.post(ENDPOINTS.AUTH.REFRESH);

        // Refresh successful -> process queue and retry original request
        processQueue(null);
        return axiosClient(originalRequest);
      } catch (refreshError) {
        // Refresh failed -> process queue with error, redirect login
        processQueue(refreshError as AxiosError);
        window.location.href = ROUTE.LOGIN;
        return Promise.reject(refreshError);
      } finally {
        isRefreshing = false;
      }
    }

    return Promise.reject(error.response.data);
  },
);

export default axiosClient;
