import { projectApi } from "@/api/project.api";
import type {
  AddProjectMembersRequest,
  CreateProjectRequest,
  PageSize,
  ProjectRole,
  ProjectVisibility,
  UpdateProjectRequest,
} from "@/types";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { SUMMARY_QUERY_KEY } from "./useSummary.hook";

export const PROJECT_QUERY_KEY = ["projects"] as const;
export const PROJECT_LIST_QUERY_KEY = [...PROJECT_QUERY_KEY, "list"] as const;
export const PROJECT_DETAIL_QUERY_KEY = [
  ...PROJECT_QUERY_KEY,
  "detail",
] as const;
export const PROJECT_MEMBERS_QUERY_KEY = [
  ...PROJECT_QUERY_KEY,
  "members",
] as const;
export const PROJECT_AVAILABLE_MEMBERS_QUERY_KEY = [
  ...PROJECT_QUERY_KEY,
  "available-members",
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
      ...PROJECT_MEMBERS_QUERY_KEY,
      projectId,
      { search, role, page, pageSize },
    ],
    queryFn: () =>
      projectApi.getMembers(projectId, search, role, page, pageSize),
    enabled: !!projectId && enabled,
  });

export const useProjectAvailableMembers = (
  projectId: string,
  search?: string,
  page?: number,
  pageSize?: PageSize,
  enabled?: boolean,
) =>
  useQuery({
    queryKey: [
      ...PROJECT_AVAILABLE_MEMBERS_QUERY_KEY,
      projectId,
      { search, page, pageSize },
    ],
    queryFn: () =>
      projectApi.getAvailableMembers(projectId, search, page, pageSize),
    enabled: !!projectId && enabled,
  });

export const useAddProjectMembers = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (params: {
      projectId: string;
      data: AddProjectMembersRequest;
    }) => projectApi.addMembers(params.projectId, params.data),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: PROJECT_MEMBERS_QUERY_KEY,
      });
      queryClient.invalidateQueries({
        queryKey: PROJECT_AVAILABLE_MEMBERS_QUERY_KEY,
      });
    },
  });
};

export const useUpdateProjectMemberRole = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (params: {
      projectId: string;
      memberId: string;
      data: { role?: ProjectRole };
    }) =>
      projectApi.updateMemberRole(
        params.projectId,
        params.memberId,
        params.data,
      ),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: PROJECT_MEMBERS_QUERY_KEY,
      });
    },
  });
};

export const useRemoveProjectMember = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (params: { projectId: string; memberId: string }) =>
      projectApi.removeMember(params.projectId, params.memberId),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: PROJECT_MEMBERS_QUERY_KEY,
      });
    },
  });
};

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

export const useDeleteProject = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => projectApi.deleteProject(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: PROJECT_LIST_QUERY_KEY });
      queryClient.invalidateQueries({ queryKey: MY_PROJECT_LIST_QUERY_KEY });
      queryClient.invalidateQueries({ queryKey: SUMMARY_QUERY_KEY });
    },
  });
};

export const useUpdateProject = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (params: { id: string; data: UpdateProjectRequest }) =>
      projectApi.update(params.id, params.data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: PROJECT_DETAIL_QUERY_KEY });
      queryClient.invalidateQueries({ queryKey: PROJECT_LIST_QUERY_KEY });
      queryClient.invalidateQueries({ queryKey: MY_PROJECT_LIST_QUERY_KEY });
    },
  });
};
