import axiosClient from "./config.api";

const baseUrl = "/api/upload";

export const uploadApi = {
  uploadPicture: (file: File): Promise<string> => {
    const formData = new FormData();
    formData.append("file", file);
    return axiosClient.post(`${baseUrl}/picture`, formData, {
      headers: { "Content-Type": undefined },
    });
  },
};
