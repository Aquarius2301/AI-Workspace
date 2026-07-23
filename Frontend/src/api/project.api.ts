import type {
  CreateProjectRequest,
  CreateProjectResponse,
  CreateTaskRequest,
  MyProjectItem,
  PageResponse,
  PageSize,
  ProjectDetailResult,
  ProjectItem,
  ProjectMemberItem,
  ProjectRole,
  ProjectVisibility,
  TaskItemResult,
  TaskPriority,
  TaskStatus,
  UpdateProjectRequest,
  UpdateTaskRequest,
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

  getTasks: (
    projectId: string,
    search?: string,
    priority?: TaskPriority,
  ): Promise<TaskItemResult[]> => {
    return axiosClient.get(ENDPOINTS.PROJECT.GET_TASKS(projectId), {
      params: { search, priority },
    });
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

  getMembers: (
    projectId: string,
    search?: string,
    role?: ProjectRole,
    page?: number,
    pageSize?: PageSize,
  ): Promise<PageResponse<ProjectMemberItem>> => {
    return axiosClient.get(ENDPOINTS.PROJECT.GET_MEMBERS(projectId), {
      params: { search, role, page, pageSize },
    });
  },

  create: (request: CreateProjectRequest): Promise<CreateProjectResponse> => {
    return axiosClient.post(ENDPOINTS.PROJECT.BASE, request);
  },

  createTask: (
    projectId: string,
    data: CreateTaskRequest,
  ): Promise<TaskItemResult> => {
    return axiosClient.post(ENDPOINTS.PROJECT.CREATE_TASK(projectId), data);
  },

  updateTask: (
    projectId: string,
    taskId: string,
    data: UpdateTaskRequest,
  ): Promise<TaskItemResult> => {
    return axiosClient.put(
      ENDPOINTS.PROJECT.UPDATE_TASK(projectId, taskId),
      data,
    );
  },

  deleteTask: (projectId: string, taskId: string): Promise<void> => {
    return axiosClient.delete(ENDPOINTS.PROJECT.DELETE_TASK(projectId, taskId));
  },

  deleteProject: (id: string): Promise<void> => {
    return axiosClient.delete(ENDPOINTS.PROJECT.DELETE_PROJECT(id));
  },

  update: (id: string, data: UpdateProjectRequest): Promise<void> => {
    return axiosClient.put(ENDPOINTS.PROJECT.BY_ID(id), data);
  },

  updateTaskStatus: (
    projectId: string,
    taskId: string,
    status: TaskStatus,
  ): Promise<TaskItemResult> => {
    return axiosClient.patch(
      ENDPOINTS.PROJECT.UPDATE_TASK_STATUS(projectId, taskId),
      { status },
    );
  },

  adminUpdateTaskStatus: (
    projectId: string,
    taskId: string,
    status: TaskStatus,
  ): Promise<TaskItemResult> => {
    return axiosClient.patch(
      ENDPOINTS.PROJECT.ADMIN_UPDATE_TASK_STATUS(projectId, taskId),
      { status },
    );
  },
};
