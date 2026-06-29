import axiosClient from "./config.api";
import type { CreateProjectRequest } from "@/types";

const baseUrl = "/api/projects";

export const projectApi = {
  create: (request: CreateProjectRequest): Promise<string> => {
    return axiosClient.post(`${baseUrl}`, request);
  },
};
