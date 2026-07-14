export const TEAM_ROLES = ["admin", "coAdmin", "member"] as const;
export type TeamRole = (typeof TEAM_ROLES)[number];

export interface TeamItem {
  id: string;
  name: string;
  description?: string;
  slug: string;
  memberCount: number;
  currentUserRole: TeamRole;
}

export interface TeamDetail {
  id: string;
  name: string;
  description?: string;
  role: TeamRole;
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
  avatarUrl: string;
  role: TeamRole;
  joinedAt: string;
  email: string;
  lastActiveAt: string;
}

export interface UpdateMemberRoleRequest {
  role?: TeamRole;
}

export interface AvailableTeamMemberItem {
  id: string;
  name: string;
  email: string;
  avatarUrl: string;
}

export interface AddTeamMemberRequest {
  members: { userId: string; role?: TeamRole }[];
}
