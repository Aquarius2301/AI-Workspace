import type {
  AddTeamMemberRequest,
  AvailableTeamMemberItem,
  CreateTeamRequest,
  PageResponse,
  PageSize,
  TeamDetail,
  TeamItem,
  TeamMemberItem,
  TeamRole,
  UpdateMemberRoleRequest,
  UpdateTeamRequest,
} from "@/types";
import axiosClient from "./config.api";
import { ENDPOINTS } from "@/constants";

export const teamApi = {
  getList: (
    search?: string,
    page?: number,
    pageSize?: PageSize,
  ): Promise<PageResponse<TeamItem>> => {
    return axiosClient.get(ENDPOINTS.TEAM.BASE, {
      params: { search, page, pageSize },
    });
  },

  create: (request: CreateTeamRequest): Promise<void> => {
    return axiosClient.post(ENDPOINTS.TEAM.BASE, request);
  },

  update: (id: string, data: UpdateTeamRequest): Promise<void> => {
    return axiosClient.put(ENDPOINTS.TEAM.BY_ID(id), data);
  },

  getId: (slug: string): Promise<{ id: string }> => {
    return axiosClient.get(ENDPOINTS.TEAM.GET_ID(slug));
  },

  getDetail: (id: string): Promise<TeamDetail> => {
    return axiosClient.get(ENDPOINTS.TEAM.BY_ID(id));
  },

  getMembers: (
    id: string,
    search?: string,
    role?: TeamRole,
    page?: number,
    pageSize?: number,
  ): Promise<PageResponse<TeamMemberItem>> => {
    return axiosClient.get(ENDPOINTS.TEAM.GET_MEMBERS(id), {
      params: { search, role, page, pageSize },
    });
  },

  getAvailableMembers: (
    id: string,
    search?: string,
    page?: number,
    pageSize?: number,
  ): Promise<PageResponse<AvailableTeamMemberItem>> => {
    return axiosClient.get(`${ENDPOINTS.TEAM.BY_ID(id)}/available-members`, {
      params: { search, page, pageSize },
    });
  },

  addMembers: (id: string, data: AddTeamMemberRequest): Promise<void> => {
    return axiosClient.post(`${ENDPOINTS.TEAM.BY_ID(id)}/members`, data);
  },

  updateMemberRole: (
    id: string,
    memberId: string,
    data: UpdateMemberRoleRequest,
  ): Promise<TeamMemberItem> => {
    return axiosClient.put(ENDPOINTS.TEAM.UPDATE_MEMBERS(id, memberId), data);
  },
};
