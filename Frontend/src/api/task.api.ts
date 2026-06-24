import type {
  TaskDetail,
  TaskItemResponse,
  MyTaskItemResponse,
  CreateTaskRequest,
  UpdateTaskRequest,
  UpdateTaskStatusRequest,
  AssignTaskRequest,
  PageResponse,
} from "@/types";
import axiosClient from "./config.api";

const baseUrl = "/api/tasks";
const projectsBaseUrl = "/api/projects";
const teamsBaseUrl = "/api/teams";

export const taskApi = {
  getDetail: (taskId: string): Promise<TaskDetail> => {
    return axiosClient.get(`${baseUrl}/${taskId}`);
  },

  getByProject: (
    projectId: string,
    params: {
      status?: string;
      memberId?: string;
      page: number;
      pageSize: number;
    },
  ): Promise<PageResponse<TaskItemResponse>> => {
    return axiosClient.get(`${projectsBaseUrl}/${projectId}/tasks`, { params });
  },

  getMyTasks: (teamId: string): Promise<MyTaskItemResponse[]> => {
    return axiosClient.get(`${teamsBaseUrl}/${teamId}/tasks/me`);
  },

  create: (projectId: string, data: CreateTaskRequest): Promise<string> => {
    return axiosClient.post(`${projectsBaseUrl}/${projectId}/tasks`, data);
  },

  update: (taskId: string, data: UpdateTaskRequest): Promise<string> => {
    return axiosClient.put(`${baseUrl}/${taskId}`, data);
  },

  updateStatus: (
    taskId: string,
    data: UpdateTaskStatusRequest,
  ): Promise<string> => {
    return axiosClient.patch(`${baseUrl}/${taskId}/status`, data);
  },

  assign: (taskId: string, data: AssignTaskRequest): Promise<string> => {
    return axiosClient.patch(`${baseUrl}/${taskId}/assign`, data);
  },

  delete: (taskId: string): Promise<string> => {
    return axiosClient.delete(`${baseUrl}/${taskId}`);
  },
};
