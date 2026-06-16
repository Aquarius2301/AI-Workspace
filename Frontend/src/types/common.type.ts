export interface PageResponse<T> {
  page: number;
  pageSize: number;
  total: number;
  items: T[];
}
