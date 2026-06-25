import type {
  PageResponse,
  UserItem,
  UpdateProfileRequest,
  ChangePasswordRequest,
} from "@/types";
import axiosClient from "./config.api";

const baseUrl = "/api/users";

export const userApi = {
  getUsers: (params: {
    page: number;
    pageSize: number;
  }): Promise<PageResponse<UserItem>> => {
    return axiosClient.get(`${baseUrl}/users`, {
      params,
    });
  },

  updateProfile: (data: UpdateProfileRequest): Promise<string> => {
    return axiosClient.put(`${baseUrl}/users`, data);
  },

  changePassword: (
    id: string,
    data: ChangePasswordRequest,
  ): Promise<string> => {
    return axiosClient.patch(`${baseUrl}/${id}/password`, data);
  },
};
