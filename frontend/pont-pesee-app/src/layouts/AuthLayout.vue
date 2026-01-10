<template>
  <v-main class="auth-main">
    <v-container fluid class="fill-height d-flex align-center justify-center pa-0">
      <slot />
    </v-container>
  </v-main>
</template>

<script setup lang="ts">
// Simple layout to isolate auth page styles
</script>

<style scoped>
.auth-main {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  /* Make the auth v-main span the full grid so it isn't constrained
     by a two-column layout on #app (avoids truncated login card). */
  grid-column: 1 / -1;
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 2rem 0;
}

.auth-main .v-container {
  padding: 0;
  width: 100%;
  max-width: 920px; /* keep login card reasonably constrained */
}

.auth-main .v-card {
  width: 100%;
  box-sizing: border-box;
}
</style>
