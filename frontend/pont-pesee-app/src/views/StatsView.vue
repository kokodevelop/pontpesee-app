<template>
  <DefaultLayout>
    <v-container fluid class="pa-6">
        <!-- Filtres -->
        <v-card class="mb-6" elevation="2">
          <v-card-title class="bg-grey-lighten-3">
            <v-icon class="mr-2">mdi-filter</v-icon>
            Filtres de Recherche
          </v-card-title>

          <v-card-text class="pa-6">
            <v-row>
              <!-- Sélection du site -->
              <v-col cols="12" md="6" lg="3">
                <v-select
                  v-model="filters.tableName"
                  :items="sites"
                  item-title="text"
                  item-value="value"
                  label="Site"
                  prepend-inner-icon="mdi-office-building"
                  variant="outlined"
                  density="comfortable"
                ></v-select>
              </v-col>

              <!-- Date début -->
              <v-col cols="12" md="6" lg="3">
                <v-text-field
                  v-model="filters.dateDebut"
                  label="Date début"
                  type="date"
                  prepend-inner-icon="mdi-calendar"
                  variant="outlined"
                  density="comfortable"
                ></v-text-field>
              </v-col>

              <!-- Date fin -->
              <v-col cols="12" md="6" lg="3">
                <v-text-field
                  v-model="filters.dateFin"
                  label="Date fin"
                  type="date"
                  prepend-inner-icon="mdi-calendar"
                  variant="outlined"
                  density="comfortable"
                ></v-text-field>
              </v-col>

              <!-- Mouvement -->
              <v-col cols="12" md="6" lg="3">
                <v-select
                  v-model="filters.mouvement"
                  :items="['ENTREE', 'SORTIE']"
                  label="Mouvement"
                  prepend-inner-icon="mdi-swap-vertical"
                  variant="outlined"
                  density="comfortable"
                  clearable
                ></v-select>
              </v-col>

              <!-- Fournisseur -->
              <v-col cols="12" md="6" lg="4">
                <v-autocomplete
                  v-model="filters.fournisseur"
                  :items="fournisseurItems"
                  label="Fournisseur"
                  prepend-inner-icon="mdi-truck-delivery"
                  variant="outlined"
                  density="comfortable"
                  clearable
                ></v-autocomplete>
              </v-col>

              <!-- Client -->
              <v-col cols="12" md="6" lg="4">
                <v-autocomplete
                  v-model="filters.client"
                  :items="clientItems"
                  label="Client"
                  prepend-inner-icon="mdi-account-group"
                  variant="outlined"
                  density="comfortable"
                  clearable
                ></v-autocomplete>
              </v-col>

              <!-- Produit -->
              <v-col cols="12" md="6" lg="4">
                <v-autocomplete
                  v-model="filters.produit"
                  :items="produits"
                  label="Produit"
                  prepend-inner-icon="mdi-package-variant"
                  variant="outlined"
                  density="comfortable"
                  clearable
                ></v-autocomplete>
              </v-col>

              <!-- Boutons d'action -->
              <v-col cols="12" class="d-flex gap-2">
                <v-btn
                  color="primary"
                  prepend-icon="mdi-magnify"
                  @click="loadPesees"
                  :loading="loading"
                >
                  Rechercher
                </v-btn>

                <v-btn
                  variant="outlined"
                  prepend-icon="mdi-refresh"
                  @click="resetFilters"
                >
                  Réinitialiser
                </v-btn>

                <v-spacer></v-spacer>

                <v-btn
                  color="success"
                  prepend-icon="mdi-file-excel"
                  variant="outlined"
                  @click="exportExcel"
                >
                  Exporter Excel
                </v-btn>
                <v-btn
                  color="primary"
                  prepend-icon="mdi-file-pdf"
                  variant="outlined"
                  @click="enqueuePdf"
                >
                  Aperçu PDF
                </v-btn>
              </v-col>
            </v-row>
          </v-card-text>
        </v-card>

        <!-- Export Jobs -->
        <v-card class="mb-6" elevation="2">
          <v-card-title class="bg-grey-lighten-3">
            <v-icon class="mr-2">mdi-history</v-icon>
            Export Jobs
          </v-card-title>

          <v-card-text class="pa-6">
            <div v-if="jobs.length === 0" class="text-caption">Aucun job en file.</div>
            <v-list v-else>
              <v-list-item v-for="job in jobs" :key="job.id">
                <v-list-item-content>
                  <v-list-item-title>{{ job.id }}</v-list-item-title>
                  <v-list-item-subtitle>Status: {{ job.status }}</v-list-item-subtitle>
                </v-list-item-content>

                <v-list-item-action>
                  <v-btn v-if="job.status === 'Completed'" small color="primary" @click="downloadJob(job)">Télécharger</v-btn>
                  <v-btn v-else-if="job.status === 'Downloaded'" small variant="outlined" disabled>Downloaded</v-btn>
                  <v-btn v-else-if="job.status === 'Failed'" small color="error" disabled>Failed</v-btn>
                  <v-btn v-else small variant="outlined" disabled>En cours...</v-btn>
                </v-list-item-action>
              </v-list-item>
            </v-list>
          </v-card-text>
        </v-card>

        <!-- Statistiques -->
        <v-row v-if="stats" class="mb-6">
          <v-col cols="12" sm="4">
            <v-card color="blue-lighten-5" elevation="2">
              <v-card-text>
                <div class="d-flex align-center">
                  <v-icon size="40" color="blue">mdi-counter</v-icon>
                  <div class="ml-4">
                    <div class="text-caption">Nombre de Pesées</div>
                    <div class="text-h4 font-weight-bold">{{ stats.nombrePesees }}</div>
                  </div>
                </div>
              </v-card-text>
            </v-card>
          </v-col>

          <v-col cols="12" sm="4">
            <v-card color="green-lighten-5" elevation="2">
              <v-card-text>
                <div class="d-flex align-center">
                  <v-icon size="40" color="green">mdi-weight-kilogram</v-icon>
                  <div class="ml-4">
                    <div class="text-caption">Poids Brut Total (kg)</div>
                    <div class="text-h5 font-weight-bold">{{ formatNumber(stats.poidsBrutTotal) }}</div>
                  </div>
                </div>
              </v-card-text>
            </v-card>
          </v-col>

          <v-col cols="12" sm="4">
            <v-card color="orange-lighten-5" elevation="2">
              <v-card-text>
                <div class="d-flex align-center">
                  <v-icon size="40" color="orange">mdi-scale</v-icon>
                  <div class="ml-4">
                    <div class="text-caption">Poids Net Total (kg)</div>
                    <div class="text-h5 font-weight-bold">{{ formatNumber(stats.poidsNetTotal) }}</div>
                  </div>
                </div>
              </v-card-text>
            </v-card>
          </v-col>
        </v-row>

        <!-- Tableau des pesées -->
        <v-card elevation="2">
          <v-card-title class="bg-grey-lighten-3">
            <v-icon class="mr-2">mdi-table</v-icon>
            Liste des Pesées
          </v-card-title>

          <v-data-table
            :headers="headers"
            :items="pesees"
            :loading="loading"
            :items-per-page="10"
            class="elevation-1"
          >
            <template v-slot:item.poidsNet="{ item }">
              <v-chip color="success" size="small">
                {{ formatNumber(item.poidsNet) }} kg
              </v-chip>
            </template>

            <template v-slot:item.mouvement="{ item }">
              <v-chip
                :color="item.mouvement === 'ENTREE' ? 'primary' : 'warning'"
                size="small"
              >
                {{ item.mouvement }}
              </v-chip>
            </template>

            <template v-slot:item.annuler="{ item }">
              <v-icon v-if="item.annuler" color="error">mdi-close-circle</v-icon>
              <v-icon v-else color="success">mdi-check-circle</v-icon>
            </template>

            <template v-slot:item.dmv="{ item }">
              {{ formatDate(item.dmv) }}
            </template>

            <template v-slot:item.actions="{ item }">
              <v-btn
                icon="mdi-printer"
                size="small"
                variant="text"
                color="primary"
                @click="printTicket(item)"
              ></v-btn>
            </template>
          </v-data-table>
        </v-card>
      
        <v-snackbar v-model="snackbar.show" :color="snackbar.color" timeout="6000" location="top right">
          {{ snackbar.message }}
        </v-snackbar>
      </v-container>
  </DefaultLayout>
</template>

<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue'
import { useRouter } from 'vue-router'
import DefaultLayout from '@/layouts/DefaultLayout.vue'
import { peseeAPI } from '@/services/api'

const router = useRouter()

const loading = ref(false)
const pesees = ref<any[]>([])
const stats = ref<any>(null)
const fournisseurs = ref<Record<string, string>>({})
const clients = ref<Record<string, string>>({})
const produits = ref<string[]>([])

const filters = ref({
  tableName: 'pesee',
  dateDebut: new Date(new Date().getFullYear(), 0, 1).toISOString().split('T')[0],
  dateFin: new Date().toISOString().split('T')[0],
  mouvement: null as string | null,
  fournisseur: null as string | null,
  client: null as string | null,
  produit: null as string | null,
  page: 0,
  pageSize: 100
})

const sites = [
  { text: 'SOPRECI GUTRI', value: 'pesee' },
  { text: 'SOPRECI OLODIO', value: 'peseep2' }
]

const headers = [
  { title: 'Ticket', key: 'numTicket', sortable: true },
  { title: 'Site', key: 'codeSite', sortable: true },
  { title: 'Date', key: 'dmv', sortable: true },
  { title: 'Immatriculation', key: 'immatriculation', sortable: true },
  { title: 'Fournisseur', key: 'idFournisseur', sortable: true },
  { title: 'Client', key: 'nomClient', sortable: true },
  { title: 'Produit', key: 'label', sortable: true },
  { title: 'Poids Net', key: 'poidsNet', sortable: true },
  { title: 'Mouvement', key: 'mouvement', sortable: true },
  { title: 'Annulé', key: 'annuler', sortable: true },
  { title: 'Actions', key: 'actions', sortable: false }
]

const fournisseurItems = computed(() => {
  return Object.entries(fournisseurs.value).map(([code, nom]) => ({
    title: `${code} - ${nom}`,
    value: nom
  }))
})

const clientItems = computed(() => {
  return Object.entries(clients.value).map(([code, nom]) => ({
    title: `${code} - ${nom}`,
    value: nom
  }))
})

const loadPesees = async () => {
  loading.value = true
  try {
    const filterData = {
      ...filters.value,
      dateDebut: filters.value.dateDebut ? `${filters.value.dateDebut}T00:00:00` : undefined,
      dateFin: filters.value.dateFin ? `${filters.value.dateFin}T23:59:59` : undefined,
      tous: true
    }

    const response = await peseeAPI.search(filterData as any)
    pesees.value = response.data.pesees || []
    stats.value = response.data
  } catch (error) {
    console.error('Erreur lors du chargement des pesées:', error)
  } finally {
    loading.value = false
  }
}

const loadReferenceData = async () => {
  try {
    const [fournisseursRes, clientsRes, produitsRes] = await Promise.all([
      peseeAPI.getFournisseurs(filters.value.tableName),
      peseeAPI.getClients(filters.value.tableName),
      peseeAPI.getProduits(filters.value.tableName)
    ])

    fournisseurs.value = fournisseursRes.data || {}
    clients.value = clientsRes.data || {}
    produits.value = produitsRes.data || []

    console.log('Fournisseurs chargés:', fournisseurs.value)
    console.log('Clients chargés:', clients.value)
    console.log('Produits chargés:', produits.value)
  } catch (error) {
    console.error('Erreur lors du chargement des données de référence:', error)
  }
}

const resetFilters = () => {
  filters.value = {
    tableName: 'pesee',
    dateDebut: new Date(new Date().getFullYear(), 0, 1).toISOString().split('T')[0],
    dateFin: new Date().toISOString().split('T')[0],
    mouvement: null,
    fournisseur: null,
    client: null,
    produit: null,
    page: 0,
    pageSize: 100
  }
  loadPesees()
}

const formatNumber = (value: number | null) => {
  if (value === null || value === undefined) return '0'
  return new Intl.NumberFormat('fr-FR').format(value)
}

const formatDate = (dateString: string | null) => {
  if (!dateString) return '-'
  return new Date(dateString).toLocaleDateString('fr-FR')
}

const printTicket = (item: any) => {
  console.log('Imprimer le ticket:', item.numTicket)
  // TODO: Implémenter l'impression
}

const exportExcel = async () => {
  loading.value = true
  try {
    const filterData = {
      ...filters.value,
      dateDebut: filters.value.dateDebut ? `${filters.value.dateDebut}T00:00:00` : undefined,
      dateFin: filters.value.dateFin ? `${filters.value.dateFin}T23:59:59` : undefined,
      tous: true,
      page: 0,
      pageSize: 1000000
    }

    const response = await peseeAPI.exportStatisticsExcel(filterData as any)
    const blob = new Blob([response.data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' })
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = `statistiques_${new Date().toISOString().slice(0,19).replace(/[:T]/g,'_')}.xlsx`
    document.body.appendChild(link)
    link.click()
    link.remove()
    window.URL.revokeObjectURL(url)
  } catch (error) {
    console.error('Erreur export Excel', error)
  } finally {
    loading.value = false
  }
}

const exportPdf = async () => {
  loading.value = true
  try {
    const filterData = {
      ...filters.value,
      dateDebut: filters.value.dateDebut ? `${filters.value.dateDebut}T00:00:00` : undefined,
      dateFin: filters.value.dateFin ? `${filters.value.dateFin}T23:59:59` : undefined,
      tous: true,
      page: 0,
      pageSize: 1000000
    }

    const response = await peseeAPI.exportStatisticsPdf(filterData as any)
    const blob = new Blob([response.data], { type: 'application/pdf' })
    const url = window.URL.createObjectURL(blob)
    window.open(url)
    // keep blob URL for user to print/download from new tab
  } catch (error) {
    console.error('Erreur export PDF', error)
  } finally {
    loading.value = false
  }
}

// Background enqueue + polling
const jobs = ref<Array<{ id: string; status: string }>>([])
const snackbar = ref({ show: false, message: '', color: 'success' })

const pollJobStatus = async (jobId: string) => {
  try {
    const res = await peseeAPI.getReportJobStatus(jobId)
    const status = res.data.status
    const idx = jobs.value.findIndex(j => j.id === jobId)
    if (idx >= 0 && jobs.value[idx]) jobs.value[idx].status = status
    if (status === 'Completed') {
      snackbar.value = { show: true, message: `Rapport prêt: ${jobId}`, color: 'success' }
      // auto-download the completed report
      if (jobs.value[idx]) {
        // small delay to ensure backend file ready
        setTimeout(() => downloadJob({ id: jobId, status: status }), 500)
      }
      return
    }
    if (status === 'Failed') {
      snackbar.value = { show: true, message: `Erreur génération du rapport: ${jobId}`, color: 'error' }
      return
    }
    // poll again
    setTimeout(() => pollJobStatus(jobId), 2000)
  } catch (err) {
    console.error('Poll job error', err)
    const idx = jobs.value.findIndex(j => j.id === jobId)
    if (idx >= 0 && jobs.value[idx]) jobs.value[idx].status = 'Failed'
    snackbar.value = { show: true, message: `Erreur génération du rapport: ${jobId}`, color: 'error' }
  }
}

const enqueuePdf = async () => {
  loading.value = true
  try {
    const filterData = {
      ...filters.value,
      dateDebut: filters.value.dateDebut ? `${filters.value.dateDebut}T00:00:00` : undefined,
      dateFin: filters.value.dateFin ? `${filters.value.dateFin}T23:59:59` : undefined,
      tous: true,
      page: 0,
      pageSize: 1000000
    }

    const resp = await peseeAPI.enqueueStatisticsPdf(filterData as any)
    const jobId = resp.data.jobId
    jobs.value.unshift({ id: jobId, status: 'Pending' })
    snackbar.value = { show: true, message: `Job enqueued: ${jobId}`, color: 'info' }
    // start polling
    pollJobStatus(jobId)
  } catch (error) {
    console.error('Erreur enqueue PDF', error)
  } finally {
    loading.value = false
  }
}

const downloadJob = async (job: { id: string; status: string }) => {
  try {
    const res = await peseeAPI.downloadReportJob(job.id)
    const blob = new Blob([res.data], { type: 'application/pdf' })
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = `statistiques_${job.id}.pdf`
    document.body.appendChild(link)
    link.click()
    link.remove()
    window.URL.revokeObjectURL(url)
    // mark as downloaded in the UI
    const idx = jobs.value.findIndex(j => j.id === job.id)
    if (idx >= 0 && jobs.value[idx]) {
      jobs.value[idx].status = 'Downloaded'
    }
    snackbar.value = { show: true, message: `Fichier téléchargé: ${job.id}`, color: 'success' }
  } catch (err) {
    console.error('Download job failed', err)
    snackbar.value = { show: true, message: `Erreur téléchargement: ${job.id}`, color: 'error' }
  }
}

// Recharger les données de référence quand on change de site
watch(() => filters.value.tableName, () => {
  loadReferenceData()
})

onMounted(() => {
  loadReferenceData()
  loadPesees()
})
</script>

<style scoped>
.gap-2 {
  gap: 8px;
}
</style>
