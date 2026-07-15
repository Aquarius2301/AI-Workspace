import type {
  CreateProjectRequest,
  CreateProjectResponse,
  MyProjectItem,
  PageResponse,
  PageSize,
  ProjectItem,
  ProjectVisibility,
} from "@/types";
import axiosClient from "./config.api";
import { ENDPOINTS } from "@/constants";

export const projectApi = {
  getListByTeam: (
    id: string,
    search?: string,
    visibility?: ProjectVisibility,
    page?: number,
    pageSize?: PageSize,
  ): Promise<PageResponse<ProjectItem>> => {
    return axiosClient.get(ENDPOINTS.TEAM.GET_PROJECTS(id), {
      params: { search, visibility, page, pageSize },
    });
  },

  getMyList: (
    search?: string,
    visibility?: ProjectVisibility,
    page?: number,
    pageSize?: PageSize,
  ): Promise<PageResponse<MyProjectItem>> => {
    return axiosClient.get(ENDPOINTS.PROJECT.BASE, {
      params: { search, visibility, page, pageSize },
    });
  },

  create: (request: CreateProjectRequest): Promise<CreateProjectResponse> => {
    return axiosClient.post(ENDPOINTS.PROJECT.BASE, request);
  },
};
