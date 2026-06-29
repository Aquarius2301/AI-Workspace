export interface SummaryResponse {
  totalTeams: number;
  totalProjects: number;
  myTasks: TaskSummary;

  recentTasks: TaskItemSummary[];

  recentComments: CommentSummary[];

  teamSummaries: TeamSummary[];
}

export interface TaskSummary {
  total: number;

  open: number;

  inProgress: number;

  done: number;

  blocked: number;

  overdue: number;
}

export interface TaskItemSummary {
  id: string;

  title: string;

  projectName: string;

  status: string;

  createdAt: string;

  dueDate: string | null;
}

export interface CommentSummary {
  id: string;

  content: string;

  projectName: string | null;

  createdAt: string;
}

export interface TeamMemberInfo {
  teamId: string;

  role: string;
}

export interface TeamSummary {
  teamId: string;

  teamName: string;

  myRole: string;

  memberCount: number;

  projectCount: number;
}
