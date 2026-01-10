<template>
  <AuthLayout>
    <v-row align="center" justify="center">
      <v-col cols="12" sm="8" md="6" lg="4">
        <v-card class="elevation-12" rounded="lg">
          <v-card-title class="text-h4 font-weight-bold text-center pa-6 bg-primary text-white">
            <v-icon size="40" class="mr-2">mdi-scale-balance</v-icon>
            Pont Pesée
          </v-card-title>

          <v-card-subtitle class="text-center text-subtitle-1 pa-4">
            Gestion des Pesées - Multi-sites
          </v-card-subtitle>

          <v-card-text class="pa-6">
            <v-form @submit.prevent="handleLogin" ref="form">
              <v-text-field
                v-model="username"
                label="Nom d'utilisateur"
                prepend-inner-icon="mdi-account"
                variant="outlined"
                :rules="[rules.required]"
                :error-messages="errorMessage"
                class="mb-3"
                autofocus
              ></v-text-field>

              <v-text-field
                v-model="password"
                label="Mot de passe"
                prepend-inner-icon="mdi-lock"
                :type="showPassword ? 'text' : 'password'"
                :append-inner-icon="showPassword ? 'mdi-eye-off' : 'mdi-eye'"
                @click:append-inner="showPassword = !showPassword"
                variant="outlined"
                :rules="[rules.required]"
                :error-messages="errorMessage"
                class="mb-4"
              ></v-text-field>

              <v-btn
                type="submit"
                color="primary"
                size="large"
                block
                :loading="loading"
                class="mt-4"
              >
                <v-icon left class="mr-2">mdi-login</v-icon>
                Se connecter
              </v-btn>
            </v-form>

            <v-alert
              v-if="errorMessage"
              type="error"
              variant="tonal"
              class="mt-4"
            >
              {{ errorMessage }}
            </v-alert>
          </v-card-text>

          <v-card-actions class="pa-6 pt-0">
            <v-spacer></v-spacer>
            <span class="text-caption text-grey">Version 1.0.0 - 2026</span>
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>
  </AuthLayout>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import AuthLayout from '@/layouts/AuthLayout.vue'

const router = useRouter()
const authStore = useAuthStore()

const username = ref('')
const password = ref('')
const showPassword = ref(false)
const loading = ref(false)
const errorMessage = ref('')
const form = ref()

const rules = {
  required: (value: string) => !!value || 'Ce champ est requis'
}

const handleLogin = async () => {
  // Valider le formulaire
  const { valid } = await form.value.validate()

  if (!valid) return

  loading.value = true
  errorMessage.value = ''

  try {
    const success = await authStore.login({
      username: username.value,
      password: password.value
    })

    if (success) {
      router.push({ name: 'dashboard' })
    } else {
      errorMessage.value = 'Nom d\'utilisateur ou mot de passe incorrect'
    }
  } catch (error: any) {
    errorMessage.value = error.response?.data?.message || 'Nom d\'utilisateur ou mot de passe incorrect'
  } finally {
    loading.value = false
  }
}
</script>

