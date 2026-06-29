import { teamApi } from "@/api";
import type { PageSize, ProjectVisibility, TeamRole } from "@/types";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

export const TEAM_QUERY_KEY = ["teams"] as const;
export const TEAM_LIST_QUERY_KEY = [...TEAM_QUERY_KEY, "list"] as const;
export const TEAM_DETAIL_QUERY_KEY = [...TEAM_QUERY_KEY, "detail"] as const;
export const TEAM_MEMBERS_QUERY_KEY = [...TEAM_QUERY_KEY, "members"] as const;
export const TEAM_ME_QUERY_KEY = [...TEAM_QUERY_KEY, "me"] as const;
export const TEAM_AVAILABLE_MEMBERS_QUERY_KEY = [
  ...TEAM_QUERY_KEY,
  "available-members",
] as const;
export const TEAM_PROJECTS_QUERY_KEY = [...TEAM_QUERY_KEY, "projects"] as const;

// ===================== QUERY HOOKS =====================

export const useTeamList = (
  myTeams: boolean,
  search?: string,
  page?: number,
  pageSize?: PageSize,
) =>
  useQuery({
    queryKey: [...TEAM_LIST_QUERY_KEY, { myTeams, search, page, pageSize }],
    queryFn: () => teamApi.getList(myTeams, search, page, pageSize),
  });

export const useTeamDetail = (id: string, enabled: boolean = true) =>
  useQuery({
    queryKey: [...TEAM_DETAIL_QUERY_KEY, id],
    queryFn: () => teamApi.getDetail(id),
    enabled: !!id && enabled,
  });

export const useTeamMe = (teamId: string, enabled: boolean = true) =>
  useQuery({
    queryKey: [...TEAM_ME_QUERY_KEY, teamId],
    queryFn: () => teamApi.me(teamId),
    enabled: !!teamId && enabled,
  });

export const useTeamMembers = (
  id: string,
  search?: string,
  role?: TeamRole,
  page?: number,
  pageSize?: PageSize,
  enabled: boolean = true,
) =>
  useQuery({
    queryKey: [...TEAM_MEMBERS_QUERY_KEY, id, search, role, page, pageSize],
    queryFn: () => teamApi.getMembers(id, search, role, page, pageSize),
    enabled: !!id && enabled,
  });

export const useTeamAvailableMembers = (
  id: string,
  search?: string,
  page?: number,
  pageSize?: number,
  enabled: boolean = true,
) =>
  useQuery({
    queryKey: [...TEAM_AVAILABLE_MEMBERS_QUERY_KEY, id, search, page, pageSize],
    queryFn: () => teamApi.getAvailableMembers(id, { search, page, pageSize }),
    enabled: !!id && enabled,
  });

export const useTeamProjects = (
  id: string,
  search?: string,
  visibility?: ProjectVisibility,
  page?: number,
  pageSize?: PageSize,
  enabled: boolean = true,
) =>
  useQuery({
    queryKey: [
      ...TEAM_PROJECTS_QUERY_KEY,
      id,
      search,
      visibility,
      page,
      pageSize,
    ],
    queryFn: () => teamApi.getProjects(id, search, visibility, page, pageSize),
    enabled: !!id && enabled,
  });

// ===================== MUTATION HOOKS =====================

export const useTeam = () => {
  const queryClient = useQueryClient();

  const create = useMutation({
    mutationFn: teamApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: TEAM_LIST_QUERY_KEY });
    },
  });

  const update = useMutation({
    mutationFn: (params: {
      id: string;
      data: { name?: string; description?: string };
    }) => teamApi.update(params.id, params.data),
    onSuccess: (_data) => {
      queryClient.invalidateQueries({ queryKey: TEAM_DETAIL_QUERY_KEY });
      queryClient.invalidateQueries({ queryKey: TEAM_LIST_QUERY_KEY });
    },
  });

  const deleteTeam = useMutation({
    mutationFn: teamApi.delete,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: TEAM_LIST_QUERY_KEY });
    },
  });

  const addMembers = useMutation({
    mutationFn: (params: {
      id: string;
      data: { members: { userId: string; role?: TeamRole }[] };
    }) => teamApi.addMembers(params.id, params.data),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: TEAM_MEMBERS_QUERY_KEY,
      });
    },
  });

  const updateMemberRole = useMutation({
    mutationFn: (params: {
      id: string;
      memberId: string;
      data: { role?: TeamRole };
    }) => teamApi.updateMemberRole(params.id, params.memberId, params.data),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: TEAM_MEMBERS_QUERY_KEY,
      });
    },
  });

  const removeMember = useMutation({
    mutationFn: (params: { id: string; memberId: string }) =>
      teamApi.removeMember(params.id, params.memberId),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: TEAM_MEMBERS_QUERY_KEY,
      });
    },
  });

  const leave = useMutation({
    mutationFn: teamApi.leave,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: TEAM_LIST_QUERY_KEY });
      queryClient.invalidateQueries({
        queryKey: TEAM_MEMBERS_QUERY_KEY,
      });
    },
  });

  const transferAdmin = useMutation({
    mutationFn: (params: { id: string; targetUserId: string }) =>
      teamApi.transferAdmin(params.id, params.targetUserId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: TEAM_MEMBERS_QUERY_KEY });
      queryClient.invalidateQueries({ queryKey: TEAM_ME_QUERY_KEY });
    },
  });

  return {
    create,
    update,
    deleteTeam,
    addMembers,
    updateMemberRole,
    removeMember,
    leave,
    transferAdmin,
  };
};
