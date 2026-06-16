export type ProjectVisibility = "Public" | "Private";

export interface ProjectDetail {
  id: string;
  teamId: string;
  creatorId: string;
  name: string;
  description?: string;
  visibility: ProjectVisibility;
}

export interface TeamProjectItem {
  id: string;
  creatorId: string;
  name: string;
  description?: string;
  visibility: ProjectVisibility;
}

export interface CreateProjectRequest {
  name: string;
  description?: string;
  isPublic?: boolean;
}

export interface UpdateProjectRequest {
  name?: string;
  description?: string;
  isPublic?: boolean;
}

export interface AddMemberRequest {
  userId: string;
}

export interface ProjectMemberItem {
  userId: string;
  userName: string;
  email?: string;
  joinedAt: string;
}

export interface AvailableMemberItem {
  userId: string;
  userName: string;
  email?: string;
}
