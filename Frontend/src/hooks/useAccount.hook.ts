import { accountApi } from "@/api";
import type { UserItem } from "@/types";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

export const USERS_QUERY_KEY = ["account", "users"];

export const useAccount = () => {
  const queryClient = useQueryClient();

  const getUsers = (params: { page: number; pageSize: number }) =>
    useQuery({
      queryKey: [...USERS_QUERY_KEY, params.page, params.pageSize],
      queryFn: () => accountApi.getUsers(params),
    });

  const updateProfile = useMutation({
    mutationFn: accountApi.updateProfile,
    onSuccess: (data) => {
      const user = data as unknown as UserItem;
      queryClient.setQueryData(USERS_QUERY_KEY, (old: any) => {
        if (!old?.items) return old;
        return {
          ...old,
          items: old.items.map((item: any) =>
            item.id === user.id ? { ...item, ...user } : item,
          ),
        };
      });
    },
  });

  const changePassword = useMutation({
    mutationFn: (params: {
      id: string;
      data: { oldPassword: string; newPassword: string };
    }) => accountApi.changePassword(params.id, params.data),
  });

  return {
    getUsers,
    updateProfile,
    changePassword,
  };
};
