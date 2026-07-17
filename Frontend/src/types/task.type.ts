export const TASK_STATUS = ["toDo", "doing", "done"] as const;
export type TaskStatus = (typeof TASK_STATUS)[number];

export const TASK_PRIORITY = ["low", "medium", "high"] as const;
export type TaskPriority = (typeof TASK_PRIORITY)[number];

export interface TaskItemResult {
  id: string;
  projectId: string;
  projectName: string;
  title: string;
  description: string | null;
  assignedToId: string | null;
  assignedToName: string | null;
  priority: TaskPriority;
  status: TaskStatus;
  createdAt: string;
  dueDate: string | null;
}

export interface CreateTaskRequest {
  title: string;
  description?: string;
  assignedToId?: string;
  priority: TaskPriority;
  dueDate?: string;
}
