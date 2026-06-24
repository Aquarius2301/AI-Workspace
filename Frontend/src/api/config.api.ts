import axios, {
  AxiosError,
  type AxiosInstance,
  type InternalAxiosRequestConfig,
} from "axios";

/**
 * Base URL của backend API.
 * Có thể ghi đè bằng biến môi trường VITE_API_BASE_URL.
 */
// Cảnh báo nếu dùng HTTP trong production (cookie Secure=true sẽ không hoạt động với HTTP)
const API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL || "http://localhost:8080";

if (!import.meta.env.DEV && !API_BASE_URL.startsWith("https://")) {
  console.warn(
    "[SECURITY] API_BASE_URL is not HTTPS. Cookies with Secure=true will be rejected by the browser.",
  );
}

/**
 * Key lưu thời gian active gần nhất.
 */
const ACTIVE_TIMESTAMP_KEY = "last-active-timestamp";

/**
 * Ngưỡng thời gian (10 phút) để gọi /api/auth/me/active.
 */
const ACTIVE_THRESHOLD_MS = 10 * 60 * 1000;

/**
 * Flag để kiểm tra xem refresh token đang trong quá trình thực hiện không.
 * Ngăn chặn nhiều request gọi refresh cùng lúc gây ra race condition.
 */
let isRefreshing = false;

/**
 * Queue subscriber cho các request bị 401 khi refresh token đang chạy.
 * resolve được gọi khi refresh thành công, reject khi refresh thất bại.
 */
interface RefreshSubscriber {
  resolve: (value: unknown) => void;
  reject: (error: unknown) => void;
}

let refreshSubscribers: RefreshSubscriber[] = [];

/**
 * Log request ra console (chỉ ở dev mode, KHÔNG log request body để tránh leak password).
 */
function writeRequestLog(url: string): void {
  if (import.meta.env.DEV) {
    console.log(`[API] Request: ${url}`);
  }
}

function writeResponseLog(url: string, status: number): void {
  if (import.meta.env.DEV) {
    console.log(`[API] Response: ${url} → ${status}`);
  }
}

/**
 * Xác định request có thuộc nhóm đăng nhập hay không.
 * Nếu là login/refresh thì không tự động refresh token khi bị 401.
 */
function isAuthEndpoint(url?: string): boolean {
  if (!url) return false;
  return url.includes("/api/auth/login") || url.includes("/api/auth/refresh");
}

/**
 * Đọc thời gian active gần nhất từ localStorage.
 */
function getLastActiveTimestamp(): number {
  try {
    const stored = localStorage.getItem(ACTIVE_TIMESTAMP_KEY);
    return stored ? parseInt(stored, 10) : 0;
  } catch {
    return 0;
  }
}

/**
 * Ghi thời gian hiện tại vào localStorage.
 */
function setLastActiveTimestamp(): void {
  try {
    localStorage.setItem(ACTIVE_TIMESTAMP_KEY, String(Date.now()));
  } catch {
    // ignore
  }
}

/**
 * Kiểm tra xem đã quá 10 phút kể từ lần active gần nhất chưa.
 */
function shouldUpdateActive(): boolean {
  const lastActive = getLastActiveTimestamp();
  if (lastActive === 0) return true;
  return Date.now() - lastActive > ACTIVE_THRESHOLD_MS;
}

/**
 * Lưu thời gian hiện tại ngay khi module được load (người dùng mở web).
 */
setLastActiveTimestamp();

/**
 * Tạo một axios client riêng dùng để refresh token, tránh bị interceptor lặp vô hạn.
 */
const refreshClient = axios.create({
  baseURL: API_BASE_URL,
  timeout: 10000,
  headers: {
    "Content-Type": "application/json",
  },
  withCredentials: true,
});

const axiosClient: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
  timeout: 10000,
  headers: {
    "Content-Type": "application/json",
  },
  withCredentials: true,
});

/**
 * Gắn interceptor cho request:
 * - Log request ra console
 * - Lưu request body vào log cục bộ
 * - Kiểm tra thời gian active, nếu > 10 phút thì gọi /api/auth/me/active trước
 */
axiosClient.interceptors.request.use(
  async (config: InternalAxiosRequestConfig) => {
    const url = `${config.url ?? ""}`;
    writeRequestLog(url);

    // Chỉ áp dụng cho tất cả api ngoài login
    if (!url.includes("/api/auth/login") && shouldUpdateActive()) {
      try {
        await refreshClient.post("/api/auth/me/active");
        setLastActiveTimestamp();
      } catch {
        // Nếu gọi active thất bại thì vẫn cho request gốc đi tiếp
      }
    }

    return config;
  },
);

/**
 * Gắn interceptor cho response:
 * - Log response thành công
 * - Khi gặp 401 ở trang ngoài login/refresh thì:
 *   1) Nếu đang refresh → queue request lại
 *   2) Nếu chưa refresh → gọi refresh một lần, resolve tất cả queue
 */
axiosClient.interceptors.response.use(
  (response) => {
    const url = `${response.config.baseURL ?? ""}${response.config.url ?? ""}`;
    writeResponseLog(url, response.status);
    return response.data;
  },
  async (error: AxiosError) => {
    const originalRequest = error.config as
      | (InternalAxiosRequestConfig & { _retry?: boolean })
      | undefined;
    const status = error.response?.status;
    const url = `${originalRequest?.baseURL ?? ""}${originalRequest?.url ?? ""}`;

    // Log lỗi response ra console (chỉ dev mode, không log body)
    if (import.meta.env.DEV) {
      console.log(`[API] Response Error: ${url} → ${status ?? "UNKNOWN"}`);
    }

    // Nếu không phải 401 hoặc là endpoint auth thì trả lỗi luôn.
    if (
      status !== 401 ||
      !originalRequest ||
      isAuthEndpoint(originalRequest.url) ||
      originalRequest._retry
    ) {
      return Promise.reject(error);
    }

    // Nếu refresh token đang thực hiện, thêm request này vào queue
    if (isRefreshing) {
      return new Promise((resolve, reject) => {
        refreshSubscribers.push({
          resolve: (token: unknown) => {
            // Thêm token mới vào header và retry request
            originalRequest.headers = originalRequest.headers || {};
            originalRequest.headers.Authorization = `Bearer ${token}`;
            resolve(axiosClient(originalRequest));
          },
          reject,
        });
      });
    }

    // Đánh dấu đang refresh
    isRefreshing = true;
    originalRequest._retry = true;

    try {
      // Gọi refresh token khi bị 401 ở các màn hình ngoài login.
      const response = await refreshClient.post("/api/auth/refresh");
      const accessToken = response.data?.accessToken;

      // Resolve tất cả request đang chờ trong queue với token mới
      refreshSubscribers.forEach((sub) => sub.resolve(accessToken));
      refreshSubscribers = []; // Clear queue

      // Reset flag
      isRefreshing = false;

      // Sau khi refresh thành công, gọi lại API ban đầu với token mới
      originalRequest.headers = originalRequest.headers || {};
      originalRequest.headers.Authorization = `Bearer ${accessToken}`;
      return axiosClient(originalRequest);
    } catch (refreshError) {
      // Nếu refresh thất bại, reject tất cả request trong queue
      refreshSubscribers.forEach((sub) => sub.reject(refreshError));
      refreshSubscribers = [];
      isRefreshing = false;

      return Promise.reject(refreshError);
    }
  },
);

export default axiosClient;
