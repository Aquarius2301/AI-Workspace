import { userApi } from "@/api";
import type { PageSize } from "@/types";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { AUTH_ME_QUERY_KEY } from "./useAuth.hook";

export const USER_QUERY_KEY = ["user"] as const;

export const useGetUsers = (page: number, pageSize: PageSize) =>
  useQuery({
    queryKey: [...USER_QUERY_KEY, page, pageSize],
    queryFn: () => userApi.getUsers(page, pageSize),
  });

export const useUser = () => {
  const queryClient = useQueryClient();
  const updateProfile = useMutation({
    mutationFn: userApi.updateProfile,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: AUTH_ME_QUERY_KEY });
    },
  });

  const changePassword = useMutation({
    mutationFn: (params: { oldPassword: string; newPassword: string }) =>
      userApi.changePassword(params),
  });

  return {
    updateProfile,
    changePassword,
  };
};
