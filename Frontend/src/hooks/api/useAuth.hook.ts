import { authApi } from "@/api";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

export const AUTH_QUERY_KEY = ["auth"] as const;

export const AUTH_ME_QUERY_KEY = [...AUTH_QUERY_KEY, "me"] as const;
export const AUTH_SESSIONS_QUERY_KEY = [...AUTH_QUERY_KEY, "sessions"] as const;

// ===================== QUERY HOOKS ========================

export const useAuthMe = (enabled: boolean = true) => {
  return useQuery({
    queryKey: AUTH_ME_QUERY_KEY,
    queryFn: authApi.me,
    enabled,
    staleTime: Infinity,
    gcTime: 1000 * 60 * 30,
  });
};

export const useSessionsQuery = (enabled: boolean = true) => {
  return useQuery({
    queryKey: AUTH_SESSIONS_QUERY_KEY,
    queryFn: authApi.getSessions,
    enabled,
  });
};

// ===================== MUTATION HOOKS =====================
export const useLogin = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: authApi.login,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: AUTH_ME_QUERY_KEY });
    },
  });
};

export const useRegister = () => {
  return useMutation({ mutationFn: authApi.register });
};

export const useLogout = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: authApi.logout,
    onSuccess: () => {
      queryClient.clear();
    },
  });
};

export const useRevokeSession = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: authApi.revokeSession,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: AUTH_SESSIONS_QUERY_KEY });
    },
  });
};

export const useRevokeAllSessions = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: authApi.revokeAllRefresh,
    onSuccess: () => {
      queryClient.clear();
    },
  });
};
