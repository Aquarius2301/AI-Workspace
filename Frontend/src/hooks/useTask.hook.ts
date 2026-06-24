import { taskApi } from "@/api";
import type { UpdateTaskStatusRequest } from "@/types";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

export const TASK_DETAIL_QUERY_KEY = ["tasks", "detail"];
export const TASKS_BY_PROJECT_QUERY_KEY = ["tasks", "project"];
export const MY_TASKS_QUERY_KEY = ["tasks", "my"];

export const useTask = () => {
  const queryClient = useQueryClient();

  const getDetail = (taskId: string, enabled: boolean = true) =>
    useQuery({
      queryKey: [...TASK_DETAIL_QUERY_KEY, taskId],
      queryFn: () => taskApi.getDetail(taskId),
      enabled: !!taskId && enabled,
    });

  const getByProject = (
    projectId: string,
    params: {
      status?: string;
      memberId?: string;
      page: number;
      pageSize: number;
    },
    enabled: boolean = true,
  ) =>
    useQuery({
      queryKey: [...TASKS_BY_PROJECT_QUERY_KEY, projectId, params],
      queryFn: () => taskApi.getByProject(projectId, params),
      enabled: !!projectId && enabled,
    });

  const getMyTasks = (teamId: string, enabled: boolean = true) =>
    useQuery({
      queryKey: [...MY_TASKS_QUERY_KEY, teamId],
      queryFn: () => taskApi.getMyTasks(teamId),
      enabled: !!teamId && enabled,
    });

  const create = useMutation({
    mutationFn: (params: {
      projectId: string;
      data: {
        title: string;
        description?: string;
        assignedToId?: string;
        priority: number;
        dueDate?: string;
      };
    }) => taskApi.create(params.projectId, params.data),
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: [...TASKS_BY_PROJECT_QUERY_KEY, variables.projectId],
      });
    },
  });

  const update = useMutation({
    mutationFn: (params: {
      taskId: string;
      data: { title?: string; description?: string; dueDate?: string };
    }) => taskApi.update(params.taskId, params.data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: TASK_DETAIL_QUERY_KEY });
    },
  });

  const updateStatus = useMutation({
    mutationFn: (params: { taskId: string; data: UpdateTaskStatusRequest }) =>
      taskApi.updateStatus(params.taskId, params.data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: TASK_DETAIL_QUERY_KEY });
    },
  });

  const assign = useMutation({
    mutationFn: (params: { taskId: string; data: { assignedToId: string } }) =>
      taskApi.assign(params.taskId, params.data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: TASK_DETAIL_QUERY_KEY });
    },
  });

  const deleteTask = useMutation({
    mutationFn: taskApi.delete,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: TASKS_BY_PROJECT_QUERY_KEY });
    },
  });

  return {
    getDetail,
    getByProject,
    getMyTasks,
    create,
    update,
    updateStatus,
    assign,
    deleteTask,
  };
};
