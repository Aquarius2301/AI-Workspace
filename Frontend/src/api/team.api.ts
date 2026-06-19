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
} from "@/types";
import axiosClient from "./config.api";

const baseUrl = "/api/teams";

export const teamApi = {
  getList: (params: {
    myTeams: boolean;
    search?: string;
    sortBy?: string;
    sortDesc?: boolean;
    page: number;
    pageSize: number;
  }): Promise<PageResponse<TeamItem>> => {
    return axiosClient.get(`${baseUrl}`, { params });
  },

  me: (teamId: string): Promise<TeamMemberItem> => {
    return axiosClient.get(`${baseUrl}/${teamId}/me`);
  },

  getDetail: (id: string): Promise<TeamDetail> => {
    return axiosClient.get(`${baseUrl}/${id}`);
  },

  create: (data: CreateTeamRequest): Promise<TeamDetail> => {
    return axiosClient.post(`${baseUrl}`, data);
  },

  update: (id: string, data: UpdateTeamRequest): Promise<TeamDetail> => {
    return axiosClient.put(`${baseUrl}/${id}`, data);
  },

  delete: (id: string): Promise<void> => {
    return axiosClient.delete(`${baseUrl}/${id}`);
  },

  getMembers: (id: string): Promise<TeamMemberItem[]> => {
    return axiosClient.get(`${baseUrl}/${id}/members`);
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

  removeMember: (id: string, memberId: string): Promise<void> => {
    return axiosClient.delete(`${baseUrl}/${id}/members/${memberId}`);
  },

  getAvailableMembers: (): Promise<AvailableTeamMemberItem[]> => {
    return axiosClient.get(`${baseUrl}/available-members`);
  },

  getAvailableMembersByTeam: (
    id: string,
    search?: string,
    page?: number,
    pageSize?: number,
  ): Promise<AvailableTeamMemberItem[]> => {
    return axiosClient.get(`${baseUrl}/${id}/available-members`, {
      params: {
        search,
        page,
        pageSize,
      },
    });
  },

  leave: (id: string): Promise<void> => {
    return axiosClient.post(`${baseUrl}/${id}/leave`);
  },
};
