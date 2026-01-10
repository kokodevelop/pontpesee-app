<template>
  <DefaultLayout>
    <v-container fluid class="pa-8">
        <v-row>
          <v-col cols="12">
            <h1 class="text-h3 font-weight-bold mb-2">Tableau de Bord</h1>
            <p class="text-subtitle-1 text-grey-darken-1">Gestion des pesées - Multi-sites (SOPRECI GUTRI & OLODIO)</p>
          </v-col>
        </v-row>

        <v-row class="mt-6">
          <!-- Card Statistiques -->
          <v-col cols="12" md="6" lg="4">
            <v-card
              class="menu-card"
              hover
              @click="navigateTo('stats')"
              elevation="4"
            >
              <v-card-text class="text-center pa-8">
                <v-avatar size="80" color="primary" class="mb-4">
                  <v-icon size="50" color="white">mdi-chart-bar</v-icon>
                </v-avatar>
                <h2 class="text-h5 font-weight-bold mb-2">Statistiques</h2>
                <p class="text-body-2 text-grey-darken-1">
                  Consulter les pesées, filtrer par date, site, fournisseur, client
                </p>
              </v-card-text>
            </v-card>
          </v-col>

          <!-- Card Réédition de tickets -->
          <v-col cols="12" md="6" lg="4">
            <v-card
              class="menu-card"
              hover
              @click="navigateTo('reedition')"
              elevation="4"
            >
              <v-card-text class="text-center pa-8">
                <v-avatar size="80" color="success" class="mb-4">
                  <v-icon size="50" color="white">mdi-printer</v-icon>
                </v-avatar>
                <h2 class="text-h5 font-weight-bold mb-2">Réédition</h2>
                <p class="text-body-2 text-grey-darken-1">
                  Réimprimer un ticket de pesée par numéro
                </p>
              </v-card-text>
            </v-card>
          </v-col>

          <!-- Card Export -->
          <v-col cols="12" md="6" lg="4">
            <v-card
              class="menu-card"
              hover
              elevation="4"
            >
              <v-card-text class="text-center pa-8">
                <v-avatar size="80" color="orange" class="mb-4">
                  <v-icon size="50" color="white">mdi-file-excel</v-icon>
                </v-avatar>
                <h2 class="text-h5 font-weight-bold mb-2">Export Excel</h2>
                <p class="text-body-2 text-grey-darken-1">
                  Exporter les données vers Excel
                </p>
                <v-chip size="small" color="warning" class="mt-2">Bientôt disponible</v-chip>
              </v-card-text>
            </v-card>
          </v-col>
        </v-row>

        <!-- Statistiques rapides -->
        <v-row class="mt-8">
          <v-col cols="12">
            <h2 class="text-h5 font-weight-bold mb-4">Aperçu Rapide</h2>
          </v-col>

          <v-col cols="12" sm="6" md="3">
            <v-card color="blue-lighten-5" elevation="2">
              <v-card-text>
                <div class="d-flex align-center">
                  <v-icon size="40" color="blue">mdi-database</v-icon>
                  <div class="ml-4">
                    <div class="text-caption text-grey-darken-1">Total Pesées</div>
                    <div class="text-h4 font-weight-bold">-</div>
                  </div>
                </div>
              </v-card-text>
            </v-card>
          </v-col>

          <v-col cols="12" sm="6" md="3">
            <v-card color="green-lighten-5" elevation="2">
              <v-card-text>
                <div class="d-flex align-center">
                  <v-icon size="40" color="green">mdi-truck</v-icon>
                  <div class="ml-4">
                    <div class="text-caption text-grey-darken-1">Poids Net Total</div>
                    <div class="text-h4 font-weight-bold">-</div>
                  </div>
                </div>
              </v-card-text>
            </v-card>
          </v-col>

          <v-col cols="12" sm="6" md="3">
            <v-card color="orange-lighten-5" elevation="2">
              <v-card-text>
                <div class="d-flex align-center">
                  <v-icon size="40" color="orange">mdi-calendar-today</v-icon>
                  <div class="ml-4">
                    <div class="text-caption text-grey-darken-1">Aujourd'hui</div>
                    <div class="text-h4 font-weight-bold">-</div>
                  </div>
                </div>
              </v-card-text>
            </v-card>
          </v-col>

          <v-col cols="12" sm="6" md="3">
            <v-card color="purple-lighten-5" elevation="2">
              <v-card-text>
                <div class="d-flex align-center">
                  <v-icon size="40" color="purple">mdi-office-building</v-icon>
                  <div class="ml-4">
                    <div class="text-caption text-grey-darken-1">Sites Actifs</div>
                    <div class="text-h4 font-weight-bold">2</div>
                  </div>
                </div>
              </v-card-text>
            </v-card>
          </v-col>
        </v-row>
      </v-container>
  </DefaultLayout>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import DefaultLayout from '@/layouts/DefaultLayout.vue'

const router = useRouter()

const navigateTo = (routeName: string) => {
  router.push({ name: routeName })
}
</script>

<style scoped>
.menu-card {
  transition: transform 0.2s ease-in-out, box-shadow 0.2s ease-in-out;
  cursor: pointer;
}

.menu-card:hover {
  transform: translateY(-8px);
  box-shadow: 0 12px 24px rgba(0, 0, 0, 0.15) !important;
}

/* Defensive rules: ensure menu cards render horizontally even if global CSS
   accidentally sets vertical writing modes or transforms elsewhere. */
.menu-card {
  min-width: 220px;
  max-width: 360px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.menu-card * {
  writing-mode: horizontal-tb !important;
  transform: none !important;
  white-space: normal !important;
}

.menu-card .v-card-text {
  display: flex;
  flex-direction: column;
  align-items: center;
}
</style>
