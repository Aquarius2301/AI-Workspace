import axios, {
  type AxiosError,
  type AxiosInstance,
  type InternalAxiosRequestConfig,
} from "axios";

// ===================== Base URL Configuration =====================

/** Base URL for all API requests, loaded from .env / .env.development */
const BASE_URL: string = import.meta.env.VITE_API_BASE_URL;

if (!BASE_URL) {
  console.warn(
    "[config.api] VITE_API_BASE_URL is not defined. Check your .env file.",
  );
}

// ===================== Axios Client Instance =====================

const axiosClient: AxiosInstance = axios.create({
  baseURL: BASE_URL,
  withCredentials: true, // Required for httpOnly refresh-token cookies
  headers: {
    "Content-Type": "application/json",
  },
});

// ===================== Skip Refresh Paths =====================

/** Endpoints that should NOT trigger a token refresh on 401.
 *  e.g., login/register failures should reject normally without redirect. */
const SKIP_REFRESH_PATHS: readonly string[] = [
  "/api/auth/login",
  "/api/auth/register",
  "/api/auth/refresh",
  "/api/auth/logout",
  "/api/auth/revoke-all-refresh",
];

// Extend the request config type to support the _retry flag
interface RetryableRequestConfig extends InternalAxiosRequestConfig {
  _retry?: boolean;
}

/** Check whether the given request URL belongs to a skip-refresh endpoint. */
function shouldSkipRefresh(url?: string): boolean {
  if (!url) return true;
  return SKIP_REFRESH_PATHS.some((path) => url.includes(path));
}

// ===================== Response Interceptors =====================

axiosClient.interceptors.response.use(
  // ----- Success: Unwrap ApiResponse<T> envelope -----
  (response) => response.data.data,

  // ----- Error: Handle 401 with automatic token refresh -----
  async (error: AxiosError) => {
    const originalRequest = error.config as RetryableRequestConfig | undefined;

    // Reject if there's no request config or no HTTP response
    if (!originalRequest || !error.response) {
      return Promise.reject(error);
    }

    const { status } = error.response;

    // Only handle 401 Unauthorized errors
    if (status !== 401) {
      return Promise.reject(error);
    }

    // Skip refresh for auth-related endpoints (login, register, refresh, logout, etc.)
    if (shouldSkipRefresh(originalRequest.url)) {
      return Promise.reject(error);
    }

    // Skip if this request has already been retried once
    if (originalRequest._retry) {
      return Promise.reject(error);
    }

    // Mark as retried to prevent infinite retry loops
    originalRequest._retry = true;

    try {
      // Attempt to refresh the access token via httpOnly cookie
      await axiosClient.post("/api/auth/refresh");

      // Refresh succeeded — retry the original failed request
      return await axiosClient(originalRequest);
    } catch (refreshError) {
      // Refresh failed (e.g., refresh token expired) — redirect to login
      window.location.href = "/login";
      return Promise.reject(refreshError);
    }
  },
);

export default axiosClient;
