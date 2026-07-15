import { projectApi } from "@/api/project.api";
import type {
  CreateProjectRequest,
  PageSize,
  ProjectRole,
  ProjectVisibility,
  TaskPriority,
  TaskStatus,
} from "@/types";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { SUMMARY_QUERY_KEY } from "./useSummary.hook";

export const PROJECT_QUERY_KEY = ["projects"] as const;
export const PROJECT_LIST_QUERY_KEY = [...PROJECT_QUERY_KEY, "list"] as const;
export const PROJECT_DETAIL_QUERY_KEY = [
  ...PROJECT_QUERY_KEY,
  "detail",
] as const;

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

export const MY_PROJECT_LIST_QUERY_KEY = [
  ...PROJECT_QUERY_KEY,
  "my-list",
] as const;

export const useMyProjectList = (
  search?: string,
  visibility?: ProjectVisibility,
  page?: number,
  pageSize?: PageSize,
  enabled: boolean = true,
) =>
  useQuery({
    queryKey: [
      ...MY_PROJECT_LIST_QUERY_KEY,
      { search, visibility, page, pageSize },
    ],
    queryFn: () => projectApi.getMyList(search, visibility, page, pageSize),
    enabled,
  });

export const useProjectDetailBySlug = (slug: string, enabled: boolean = true) =>
  useQuery({
    queryKey: [...PROJECT_DETAIL_QUERY_KEY, "slug", slug],
    queryFn: () => projectApi.getBySlug(slug),
    enabled: !!slug && enabled,
  });

export const useProjectDetailById = (id: string, enabled: boolean = true) =>
  useQuery({
    queryKey: [...PROJECT_DETAIL_QUERY_KEY, id],
    queryFn: () => projectApi.getById(id),
    enabled: !!id && enabled,
  });

export const useMyTasksByProject = (
  projectId: string,
  search?: string,
  status?: TaskStatus,
  priority?: TaskPriority,
  page?: number,
  pageSize?: PageSize,
  enabled: boolean = true,
) =>
  useQuery({
    queryKey: [
      ...PROJECT_QUERY_KEY,
      projectId,
      "my-tasks",
      { search, status, priority, page, pageSize },
    ],
    queryFn: () =>
      projectApi.getMyTasks(
        projectId,
        search,
        status,
        priority,
        page,
        pageSize,
      ),
    enabled: !!projectId && enabled,
  });

export const useProjectMembers = (
  projectId: string,
  search?: string,
  role?: ProjectRole,
  page?: number,
  pageSize?: PageSize,
  enabled: boolean = true,
) =>
  useQuery({
    queryKey: [
      ...PROJECT_QUERY_KEY,
      projectId,
      "members",
      { search, role, page, pageSize },
    ],
    queryFn: () =>
      projectApi.getMembers(projectId, search, role, page, pageSize),
    enabled: !!projectId && enabled,
  });

export const useCreateProject = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: CreateProjectRequest) => projectApi.create(request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: PROJECT_LIST_QUERY_KEY });
      queryClient.invalidateQueries({ queryKey: SUMMARY_QUERY_KEY });
    },
  });
};
