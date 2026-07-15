import { teamApi } from "@/api";
import type {
  AddTeamMemberRequest,
  PageSize,
  TeamRole,
  UpdateMemberRoleRequest,
} from "@/types";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { SUMMARY_QUERY_KEY } from "./useSummary.hook";
import { PROJECT_QUERY_KEY } from "./useProject.hook";

export const TEAM_QUERY_KEY = ["teams"] as const;
export const TEAM_LIST_QUERY_KEY = [...TEAM_QUERY_KEY, "list"] as const;
export const TEAM_DETAIL_QUERY_KEY = [...TEAM_QUERY_KEY, "detail"] as const;
export const TEAM_DETAIL_SLUG_QUERY_KEY = [
  ...TEAM_DETAIL_QUERY_KEY,
  "slug",
] as const;

export const TEAM_MEMBERS_QUERY_KEY = [...TEAM_QUERY_KEY, "members"] as const;
export const TEAM_AVAILABLE_MEMBERS_QUERY_KEY = [
  ...TEAM_QUERY_KEY,
  "available-members",
] as const;

// ===================== QUERY HOOKS =====================

export const useTeamList = (
  search?: string,
  page?: number,
  pageSize?: PageSize,
) =>
  useQuery({
    queryKey: [...TEAM_LIST_QUERY_KEY, { search, page, pageSize }],
    queryFn: () => teamApi.getList(search, page, pageSize),
  });

export const useTeamDetailBySlug = (slug: string, enabled?: boolean) => {
  const getId = useQuery({
    queryKey: [...TEAM_DETAIL_SLUG_QUERY_KEY, slug],
    queryFn: () => teamApi.getId(slug),
    enabled: !!slug && enabled,
  });

  const teamDetail = useTeamDetailById(getId.data?.id!);

  return {
    ...teamDetail,
    data: teamDetail.data,
    isLoading: getId.isLoading || teamDetail.isLoading,
    isFetching: getId.isFetching || teamDetail.isFetching,
    isError: getId.isError || teamDetail.isError,
    error: getId.error ?? teamDetail.error,
  };
};

export const useTeamDetailById = (id: string, enabled?: boolean) => {
  return useQuery({
    queryKey: [...TEAM_DETAIL_QUERY_KEY, id],
    queryFn: () => teamApi.getDetail(id),
    enabled: !!id && enabled,
  });
};

export const useTeamMembers = (
  id: string,
  search?: string,
  role?: TeamRole,
  page?: number,
  pageSize?: number,
  enabled?: boolean,
) => {
  return useQuery({
    queryKey: [...TEAM_MEMBERS_QUERY_KEY, id, { search, role, page, pageSize }],
    queryFn: () => teamApi.getMembers(id, search, role, page, pageSize),
    enabled: !!id && enabled,
  });
};

export const useTeamAvailableMembers = (
  id: string,
  search?: string,
  page?: number,
  pageSize?: number,
  enabled?: boolean,
) => {
  return useQuery({
    queryKey: [
      ...TEAM_AVAILABLE_MEMBERS_QUERY_KEY,
      id,
      { search, page, pageSize },
    ],
    queryFn: () => teamApi.getAvailableMembers(id, search, page, pageSize),
    enabled: !!id && enabled,
  });
};

// ===================== MUTATION HOOKS =====================

export const useCreateTeam = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: teamApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: TEAM_LIST_QUERY_KEY });
      queryClient.invalidateQueries({ queryKey: SUMMARY_QUERY_KEY });
    },
  });
};

export const useUpdateTeam = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (params: {
      id: string;
      data: { name?: string; description?: string };
    }) => teamApi.update(params.id, params.data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: TEAM_LIST_QUERY_KEY });
      queryClient.invalidateQueries({ queryKey: TEAM_DETAIL_QUERY_KEY });
    },
  });
};

export const useAddMembers = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (params: { id: string; data: AddTeamMemberRequest }) =>
      teamApi.addMembers(params.id, params.data),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: TEAM_MEMBERS_QUERY_KEY,
      });
      queryClient.invalidateQueries({
        queryKey: TEAM_AVAILABLE_MEMBERS_QUERY_KEY,
      });
    },
  });
};

export const useUpdateMemberRole = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (params: {
      id: string;
      memberId: string;
      data: UpdateMemberRoleRequest;
    }) => teamApi.updateMemberRole(params.id, params.memberId, params.data),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: TEAM_MEMBERS_QUERY_KEY,
      });
    },
  });
};

export const useDeleteTeam = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => teamApi.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: TEAM_LIST_QUERY_KEY });
      queryClient.invalidateQueries({ queryKey: SUMMARY_QUERY_KEY });
    },
  });
};

export const useDeleteMember = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (params: { id: string; memberId: string }) =>
      teamApi.deleteMember(params.id, params.memberId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: TEAM_MEMBERS_QUERY_KEY });
      queryClient.invalidateQueries({ queryKey: PROJECT_QUERY_KEY });
    },
  });
};
