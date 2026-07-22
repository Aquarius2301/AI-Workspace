import { projectApi } from "@/api/project.api";
import type {
  CreateTaskRequest,
  PageSize,
  TaskItemResult,
  TaskPriority,
  TaskStatus,
  UpdateTaskRequest,
} from "@/types";
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

export const useUpdateTask = (projectId: string) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      taskId,
      data,
    }: {
      taskId: string;
      data: UpdateTaskRequest;
    }) => projectApi.updateTask(projectId, taskId, data),
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

export const useDeleteTask = (projectId: string) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (taskId: string) => projectApi.deleteTask(projectId, taskId),
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
    onMutate: async (params) => {
      // Snapshot previous task lists for rollback
      const queryKey = [...TASKS_LIST_QUERY_KEY, projectId];
      await queryClient.cancelQueries({ queryKey });
      const previousTasks =
        queryClient.getQueryData<TaskItemResult[]>(queryKey);

      // Optimistically update the task status in the cache
      queryClient.setQueryData<TaskItemResult[]>(queryKey, (old) =>
        old?.map((t) =>
          t.id === params.taskId ? { ...t, status: params.status } : t,
        ),
      );

      return { previousTasks, queryKey };
    },
    onError: (_err, _params, context) => {
      // Rollback on error
      if (context?.previousTasks) {
        queryClient.setQueryData(context.queryKey, context.previousTasks);
      }
    },
    onSettled: (_data, _error, _params, context) => {
      queryClient.invalidateQueries({ queryKey: context?.queryKey });
      queryClient.invalidateQueries({
        queryKey: [...TASKS_MY_LIST_QUERY_KEY, projectId],
      });
    },
  });
};

export const useAdminUpdateTaskStatus = (projectId: string) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (params: { taskId: string; status: TaskStatus }) =>
      projectApi.adminUpdateTaskStatus(projectId, params.taskId, params.status),
    onMutate: async (params) => {
      const queryKey = [...TASKS_LIST_QUERY_KEY, projectId];
      await queryClient.cancelQueries({ queryKey });
      const previousTasks =
        queryClient.getQueryData<TaskItemResult[]>(queryKey);

      queryClient.setQueryData<TaskItemResult[]>(queryKey, (old) =>
        old?.map((t) =>
          t.id === params.taskId ? { ...t, status: params.status } : t,
        ),
      );

      return { previousTasks, queryKey };
    },
    onError: (_err, _params, context) => {
      if (context?.previousTasks) {
        queryClient.setQueryData(context.queryKey, context.previousTasks);
      }
    },
    onSettled: (_data, _error, _params, context) => {
      queryClient.invalidateQueries({ queryKey: context?.queryKey });
      queryClient.invalidateQueries({
        queryKey: [...TASKS_MY_LIST_QUERY_KEY, projectId],
      });
    },
  });
};
