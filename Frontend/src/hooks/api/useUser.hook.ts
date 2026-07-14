import { userApi } from "@/api";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { AUTH_ME_QUERY_KEY } from "./useAuth.hook";

export const useUpdateProfile = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: userApi.updateProfile,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: AUTH_ME_QUERY_KEY });
    },
  });
};

export const useUploadPicture = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: userApi.uploadPicture,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: AUTH_ME_QUERY_KEY });
    },
  });
};
