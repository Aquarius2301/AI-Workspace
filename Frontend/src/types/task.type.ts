export const TASK_STATUS = ["toDo", "doing", "done"] as const;
export type TaskStatus = (typeof TASK_STATUS)[number];

export const TASK_PRIORITY = ["low", "medium", "high"] as const;
export type TaskPriority = (typeof TASK_PRIORITY)[number];
