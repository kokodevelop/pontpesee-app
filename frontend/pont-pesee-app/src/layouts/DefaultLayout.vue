<template>
  <div class="default-layout">
    <v-app-bar color="primary" elevation="4">
      <v-app-bar-nav-icon @click="drawer = !drawer"></v-app-bar-nav-icon>

      <v-toolbar-title>
        <v-icon size="32" class="mr-2">mdi-scale-balance</v-icon>
        Pont Pesée
      </v-toolbar-title>

      <v-spacer></v-spacer>

      <v-chip class="mr-4" color="white" text-color="primary">
        <v-icon start>mdi-account-circle</v-icon>
        {{ authStore.username?.toUpperCase() }}
      </v-chip>

      <v-chip class="mr-4" color="white" text-color="primary" size="small">
        {{ authStore.profil }}
      </v-chip>

      <v-btn icon="mdi-logout" @click="handleLogout"></v-btn>
    </v-app-bar>

    <v-navigation-drawer v-model="drawer" temporary>
      <v-list>
        <v-list-item
          prepend-icon="mdi-view-dashboard"
          title="Tableau de Bord"
          value="dashboard"
          @click="navigateTo('dashboard')"
        ></v-list-item>

        <v-list-item
          prepend-icon="mdi-chart-bar"
          title="Statistiques"
          value="stats"
          @click="navigateTo('stats')"
        ></v-list-item>

        <v-list-item
          prepend-icon="mdi-printer"
          title="Réédition Ticket"
          value="reedition"
          @click="navigateTo('reedition')"
        ></v-list-item>
      </v-list>
    </v-navigation-drawer>

    <v-main>
      <slot></slot>
    </v-main>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const authStore = useAuthStore()
const drawer = ref(false)

const navigateTo = (routeName: string) => {
  router.push({ name: routeName })
  drawer.value = false
}

const handleLogout = () => {
  authStore.logout()
  router.push({ name: 'login' })
}
</script>

<style scoped>
/* Make the default layout span the full width of the app grid so
   dashboard and main content are not clipped into a single narrow column. */
.default-layout {
  grid-column: 1 / -1;
}

.default-layout .v-main {
  padding: 0;
}
</style>
