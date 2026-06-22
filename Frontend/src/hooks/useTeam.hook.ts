import { teamApi } from "@/api";
import type { TeamRole } from "@/types";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

export const TEAMS_QUERY_KEY = ["teams", "list"];
export const TEAM_DETAIL_QUERY_KEY = ["teams", "detail"];
export const TEAM_MEMBERS_QUERY_KEY = ["teams", "members"];
export const TEAM_ME_QUERY_KEY = ["teams", "me"];
export const TEAM_AVAILABLE_MEMBERS_QUERY_KEY = ["teams", "available-members"];

export const useTeam = () => {
  const queryClient = useQueryClient();

  const getList = (params: {
    myTeams: boolean;
    search?: string;
    sortBy?: string;
    sortDesc?: boolean;
    page: number;
    pageSize: number;
  }) =>
    useQuery({
      queryKey: [...TEAMS_QUERY_KEY, params],
      queryFn: () => teamApi.getList(params),
    });

  const getDetail = (id: string) =>
    useQuery({
      queryKey: [...TEAM_DETAIL_QUERY_KEY, id],
      queryFn: () => teamApi.getDetail(id),
    });

  const me = (teamId: string) =>
    useQuery({
      queryKey: [...TEAM_ME_QUERY_KEY, teamId],
      queryFn: () => teamApi.me(teamId),
    });

  const create = useMutation({
    mutationFn: teamApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: TEAMS_QUERY_KEY });
    },
  });

  const update = useMutation({
    mutationFn: (params: {
      id: string;
      data: { name?: string; description?: string };
    }) => teamApi.update(params.id, params.data),
    onSuccess: (_data) => {
      queryClient.invalidateQueries({ queryKey: TEAM_DETAIL_QUERY_KEY });
      queryClient.invalidateQueries({ queryKey: TEAMS_QUERY_KEY });
    },
  });

  const deleteTeam = useMutation({
    mutationFn: teamApi.delete,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: TEAMS_QUERY_KEY });
    },
  });

  const getMembers = (
    id: string,
    search?: string,
    role?: TeamRole,
    page?: number,
    pageSize?: number,
    enabled: boolean = true,
  ) =>
    useQuery({
      queryKey: [...TEAM_MEMBERS_QUERY_KEY, id, search, role, page, pageSize],
      queryFn: () => teamApi.getMembers(id, search, role, page, pageSize),
      enabled: enabled,
    });

  const addMembers = useMutation({
    mutationFn: (params: {
      id: string;
      data: { members: { userId: string; role?: TeamRole }[] };
    }) => teamApi.addMembers(params.id, params.data),
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: [...TEAM_MEMBERS_QUERY_KEY, variables.id],
      });
    },
  });

  const updateMemberRole = useMutation({
    mutationFn: (params: {
      id: string;
      memberId: string;
      data: { role?: TeamRole };
    }) => teamApi.updateMemberRole(params.id, params.memberId, params.data),
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: [...TEAM_MEMBERS_QUERY_KEY, variables.id],
      });
    },
  });

  const removeMember = useMutation({
    mutationFn: (params: { id: string; memberId: string }) =>
      teamApi.removeMember(params.id, params.memberId),
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: [...TEAM_MEMBERS_QUERY_KEY, variables.id],
      });
    },
  });

  const getAvailableMembers = () =>
    useQuery({
      queryKey: TEAM_AVAILABLE_MEMBERS_QUERY_KEY,
      queryFn: teamApi.getAvailableMembers,
    });

  const getAvailableMembersByTeam = (
    id: string,
    search?: string,
    page?: number,
    pageSize?: number,
  ) =>
    useQuery({
      queryKey: [
        ...TEAM_AVAILABLE_MEMBERS_QUERY_KEY,
        id,
        search,
        page,
        pageSize,
      ],
      queryFn: () =>
        teamApi.getAvailableMembersByTeam(id, search, page, pageSize),
    });

  const leave = useMutation({
    mutationFn: teamApi.leave,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: TEAMS_QUERY_KEY });
    },
  });

  return {
    getList,
    getDetail,
    me,
    create,
    update,
    deleteTeam,
    getMembers,
    addMembers,
    updateMemberRole,
    removeMember,
    getAvailableMembers,
    getAvailableMembersByTeam,
    leave,
  };
};
