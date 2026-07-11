import {defineStore} from 'pinia'
import {ref} from 'vue'
import { login,register,getCurrentUser ,type UserDto} from '@/api/auth'

export const useUserStore = defineStore('user', () =>
{
    const user = ref<UserDto | null>(null);
    const isLoggedIn = ref(false);
    const loginAction = async (data: {userName: string, passWord: string}) =>
      {
        const res = await login(data);
        user.value = res;
        isLoggedIn.value = true;
        localStorage.setItem('token', res.token);
      } 

      const registerAction = async (data: RegisterDto) =>
      {
        const res = await register(data);
        user.value = res;
        isLoggedIn.value = true;
        localStorage.setItem('token', res.token);
      }
      
      const logout = () =>
      {
        user.value = null;
        isLoggedIn.value = false;
        localStorage.removeItem('token');
      }

      const checkAuth = async () =>
      {
        const token = localStorage.getItem('token');
        if(token)
        {
            try
            {       
             const res = await getCurrentUser(token);
             user.value = res;
             isLoggedIn.value = true;
             }
             catch(error)
             {
                logout();
             }
        }
      }

      return {
        user,
        isLoggedIn,
        loginAction,
        registerAction,
        logout,
        checkAuth
      }
})