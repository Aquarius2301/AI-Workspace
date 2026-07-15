import type { TaskStatus } from "./task.type";
import type { TeamRole } from "./team.type";

export interface SummaryResponse {
  totalTeams: number;
  totalProjects: number;
  myTasks: TaskSummary;

  recentTasks: TaskItemSummary[];

  teamSummaries: TeamSummary[];
}

export interface TaskSummary {
  total: number;

  toDo: number;

  doing: number;

  done: number;

  overdue: number;
}

export interface TaskItemSummary {
  id: string;

  title: string;

  projectId: string;

  projectName: string;

  projectSlug: string;

  status: TaskStatus | "overdue";

  createdAt: string;

  dueDate: string | null;
}

export interface TeamMemberInfo {
  teamId: string;

  role: string;
}

export interface TeamSummary {
  teamId: string;

  slug: string;

  teamName: string;

  myRole: TeamRole;

  memberCount: number;

  projectCount: number;
}
