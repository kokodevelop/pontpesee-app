<template>
  <DefaultLayout>
    <v-container fluid class="pa-8">
        <v-row justify="center">
          <v-col cols="12" md="8" lg="6">
            <v-card elevation="4">
              <v-card-title class="text-h5 bg-primary text-white pa-6">
                <v-icon class="mr-2">mdi-magnify</v-icon>
                Recherche de Ticket
              </v-card-title>

              <v-card-text class="pa-6">
                <v-row>
                  <!-- Sélection du site -->
                  <v-col cols="12">
                    <v-select
                      v-model="tableName"
                      :items="sites"
                      item-title="text"
                      item-value="value"
                      label="Site"
                      prepend-inner-icon="mdi-office-building"
                      variant="outlined"
                    ></v-select>
                  </v-col>

                  <!-- Numéro de ticket -->
                  <v-col cols="12">
                    <v-text-field
                      v-model="numTicket"
                      label="Numéro de Ticket"
                      placeholder="Ex: 68175/PBS/5/2025"
                      prepend-inner-icon="mdi-ticket"
                      variant="outlined"
                      :error-messages="errorMessage"
                      @keyup.enter="searchTicket"
                    ></v-text-field>
                  </v-col>

                  <!-- Boutons -->
                  <v-col cols="12" class="d-flex gap-2">
                    <v-btn
                      color="primary"
                      prepend-icon="mdi-magnify"
                      @click="searchTicket"
                      :loading="loading"
                      size="large"
                    >
                      Rechercher
                    </v-btn>

                    <v-btn
                      variant="outlined"
                      prepend-icon="mdi-refresh"
                      @click="reset"
                      size="large"
                    >
                      Réinitialiser
                    </v-btn>
                  </v-col>
                </v-row>
              </v-card-text>
            </v-card>

            <!-- Résultat de la recherche -->
            <v-card v-if="pesee" class="mt-6" elevation="4">
              <v-card-title class="bg-success text-white pa-4 d-flex align-center">
                <v-icon class="mr-2">mdi-check-circle</v-icon>
                Ticket Trouvé
              </v-card-title>

              <v-card-text class="pa-6">
                <v-row>
                  <v-col cols="12" sm="6">
                    <div class="info-item mb-4">
                      <div class="text-caption text-grey-darken-1">Numéro de Ticket</div>
                      <div class="text-h6 font-weight-bold">{{ pesee.numTicket }}</div>
                    </div>
                  </v-col>

                  <v-col cols="12" sm="6">
                    <div class="info-item mb-4">
                      <div class="text-caption text-grey-darken-1">Site</div>
                      <div class="text-h6 font-weight-bold">{{ pesee.codeSite }}</div>
                    </div>
                  </v-col>

                  <v-col cols="12" sm="6">
                    <div class="info-item mb-4">
                      <div class="text-caption text-grey-darken-1">Date</div>
                      <div class="text-h6">{{ formatDate(pesee.dmv) }}</div>
                    </div>
                  </v-col>

                  <v-col cols="12" sm="6">
                    <div class="info-item mb-4">
                      <div class="text-caption text-grey-darken-1">Immatriculation</div>
                      <div class="text-h6">{{ pesee.immatriculation }}</div>
                    </div>
                  </v-col>

                  <v-col cols="12" sm="4">
                    <div class="info-item mb-4">
                      <div class="text-caption text-grey-darken-1">Poids 1 (kg)</div>
                      <div class="text-h6">{{ formatNumber(pesee.poids1) }}</div>
                    </div>
                  </v-col>

                  <v-col cols="12" sm="4">
                    <div class="info-item mb-4">
                      <div class="text-caption text-grey-darken-1">Poids 2 (kg)</div>
                      <div class="text-h6">{{ formatNumber(pesee.poids2) }}</div>
                    </div>
                  </v-col>

                  <v-col cols="12" sm="4">
                    <div class="info-item mb-4">
                      <div class="text-caption text-grey-darken-1">Poids Net (kg)</div>
                      <div class="text-h5 font-weight-bold text-success">{{ formatNumber(pesee.poidsNet) }}</div>
                    </div>
                  </v-col>

                  <v-col cols="12" sm="6">
                    <div class="info-item mb-4">
                      <div class="text-caption text-grey-darken-1">Fournisseur</div>
                      <div class="text-body-1">{{ pesee.idFournisseur }}</div>
                    </div>
                  </v-col>

                  <v-col cols="12" sm="6">
                    <div class="info-item mb-4">
                      <div class="text-caption text-grey-darken-1">Client</div>
                      <div class="text-body-1">{{ pesee.nomClient }}</div>
                    </div>
                  </v-col>

                  <v-col cols="12" sm="6">
                    <div class="info-item mb-4">
                      <div class="text-caption text-grey-darken-1">Produit</div>
                      <div class="text-body-1">{{ pesee.label }}</div>
                    </div>
                  </v-col>

                  <v-col cols="12" sm="6">
                    <div class="info-item mb-4">
                      <div class="text-caption text-grey-darken-1">Chauffeur</div>
                      <div class="text-body-1">{{ pesee.chauffeur }}</div>
                    </div>
                  </v-col>

                  <v-col cols="12">
                    <v-divider class="my-4"></v-divider>
                  </v-col>

                  <v-col cols="12" class="text-center">
                    <v-btn
                      color="success"
                      size="x-large"
                      prepend-icon="mdi-printer"
                      @click="printTicket"
                    >
                      Imprimer le Ticket
                    </v-btn>
                  </v-col>
                </v-row>
              </v-card-text>
            </v-card>

            <!-- Message si aucun ticket trouvé -->
            <v-alert
              v-if="showNotFound"
              type="warning"
              variant="tonal"
              class="mt-6"
            >
              <v-alert-title>Ticket non trouvé</v-alert-title>
              Aucun ticket avec ce numéro n'a été trouvé dans la base de données du site sélectionné.
            </v-alert>
          </v-col>
        </v-row>
      </v-container>
  </DefaultLayout>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import DefaultLayout from '@/layouts/DefaultLayout.vue'
import { peseeAPI } from '@/services/api'

const router = useRouter()

const loading = ref(false)
const numTicket = ref('')
const tableName = ref('pesee')
const pesee = ref<any>(null)
const errorMessage = ref('')
const showNotFound = ref(false)

const sites = [
  { text: 'SOPRECI GUTRI', value: 'pesee' },
  { text: 'SOPRECI OLODIO', value: 'peseep2' }
]

const searchTicket = async () => {
  if (!numTicket.value.trim()) {
    errorMessage.value = 'Veuillez entrer un numéro de ticket'
    return
  }

  loading.value = true
  errorMessage.value = ''
  pesee.value = null
  showNotFound.value = false

  try {
    const response = await peseeAPI.getByTicket(numTicket.value, tableName.value)
    pesee.value = response.data
  } catch (error: any) {
    if (error.response?.status === 404) {
      showNotFound.value = true
    } else {
      errorMessage.value = 'Erreur lors de la recherche du ticket'
    }
  } finally {
    loading.value = false
  }
}

const reset = () => {
  numTicket.value = ''
  tableName.value = 'pesee'
  pesee.value = null
  errorMessage.value = ''
  showNotFound.value = false
}

const formatNumber = (value: number | null) => {
  if (value === null || value === undefined) return '0'
  return new Intl.NumberFormat('fr-FR').format(value)
}

const formatDate = (dateString: string | null) => {
  if (!dateString) return '-'
  return new Date(dateString).toLocaleDateString('fr-FR', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

const printTicket = () => {
  console.log('Imprimer le ticket:', pesee.value?.numTicket)
  // TODO: Implémenter l'impression réelle
  alert('Fonction d\'impression à implémenter')
}
</script>

<style scoped>
.gap-2 {
  gap: 8px;
}

.info-item {
  padding: 8px;
  border-radius: 4px;
  background-color: #f5f5f5;
}
</style>
