import { uploadApi } from "@/api/upload.api";
import { useMutation } from "@tanstack/react-query";

export const useUpload = () => {
  const uploadPicture = useMutation({
    mutationFn: uploadApi.uploadPicture,
  });

  return { uploadPicture };
};
