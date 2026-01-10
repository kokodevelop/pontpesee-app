import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { authAPI, type LoginRequest } from '@/services/api'

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(localStorage.getItem('token'))
  const username = ref<string | null>(localStorage.getItem('username'))
  const profil = ref<string | null>(localStorage.getItem('profil'))

  const isAuthenticated = computed(() => !!token.value)
  const isPeseur = computed(() => profil.value === 'PESEUR')
  const isSuperviseur = computed(() => profil.value === 'SUPERVISEUR')

  async function login(credentials: LoginRequest) {
    try {
      const response = await authAPI.login(credentials)
      const { token: authToken, username: user, profil: userProfil } = response.data

      token.value = authToken
      username.value = user
      profil.value = userProfil

      localStorage.setItem('token', authToken)
      localStorage.setItem('username', user)
      localStorage.setItem('profil', userProfil)

      return true
    } catch (error) {
      console.error('Login error:', error)
      return false
    }
  }

  function logout() {
    token.value = null
    username.value = null
    profil.value = null

    localStorage.removeItem('token')
    localStorage.removeItem('username')
    localStorage.removeItem('profil')

    authAPI.logout().catch(() => {
      // Ignore errors on logout
    })
  }

  return {
    token,
    username,
    profil,
    isAuthenticated,
    isPeseur,
    isSuperviseur,
    login,
    logout
  }
})
