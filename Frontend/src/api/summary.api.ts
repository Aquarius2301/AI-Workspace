import type { SummaryResponse } from "@/types";
import axiosClient from "./config.api";

const baseUrl = "/api/summaries";

export const summaryApi = {
  get: async (): Promise<SummaryResponse> => {
    return await axiosClient.get(`${baseUrl}`);
  },
};
