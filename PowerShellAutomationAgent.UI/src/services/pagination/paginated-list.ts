export interface PaginatedList<T> {
  page: number;
  pageSize: number;
  hasNext: boolean;
  hasPrevious: boolean;
  data: T[];
}