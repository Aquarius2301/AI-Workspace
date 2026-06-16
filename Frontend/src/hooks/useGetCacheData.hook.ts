import { useQueryClient } from "@tanstack/react-query";

export const useGetCacheData = <T = unknown>(queryKey: any[]) => {
  const queryClient = useQueryClient();

  return queryClient.getQueryData(queryKey) as T;
}
