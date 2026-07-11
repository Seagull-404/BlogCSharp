import axios from 'axios'

const instance = axios.create({
    baseURL: '/api',
    timeout: 10000
})

instance.interceptors.request.use(
    config =>
    {
        const token = localStorage.getItem('token');
        if(token)
        {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    })

instance.interceptors.response.use(
    response => response.data,
    error =>
    {
        if(error.response?.status === 401)
        {
            localStorage.removeItem('token');
           window.location.href = '/login';
        }
        return Promise.reject(error);
    })

type ApiResponse<T = any> = T

const request = {
    get: async <T>(url: string, config?: any): Promise<ApiResponse<T>> => {
        const response = await instance.get<T>(url, config)
        return response as ApiResponse<T>
    },
    post: async <T>(url: string, data?: any, config?: any): Promise<ApiResponse<T>> => {
        const response = await instance.post<T>(url, data, config)
        return response as ApiResponse<T>
    },
    put: async <T>(url: string, data?: any, config?: any): Promise<ApiResponse<T>> => {
        const response = await instance.put<T>(url, data, config)
        return response as ApiResponse<T>
    },
    delete: async <T>(url: string, config?: any): Promise<ApiResponse<T>> => {
        const response = await instance.delete<T>(url, config)
        return response as ApiResponse<T>
    }
}

export default request