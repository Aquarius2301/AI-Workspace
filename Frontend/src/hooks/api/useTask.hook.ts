import { projectApi } from "@/api/project.api";
import type { CreateTaskRequest, PageSize, TaskPriority, TaskStatus } from "@/types";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

export const TASK_QUERY_KEY = ["tasks"] as const;
export const TASKS_LIST_QUERY_KEY = [...TASK_QUERY_KEY, "list"] as const;
export const TASKS_MY_LIST_QUERY_KEY = [...TASK_QUERY_KEY, "my-list"] as const;

export const useTasksByProject = (
  projectId: string,
  search?: string,
  priority?: TaskPriority,
  enabled: boolean = true,
) =>
  useQuery({
    queryKey: [...TASKS_LIST_QUERY_KEY, projectId, { search, priority }],
    queryFn: () => projectApi.getTasks(projectId, search, priority),
    enabled: !!projectId && enabled,
  });

export const useMyTasksByProject = (
  projectId: string,
  search?: string,
  status?: TaskStatus,
  priority?: TaskPriority,
  page?: number,
  pageSize?: PageSize,
  enabled: boolean = true,
) =>
  useQuery({
    queryKey: [
      ...TASKS_MY_LIST_QUERY_KEY,
      projectId,
      { search, status, priority, page, pageSize },
    ],
    queryFn: () =>
      projectApi.getMyTasks(
        projectId,
        search,
        status,
        priority,
        page,
        pageSize,
      ),
    enabled: !!projectId && enabled,
  });

export const useCreateTask = (projectId: string) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: CreateTaskRequest) =>
      projectApi.createTask(projectId, data),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [...TASKS_LIST_QUERY_KEY, projectId],
      });
      queryClient.invalidateQueries({
        queryKey: [...TASKS_MY_LIST_QUERY_KEY, projectId],
      });
    },
  });
};

export const useUpdateTaskStatus = (projectId: string) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (params: { taskId: string; status: TaskStatus }) =>
      projectApi.updateTaskStatus(projectId, params.taskId, params.status),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [...TASKS_LIST_QUERY_KEY, projectId],
      });
      queryClient.invalidateQueries({
        queryKey: [...TASKS_MY_LIST_QUERY_KEY, projectId],
      });
    },
  });
};
