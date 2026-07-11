import { defineStore } from 'pinia'
import { ref } from 'vue'
import { login, register, getCurrentUser, type UserDto, type LoginDto, type RegisterDto } from '@/api/auth'

export const useUserStore = defineStore('user', () => {
  const user = ref<UserDto | null>(null)
  const isLoggedIn = ref(false)

  const loginAction = async (data: LoginDto) => {
    const result = await login(data)
    user.value = result
    isLoggedIn.value = true
    localStorage.setItem('token', result.token)
  }

  const registerAction = async (data: RegisterDto) => {
    const result = await register(data)
    user.value = result
    isLoggedIn.value = true
    localStorage.setItem('token', result.token)
  }

  const logout = () => {
    user.value = null
    isLoggedIn.value = false
    localStorage.removeItem('token')
  }

  const checkAuth = async () => {
    const token = localStorage.getItem('token')
    if (token) {
      try {
        const result = await getCurrentUser()
        user.value = result
        isLoggedIn.value = true
      } catch {
        logout()
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