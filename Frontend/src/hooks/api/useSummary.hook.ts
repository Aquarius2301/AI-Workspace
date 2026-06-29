import { summaryApi } from "@/api";
import { useQuery } from "@tanstack/react-query";

export const SUMARY_QUERY_KEY = ["summary"] as const;

// ===================== QUERY HOOKS =====================
export const useSummary = () => {
  return useQuery({
    queryKey: [...SUMARY_QUERY_KEY],
    queryFn: () => summaryApi.get(),
  });
};
