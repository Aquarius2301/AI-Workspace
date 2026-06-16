export type TeamRole = "Admin" | "Leader" | "Member";

export interface TeamItem {
  id: string;
  name: string;
  description?: string;
}

export interface TeamDetail {
  id: string;
  name: string;
  description?: string;
}

export interface CreateTeamRequest {
  name: string;
  description?: string;
}

export interface UpdateTeamRequest {
  name?: string;
  description?: string;
}

export interface TeamMemberItem {
  userId: string;
  userName: string;
  role?: TeamRole;
  joinedAt: string;
  email: string;
  lastActiveAt: string;
}

export interface AddTeamMemberRequest {
  members: { userId: string; role?: string }[];
}

export interface UpdateMemberRoleRequest {
  role?: TeamRole;
}

export interface AvailableTeamMemberItem {
  userId: string;
  userName: string;
  email?: string;
}
