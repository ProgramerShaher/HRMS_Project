export interface LoginRequest {
  userName: string;
  password: string;
  rememberMe: boolean;
}

export interface RegisterRequest {
  userName: string;
  email: string;
  password: string;
  confirmPassword: string;
  fullNameAr?: string | null;
  fullNameEn?: string | null;
  phoneNumber?: string | null;
  employeeId?: number | null;
}

export interface AuthResponse {
  userId: number;
  userName: string;
  email: string;
  fullName?: string | null;
  token: string;
  refreshToken?: string | null;
  tokenExpiration: string; // DateTime ISO string
  roles: string[];
  employeeId?: number | null;
}
