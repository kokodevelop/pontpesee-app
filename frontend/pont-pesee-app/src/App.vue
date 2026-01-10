<script setup lang="ts">
import { RouterView, useRoute } from 'vue-router'
import { computed } from 'vue'

const route = useRoute()
const isAuthRoute = computed(() => route.name === 'login')
</script>

<template>
  <v-app id="app" :class="{ 'auth-route': isAuthRoute }">
    <RouterView />
  </v-app>
</template>

<style>
html, body {
  margin: 0;
  padding: 0;
  height: 100%;
  width: 100%;
  overflow-x: hidden;
  font-family: 'Roboto', sans-serif;
}

#app {
  height: 100%;
  width: 100%;
}

/* When on auth route, avoid the two-column grid applied in `main.css` */
#app.auth-route {
  display: block !important;
  max-width: 100% !important;
  padding: 0 1rem !important;
}

.v-application {
  height: 100% !important;
}

.v-application__wrap {
  min-height: 100% !important;
}
</style>
