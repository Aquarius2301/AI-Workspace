import type {
  TeamItem,
  TeamDetail,
  CreateTeamRequest,
  UpdateTeamRequest,
  TeamMemberItem,
  AddTeamMemberRequest,
  UpdateMemberRoleRequest,
  AvailableTeamMemberItem,
  PageResponse,
  TeamRole,
  PageSize,
  TeamProjectItem,
  ProjectVisibility,
} from "@/types";
import axiosClient from "./config.api";

const baseUrl = "/api/teams";

export const teamApi = {
  getList: (
    myTeams: boolean,
    search?: string,
    page?: number,
    pageSize?: PageSize,
  ): Promise<PageResponse<TeamItem>> => {
    return axiosClient.get(`${baseUrl}`, {
      params: { search, page, pageSize, myTeams },
    });
  },

  me: (teamId: string): Promise<TeamMemberItem> => {
    return axiosClient.get(`${baseUrl}/${teamId}/me`);
  },

  getDetail: (id: string): Promise<TeamDetail> => {
    return axiosClient.get(`${baseUrl}/${id}`);
  },

  create: (data: CreateTeamRequest): Promise<string> => {
    return axiosClient.post(`${baseUrl}`, data);
  },

  update: (id: string, data: UpdateTeamRequest): Promise<string> => {
    return axiosClient.put(`${baseUrl}/${id}`, data);
  },

  delete: (id: string): Promise<string> => {
    return axiosClient.delete(`${baseUrl}/${id}`);
  },

  getMembers: (
    id: string,
    search?: string,
    role?: TeamRole,
    page?: number,
    pageSize?: number,
  ): Promise<PageResponse<TeamMemberItem>> => {
    return axiosClient.get(`${baseUrl}/${id}/members`, {
      params: { search, role, page, pageSize },
    });
  },

  addMembers: (
    id: string,
    data: AddTeamMemberRequest,
  ): Promise<TeamMemberItem[]> => {
    return axiosClient.post(`${baseUrl}/${id}/members`, data);
  },

  updateMemberRole: (
    id: string,
    memberId: string,
    data: UpdateMemberRoleRequest,
  ): Promise<TeamMemberItem> => {
    return axiosClient.put(`${baseUrl}/${id}/members/${memberId}`, data);
  },

  removeMember: (id: string, memberId: string): Promise<string> => {
    return axiosClient.delete(`${baseUrl}/${id}/members/${memberId}`);
  },

  getAvailableMembers: (
    id: string,
    pagination: { search?: string; page?: number; pageSize?: number },
  ): Promise<PageResponse<AvailableTeamMemberItem>> => {
    return axiosClient.get(`${baseUrl}/${id}/available-members`, {
      params: pagination,
    });
  },

  leave: (id: string): Promise<string> => {
    return axiosClient.post(`${baseUrl}/${id}/leave`);
  },

  getProjects: (
    id: string,
    search?: string,
    visibility?: ProjectVisibility,
    page?: number,
    pageSize?: PageSize,
  ): Promise<PageResponse<TeamProjectItem>> => {
    return axiosClient.get(`${baseUrl}/${id}/projects`, {
      params: { search, visibility, page, pageSize },
    });
  },
  transferAdmin: (id: string, targetUserId: string): Promise<string> => {
    return axiosClient.post(`${baseUrl}/${id}/transfer-admin/${targetUserId}`);
  },
};
