import { authApi } from "@/api";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

export const AUTH_QUERY_KEY = ["auth"] as const;

export const AUTH_ME_QUERY_KEY = [...AUTH_QUERY_KEY, "me"] as const;
export const AUTH_SESSIONS_QUERY_KEY = [...AUTH_QUERY_KEY, "sessions"] as const;

// ===================== QUERY HOOKS ========================

export const useAuthMe = (options?: { enabled?: boolean }) => {
  return useQuery({
    queryKey: AUTH_ME_QUERY_KEY,
    queryFn: authApi.me,
    ...options,
  });
};

export const useSessionsQuery = (options?: { enabled?: boolean }) => {
  return useQuery({
    queryKey: AUTH_SESSIONS_QUERY_KEY,
    queryFn: authApi.getSessions,
    ...options,
  });
};

// ===================== MUTATION HOOKS =====================
export const useAuth = () => {
  const queryClient = useQueryClient();

  const login = useMutation({
    mutationFn: authApi.login,
  });

  const register = useMutation({
    mutationFn: authApi.register,
  });

  const refresh = useMutation({
    mutationFn: authApi.refresh,
  });

  const logout = useMutation({
    mutationFn: authApi.logout,
    onSuccess: () => {
      queryClient.clear();
      localStorage.removeItem("last-active-timestamp");
      localStorage.removeItem("axios-client-log");
    },
  });

  const revokeAllRefresh = useMutation({
    mutationFn: authApi.revokeAllRefresh,
    onSuccess: () => {
      queryClient.clear();
      localStorage.removeItem("last-active-timestamp");
      localStorage.removeItem("axios-client-log");
    },
  });

  const revokeSession = useMutation({
    mutationFn: authApi.revokeSession,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: AUTH_SESSIONS_QUERY_KEY });
    },
  });

  const updateActive = useMutation({
    mutationFn: authApi.updateActive,
  });

  return {
    login,
    register,
    refresh,
    logout,
    revokeAllRefresh,
    revokeSession,
    updateActive,
  };
};
