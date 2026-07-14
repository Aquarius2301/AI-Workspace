export interface PageResponse<T> {
  page: number;
  pageSize: number;
  total: number;
  items: T[];
}

export const PAGE_SIZES = [10, 20, 50, 100] as const;

export type PageSize = (typeof PAGE_SIZES)[number];

export interface ValidationError {
  field: string;
  message: string;
}

export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T | null;
}

export interface ApiErrorResponse extends ApiResponse<ValidationError[]> {}
