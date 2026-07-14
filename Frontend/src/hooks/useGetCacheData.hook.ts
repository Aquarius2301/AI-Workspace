import { useQueryClient, type QueryKey } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

/**
 * Retrieves cached data for a given query key reactively.
 * Uses useQuery with enabled:false to subscribe to the query cache.
 * When the original query refetches, this hook will re-render with updated data.
 *
 * Note: queryFn is still required by TanStack Query v5 even when enabled:false.
 * It reads from cache directly to avoid making a duplicate API call.
 *
 * @param queryKey - The query key for which to retrieve cached data.
 * @returns The cached data associated with the query key, or undefined if no data is found.
 */
export const useGetCacheData = <T>(queryKey: QueryKey) => {
  const queryClient = useQueryClient();

  const { data } = useQuery<T | undefined>({
    queryKey,
    queryFn: () =>
      queryClient.getQueryData<T>(queryKey) ??
      Promise.reject(new Error("Not in cache")),
    enabled: false,
    staleTime: Infinity,
    gcTime: Infinity,
    retry: false,
  });

  return data;
};
