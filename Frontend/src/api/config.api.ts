import axios, {
  AxiosError,
  type AxiosInstance,
  type InternalAxiosRequestConfig,
} from "axios";

/**
 * Base URL của backend API.
 * Có thể ghi đè bằng biến môi trường VITE_API_BASE_URL.
 */
const API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL || "http://localhost:8080";

/**
 * File log giả lập ở phía trình duyệt.
 * Trình duyệt không thể ghi trực tiếp ra file vật lý, nên ta lưu log vào localStorage
 * để có thể xem lại/đồng bộ sau này nếu cần.
 */
const AXIOS_LOG_STORAGE_KEY = "axios-client-log";

/**
 * Key lưu thời gian active gần nhất.
 */
const ACTIVE_TIMESTAMP_KEY = "last-active-timestamp";

/**
 * Ngưỡng thời gian (10 phút) để gọi /api/auth/me/active.
 */
const ACTIVE_THRESHOLD_MS = 10 * 60 * 1000;

/**
 * Thêm một dòng log vào "file log" cục bộ.
 */
function appendToLogFile(message: string): void {
  try {
    const currentLogs = localStorage.getItem(AXIOS_LOG_STORAGE_KEY) ?? "";
    const nextLogs = currentLogs ? `${currentLogs}\n${message}` : message;
    localStorage.setItem(AXIOS_LOG_STORAGE_KEY, nextLogs);
  } catch {
    // Nếu localStorage không khả dụng thì bỏ qua, console log vẫn hoạt động.
  }
}

/**
 * Log request/response ra console và lưu vào log cục bộ.
 */
function writeRequestLog(url: string, body: unknown): void {
  console.log(`Request: ${url}`);
  appendToLogFile(
    `Request ${url}\nBody:\n${JSON.stringify(body ?? {}, null, 2)}\n--------------------------`,
  );
}

function writeResponseLog(url: string, status: number, body: unknown): void {
  console.log(`Response: ${status}`);
  appendToLogFile(
    `Response ${url}\nStatus: ${status}\nBody:\n${JSON.stringify(body ?? {}, null, 2)}\n--------------------------`,
  );
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
    writeRequestLog(url, config.data);

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
 * - Khi gặp 401 ở trang ngoài login/refresh thì gọi refresh() rồi retry request một lần
 */
axiosClient.interceptors.response.use(
  (response) => {
    const url = `${response.config.baseURL ?? ""}${response.config.url ?? ""}`;
    writeResponseLog(url, response.status, response.data);
    return response.data;
  },
  async (error: AxiosError) => {
    const originalRequest = error.config as
      | (InternalAxiosRequestConfig & { _retry?: boolean })
      | undefined;
    const status = error.response?.status;
    const url = `${originalRequest?.baseURL ?? ""}${originalRequest?.url ?? ""}`;

    // Log lỗi response ra console và file log cục bộ.
    console.log(`Response: ${status ?? "UNKNOWN"}`);
    appendToLogFile(
      `Response ${url}\nStatus: ${status ?? "UNKNOWN"}\nBody:\n${JSON.stringify(error.response?.data ?? {}, null, 2)}\n--------------------------`,
    );

    // Nếu không phải 401 hoặc là endpoint auth thì trả lỗi luôn.
    if (
      status !== 401 ||
      !originalRequest ||
      isAuthEndpoint(originalRequest.url) ||
      originalRequest._retry
    ) {
      return Promise.reject(error);
    }

    originalRequest._retry = true;

    try {
      // Gọi refresh token khi bị 401 ở các màn hình ngoài login.
      await refreshClient.post("/api/auth/refresh");

      // Sau khi refresh thành công, gọi lại API ban đầu.
      return axiosClient(originalRequest);
    } catch (refreshError) {
      return Promise.reject(refreshError);
    }
  },
);

export default axiosClient;
