export interface Pagination<T> {
  items: T[];
  pageCount: number;
  totalCount: number;
}

export interface QueryStringOptions {
  sortBy?: number;
  page?: number;
  size?: number;
  column?: number | string;
  query?: string;
  userId?: string;
}
