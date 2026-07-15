import type {
  CreateProjectRequest,
  CreateProjectResponse,
  MyProjectItem,
  PageResponse,
  PageSize,
  ProjectDetailResult,
  ProjectItem,
  ProjectVisibility,
  TaskItemResult,
  TaskStatus,
  TaskPriority,
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

  getBySlug: (slug: string): Promise<{ id: string }> => {
    return axiosClient.get(ENDPOINTS.PROJECT.BY_SLUG(slug));
  },

  getById: (id: string): Promise<ProjectDetailResult> => {
    return axiosClient.get(ENDPOINTS.PROJECT.BY_ID(id));
  },

  getMyTasks: (
    projectId: string,
    search?: string,
    status?: TaskStatus,
    priority?: TaskPriority,
    page?: number,
    pageSize?: PageSize,
  ): Promise<PageResponse<TaskItemResult>> => {
    return axiosClient.get(ENDPOINTS.PROJECT.GET_MY_TASKS(projectId), {
      params: { search, status, priority, page, pageSize },
    });
  },

  create: (request: CreateProjectRequest): Promise<CreateProjectResponse> => {
    return axiosClient.post(ENDPOINTS.PROJECT.BASE, request);
  },
};
