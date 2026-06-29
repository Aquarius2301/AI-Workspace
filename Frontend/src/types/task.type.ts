export type TaskStatus = "Open" | "InProgress" | "Done" | "Blocked";

export interface TaskDetail {
  id: string;
  projectId: string;
  title: string;
  description?: string;
  assignedToId?: string;
  assignedToName?: string;
  priority: number;
  status: TaskStatus;
  createdAt: string;
  dueDate?: string;
}

export interface TaskItemResponse {
  id: string;
  title: string;
  description?: string;
  assignedToId?: string;
  assignedToName?: string;
  priority: number;
  status: TaskStatus;
  createdAt: string;
  dueDate?: string;
}

export interface MyTaskItemResponse {
  id: string;
  projectId: string;
  projectName: string;
  title: string;
  description?: string;
  priority: number;
  status: TaskStatus;
  createdAt: string;
  dueDate?: string;
}

export interface CreateTaskRequest {
  title: string;
  description?: string;
  assignedToId?: string;
  priority: number;
  dueDate?: string;
}

export interface UpdateTaskRequest {
  title?: string;
  description?: string;
  dueDate?: string;
}

export interface UpdateTaskStatusRequest {
  status: TaskStatus;
}

export interface AssignTaskRequest {
  assignedToId: string;
}
