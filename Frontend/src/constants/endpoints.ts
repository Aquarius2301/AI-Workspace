const AUTH = "/auth" as const;
const USERS = "/users" as const;
const PROJECT = "/projects" as const;
const SUMMARY = "/summaries" as const;
const TEAM = "/teams" as const;
const UPLOADS = "/uploads" as const;

export const ENDPOINTS = {
  AUTH: {
    LOGIN: `${AUTH}/login`,
    REGISTER: `${AUTH}/register`,
    REFRESH: `${AUTH}/refresh`,
    LOGOUT: `${AUTH}/logout`,
    SESSION: `${AUTH}/sessions`,
    SESSION_DEVICE: (deviceId: string) => `${AUTH}/sessions/${deviceId}`,
    SESSION_ALL: `${AUTH}/sessions/all`,
    ME: `${AUTH}/me`,
  },
  USERS: {
    BASE: USERS,
    UPDATE_PROFILE: USERS,
  },
  PROJECT: {
    BASE: PROJECT,
    BY_ID: (id: string) => `${PROJECT}/${id}`,
    BY_SLUG: (slug: string) => `${PROJECT}/${slug}`,
    GET_TASKS: (projectId: string) => `${PROJECT}/${projectId}/tasks`,
    GET_MY_TASKS: (projectId: string) => `${PROJECT}/${projectId}/tasks/me`,
    GET_MEMBERS: (projectId: string) => `${PROJECT}/${projectId}/members`,
    AVAILABLE_MEMBERS: (projectId: string) =>
      `${PROJECT}/${projectId}/available-members`,
    ADD_MEMBERS: (projectId: string) => `${PROJECT}/${projectId}/members`,
    UPDATE_MEMBER: (projectId: string, memberId: string) =>
      `${PROJECT}/${projectId}/members/${memberId}`,
    DELETE_MEMBER: (projectId: string, memberId: string) =>
      `${PROJECT}/${projectId}/members/${memberId}`,
    CREATE_TASK: (projectId: string) => `${PROJECT}/${projectId}/tasks`,
    UPDATE_TASK: (projectId: string, taskId: string) =>
      `${PROJECT}/${projectId}/tasks/${taskId}`,
    DELETE_TASK: (projectId: string, taskId: string) =>
      `${PROJECT}/${projectId}/tasks/${taskId}`,
    DELETE_PROJECT: (id: string) => `${PROJECT}/${id}`,
    UPDATE_TASK_STATUS: (projectId: string, taskId: string) =>
      `${PROJECT}/${projectId}/tasks/${taskId}/status`,
    ADMIN_UPDATE_TASK_STATUS: (projectId: string, taskId: string) =>
      `${PROJECT}/${projectId}/tasks/${taskId}/admin-status`,
  },
  TEAM: {
    BASE: TEAM,
    BY_ID: (id: string) => `${TEAM}/${id}`,
    GET_ID: (slug: string) => `${TEAM}/${slug}`,
    GET_PROJECTS: (id: string) => `${TEAM}/${id}/projects`,
    GET_MEMBERS: (id: string) => `${TEAM}/${id}/members`,
    UPDATE_MEMBERS: (id: string, memberId: string) =>
      `${TEAM}/${id}/members/${memberId}`,
  },
  SUMMARY: {
    BASE: SUMMARY,
  },
  UPLOADS: {
    PICTURE: `${UPLOADS}/picture`,
  },
} as const;
