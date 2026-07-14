import type { SummaryResponse } from "@/types/summary.type";
import axiosClient from "./config.api";
import { ENDPOINTS } from "@/constants";

export const summaryApi = {
  get: (): Promise<SummaryResponse> => {
    return axiosClient.get(ENDPOINTS.SUMMARY.BASE);
  },
};
