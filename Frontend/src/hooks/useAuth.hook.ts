import { authApi } from "@/api";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

export const MeQueryKey = ["auth", "me"];

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
      localStorage.clear(); //remove all local storage items related to auth
    },
  });

  const revokeAllRefresh = useMutation({
    mutationFn: authApi.revokeAllRefresh,
  });

  const me = useQuery({
    queryKey: MeQueryKey,
    queryFn: authApi.me,
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
    me,
    updateActive,
  };
};
