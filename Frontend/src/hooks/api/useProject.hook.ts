import { projectApi } from "@/api";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { TEAM_PROJECTS_QUERY_KEY } from "./useTeam.hook";

export const PROJECT_QUERY_KEY = ["projects"] as const;

// ===================== MUTATION HOOKS =====================
export function useProject() {
  const queryClient = useQueryClient();

  const create = useMutation({
    mutationFn: projectApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: TEAM_PROJECTS_QUERY_KEY });
    },
  });

  return { create };
}
