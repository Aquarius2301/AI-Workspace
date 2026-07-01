import type {
  PageResponse,
  UserItem,
  UpdateProfileRequest,
  ChangePasswordRequest,
  UploadAvatarResponse,
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

  uploadAvatar: (file: File): Promise<UploadAvatarResponse> => {
    const formData = new FormData();
    formData.append("file", file);
    return axiosClient.post(`${baseUrl}/avatar`, formData, {
      headers: { "Content-Type": undefined },
    });
  },

  changePassword: (data: ChangePasswordRequest): Promise<string> => {
    return axiosClient.patch(`${baseUrl}/password`, data);
  },
};
