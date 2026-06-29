export interface PageResponse<T> {
  page: number;
  pageSize: number;
  total: number;
  items: T[];
}

export type PageSize = 5 | 10 | 20 | 50;

export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
}
