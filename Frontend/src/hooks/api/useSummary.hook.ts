// ===================== QUERY HOOKS ========================

import { summaryApi } from "@/api";
import { useQuery } from "@tanstack/react-query";

export const SUMMARY_QUERY_KEY = ["summary"] as const;

export const useSummary = (enabled: boolean = true) => {
  return useQuery({
    queryKey: SUMMARY_QUERY_KEY,
    queryFn: summaryApi.get,
    enabled,
  });
};
