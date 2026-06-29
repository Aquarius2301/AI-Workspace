import { useQueryClient, type QueryKey } from "@tanstack/react-query";

/**
 * Retrieves cached data for a given query key.
 * @param queryKey - The query key for which to retrieve cached data.
 * @returns The cached data associated with the query key, or undefined if no data is found.
 */
export const useGetCacheData = <T>(queryKey: QueryKey) => {
  const queryClient = useQueryClient();

  return queryClient.getQueryData<T>(queryKey);
};
