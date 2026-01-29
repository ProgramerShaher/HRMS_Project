export interface ApiResponse<T> {
  data: T;
  succeeded: boolean;
  message: string;
  errors: string[] | null;
  statusCode: number;
}
