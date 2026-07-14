import { projectApi } from "@/api/project.api";
import type {
  CreateProjectRequest,
  PageSize,
  ProjectVisibility,
} from "@/types";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

export const PROJECT_QUERY_KEY = ["projects"] as const;
export const PROJECT_LIST_QUERY_KEY = [...PROJECT_QUERY_KEY, "list"] as const;

export const useProjectList = (
  id: string,
  search?: string,
  visibility?: ProjectVisibility,
  page?: number,
  pageSize?: PageSize,
  enabled: boolean = true,
) =>
  useQuery({
    queryKey: [
      ...PROJECT_LIST_QUERY_KEY,
      id,
      { search, visibility, page, pageSize },
    ],
    queryFn: () =>
      projectApi.getListByTeam(id, search, visibility, page, pageSize),
    enabled: !!id && enabled,
  });

export const useCreateProject = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: CreateProjectRequest) => projectApi.create(request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: PROJECT_LIST_QUERY_KEY });
    },
  });
};
