import type {
  PageResponse,
  UserItem,
  UpdateProfileRequest,
  ChangePasswordRequest,
} from "@/types";
import axiosClient from "./config.api";

const baseUrl = "/api/users";

export const userApi = {
  getUsers: (
    page: number,
    pageSize: number,
  ): Promise<PageResponse<UserItem>> => {
    return axiosClient.get(`${baseUrl}/users`, {
      params: { page, pageSize },
    });
  },

  updateProfile: (data: UpdateProfileRequest): Promise<string> => {
    return axiosClient.put(`${baseUrl}`, data);
  },

  changePassword: (data: ChangePasswordRequest): Promise<string> => {
    return axiosClient.patch(`${baseUrl}/password`, data);
  },
};
