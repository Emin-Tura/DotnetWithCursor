import axios from 'axios';

const api = axios.create({
  baseURL: 'https://localhost:7000',
  headers: {
    'Content-Type': 'application/json',
  },
  withCredentials: true
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export interface RegisterData {
  username: string;
  email: string;
  password: string;
  firstName: string;
  lastName: string;
}

export interface LoginData {
  username: string;
  password: string;
}

export interface User {
  id: number;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  createdAt: string;
}

export interface AuthResponse {
  token: string;
  user: User;
}

export const authApi = {
  register: (data: RegisterData) => api.post<AuthResponse>('/api/Auth/register', data),
  login: (data: LoginData) => api.post<AuthResponse>('/api/Auth/login', data),
  getCurrentUser: () => api.get<User>('/api/Auth/me'),
};

export default api; 