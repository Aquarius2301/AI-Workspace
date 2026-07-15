export const PROJECT_VISIBILIES = ["public", "private"] as const;

export type ProjectVisibility = (typeof PROJECT_VISIBILIES)[number];

export const PROJECT_ROLES = ["leader", "member"] as const;

export type ProjectRole = (typeof PROJECT_ROLES)[number];

export interface ProjectItem {
  id: string;
  name: string;
  description: string;
  slug: string;
  creatorName: string;
  visibility: string;
  canView: boolean;
  memberCount: number;
  completedTaskCount: number;
  totalTaskCount: number;
}

export interface ProjectDetail {
  id: string;
  teamId: string;
  creatorId: string;
  name: string;
  description?: string;
  visibility: ProjectVisibility;
}

export interface ProjectDetailResult {
  id: string;
  name: string;
  description: string | null;
  slug: string;
  creatorName: string;
  teamName: string;
  visibility: string;
  canView: boolean;
  memberCount: number;
  completedTaskCount: number;
  totalTaskCount: number;
}

export interface MyProjectItem {
  id: string;
  name: string;
  description: string | null;
  slug: string;
  teamName: string;
  teamId: string;
  visibility: string;
  userRole: string;
  memberCount: number;
  completedTaskCount: number;
  totalTaskCount: number;
}

export interface CreateProjectResponse {
  slug: string;
}

export interface CreateProjectRequest {
  teamId: string;
  name: string;
  description?: string;
  visibility: ProjectVisibility;
}

export interface CreateProjectMemberRequest {
  userId: string;
  role: ProjectRole;
}
