import { projectApi } from "@/api";
import type { TeamProjectItem } from "@/types";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

export const PROJECT_DETAIL_QUERY_KEY = ["project", "detail"];
export const PROJECTS_BY_TEAM_QUERY_KEY = ["project", "team"];

export const useProject = () => {
  const queryClient = useQueryClient();

  const getDetail = (projectId: string) =>
    useQuery({
      queryKey: [...PROJECT_DETAIL_QUERY_KEY, projectId],
      queryFn: () => projectApi.getDetail(projectId),
    });

  const getByTeam = (
    teamId: string,
    search: string | undefined,
    page: number,
    pageSize: number,
  ) =>
    useQuery({
      queryKey: [...PROJECTS_BY_TEAM_QUERY_KEY, teamId, search, page, pageSize],
      queryFn: () => projectApi.getByTeam(teamId, search, page, pageSize),
      enabled: !!teamId,
    });

  const getAllByTeam = (teamId: string) =>
    useQuery({
      queryKey: [...PROJECTS_BY_TEAM_QUERY_KEY, teamId, "all"],
      queryFn: () => projectApi.getAllByTeam(teamId),
    });

  const create = useMutation({
    mutationFn: (params: {
      teamId: string;
      data: { name: string; description?: string; isPublic?: boolean };
    }) => projectApi.create(params.teamId, params.data),
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: [...PROJECTS_BY_TEAM_QUERY_KEY, variables.teamId],
      });
    },
  });

  const update = useMutation({
    mutationFn: (params: {
      projectId: string;
      data: { name?: string; description?: string; isPublic?: boolean };
    }) => projectApi.update(params.projectId, params.data),
    onSuccess: (data) => {
      const project = data as unknown as TeamProjectItem;
      queryClient.setQueryData(
        [...PROJECT_DETAIL_QUERY_KEY, project.id],
        project,
      );
      queryClient.invalidateQueries({
        queryKey: PROJECTS_BY_TEAM_QUERY_KEY,
      });
    },
  });

  const deleteProject = useMutation({
    mutationFn: projectApi.delete,
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: PROJECTS_BY_TEAM_QUERY_KEY,
      });
    },
  });

  const addMember = useMutation({
    mutationFn: (params: { projectId: string; data: { userId: string } }) =>
      projectApi.addMember(params.projectId, params.data),
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: [...PROJECT_DETAIL_QUERY_KEY, variables.projectId, "members"],
      });
    },
  });

  const removeMember = useMutation({
    mutationFn: (params: { projectId: string; userId: string }) =>
      projectApi.removeMember(params.projectId, params.userId),
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: [...PROJECT_DETAIL_QUERY_KEY, variables.projectId, "members"],
      });
    },
  });

  const getMembers = (projectId: string) =>
    useQuery({
      queryKey: [...PROJECT_DETAIL_QUERY_KEY, projectId, "members"],
      queryFn: () => projectApi.getMembers(projectId),
    });

  const getAvailableMembers = (teamId: string, projectId: string) =>
    useQuery({
      queryKey: [
        ...PROJECT_DETAIL_QUERY_KEY,
        teamId,
        projectId,
        "available-members",
      ],
      queryFn: () => projectApi.getAvailableMembers(teamId, projectId),
    });

  return {
    getDetail,
    getByTeam,
    getAllByTeam,
    create,
    update,
    deleteProject,
    addMember,
    removeMember,
    getMembers,
    getAvailableMembers,
  };
};
