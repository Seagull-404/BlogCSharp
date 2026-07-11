import request from './index'

export interface LoginDto
{
    userName: string,
    passWord: string
}

export interface RegisterDto
{
    userName: string,
    email: string,
    passWord: string,
    
}

export interface UserDto
{
    id: number,
    userName: string,
    email: string,
    role: string,
    token: string
}

export const login = (data: LoginDto) => request.post<UserDto>('/auth/login', data);

export const register = (data: RegisterDto) => request.post<UserDto>('/auth/register', data);

export const getCurrentUser = () => request.get<UserDto>('/auth/me');