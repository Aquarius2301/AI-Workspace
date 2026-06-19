import type {
  ProjectDetail,
  TeamProjectItem,
  CreateProjectRequest,
  UpdateProjectRequest,
  AddMemberRequest,
  ProjectMemberItem,
  AvailableMemberItem,
  PageResponse,
} from "@/types";
import axiosClient from "./config.api";

const baseUrl = "/api/projects";

export const projectApi = {
  getDetail: (projectId: string): Promise<ProjectDetail> => {
    return axiosClient.get(`${baseUrl}/${projectId}`);
  },

  getByTeam: (
    teamId: string,
    search: string | undefined,
    page: number,
    pageSize: number,
  ): Promise<PageResponse<TeamProjectItem>> => {
    return axiosClient.get(`${baseUrl}/team/${teamId}`, {
      params: { search, page, pageSize },
    });
  },

  getAllByTeam: (teamId: string): Promise<TeamProjectItem[]> => {
    return axiosClient.get(`${baseUrl}/team/${teamId}/all`);
  },

  create: (
    teamId: string,
    data: CreateProjectRequest,
  ): Promise<TeamProjectItem> => {
    return axiosClient.post(`${baseUrl}/team/${teamId}`, data);
  },

  update: (
    projectId: string,
    data: UpdateProjectRequest,
  ): Promise<TeamProjectItem> => {
    return axiosClient.put(`${baseUrl}/${projectId}`, data);
  },

  delete: (projectId: string): Promise<void> => {
    return axiosClient.delete(`${baseUrl}/${projectId}`);
  },

  addMember: (projectId: string, data: AddMemberRequest): Promise<void> => {
    return axiosClient.post(`${baseUrl}/${projectId}/member`, data);
  },

  removeMember: (projectId: string, userId: string): Promise<void> => {
    return axiosClient.delete(`${baseUrl}/${projectId}/member/${userId}`);
  },

  getMembers: (projectId: string): Promise<ProjectMemberItem[]> => {
    return axiosClient.get(`${baseUrl}/${projectId}/members`);
  },

  getAvailableMembers: (
    teamId: string,
    projectId: string,
  ): Promise<AvailableMemberItem[]> => {
    return axiosClient.get(`${baseUrl}/team/${teamId}/available-members`, {
      params: { projectId },
    });
  },
};
