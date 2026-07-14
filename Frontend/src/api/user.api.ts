import { ENDPOINTS } from "@/constants";
import type { UpdateProfileRequest, UploadPictureResult } from "@/types";
import axiosClient from "./config.api";

export const userApi = {
  updateProfile: (data: UpdateProfileRequest): Promise<void> => {
    return axiosClient.put(ENDPOINTS.USERS.UPDATE_PROFILE, data);
  },

  uploadPicture: (file: File): Promise<UploadPictureResult> => {
    const formData = new FormData();
    formData.append("file", file);
    return axiosClient.post(ENDPOINTS.UPLOADS.PICTURE, formData, {
      headers: { "Content-Type": "multipart/form-data" },
    });
  },
};
