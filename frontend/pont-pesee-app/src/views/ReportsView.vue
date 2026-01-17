<template>
  <DefaultLayout>
    <v-container fluid class="pa-6">
      <!-- Filtres -->
      <v-card class="mb-6" elevation="2">
        <v-card-title class="bg-grey-lighten-3">
          <v-icon class="mr-2">mdi-filter</v-icon>
          Filtres de Rapports
        </v-card-title>

        <v-card-text class="pa-6">
          <v-row>
            <!-- Site -->
            <v-col cols="12" md="6" lg="2">
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

            <!-- Date debut -->
            <v-col cols="12" md="6" lg="2">
              <v-text-field
                v-model="filters.dateDebut"
                label="Date debut"
                type="date"
                prepend-inner-icon="mdi-calendar"
                variant="outlined"
                density="comfortable"
              ></v-text-field>
            </v-col>

            <!-- Date fin -->
            <v-col cols="12" md="6" lg="2">
              <v-text-field
                v-model="filters.dateFin"
                label="Date fin"
                type="date"
                prepend-inner-icon="mdi-calendar"
                variant="outlined"
                density="comfortable"
              ></v-text-field>
            </v-col>

            <!-- Top N -->
            <v-col cols="12" md="6" lg="2">
              <v-select
                v-model="filters.topN"
                :items="[5, 10, 15, 20]"
                label="Top N"
                prepend-inner-icon="mdi-podium"
                variant="outlined"
                density="comfortable"
              ></v-select>
            </v-col>

            <!-- Boutons -->
            <v-col cols="12" md="6" lg="6" class="d-flex align-center gap-2">
              <v-btn
                color="primary"
                prepend-icon="mdi-chart-bar"
                @click="loadStatistics"
                :loading="loading"
              >
                Generer Rapports
              </v-btn>

              <v-btn
                color="success"
                prepend-icon="mdi-file-pdf-box"
                @click="exportToPDF"
                :loading="exportingPDF"
                :disabled="!statistics || loading"
              >
                Exporter PDF
              </v-btn>

              <v-btn
                variant="outlined"
                prepend-icon="mdi-refresh"
                @click="resetFilters"
              >
                Reinitialiser
              </v-btn>
            </v-col>
          </v-row>
        </v-card-text>
      </v-card>

      <!-- Loading State -->
      <v-progress-linear v-if="loading" indeterminate color="primary" class="mb-4"></v-progress-linear>

      <!-- Statistiques Resumees -->
      <v-row v-if="statistics" class="mb-6">
        <v-col cols="12" sm="6" md="3">
          <v-card color="blue-lighten-5" elevation="2">
            <v-card-text>
              <div class="d-flex align-center">
                <v-icon size="40" color="blue">mdi-counter</v-icon>
                <div class="ml-4">
                  <div class="text-caption">Total Pesees</div>
                  <div class="text-h4 font-weight-bold">{{ formatNumber(statistics.summary.totalPesees) }}</div>
                </div>
              </div>
            </v-card-text>
          </v-card>
        </v-col>

        <v-col cols="12" sm="6" md="3">
          <v-card color="green-lighten-5" elevation="2">
            <v-card-text>
              <div class="d-flex align-center">
                <v-icon size="40" color="green">mdi-weight-kilogram</v-icon>
                <div class="ml-4">
                  <div class="text-caption">Poids Net Total</div>
                  <div class="text-h5 font-weight-bold">{{ formatWeight(statistics.summary.totalPoidsNet) }}</div>
                </div>
              </div>
            </v-card-text>
          </v-card>
        </v-col>

        <v-col cols="12" sm="6" md="3">
          <v-card color="orange-lighten-5" elevation="2">
            <v-card-text>
              <div class="d-flex align-center">
                <v-icon size="40" color="orange">mdi-truck</v-icon>
                <div class="ml-4">
                  <div class="text-caption">Fournisseurs</div>
                  <div class="text-h4 font-weight-bold">{{ statistics.summary.nombreFournisseurs }}</div>
                </div>
              </div>
            </v-card-text>
          </v-card>
        </v-col>

        <v-col cols="12" sm="6" md="3">
          <v-card color="purple-lighten-5" elevation="2">
            <v-card-text>
              <div class="d-flex align-center">
                <v-icon size="40" color="purple">mdi-account-group</v-icon>
                <div class="ml-4">
                  <div class="text-caption">Clients</div>
                  <div class="text-h4 font-weight-bold">{{ statistics.summary.nombreClients }}</div>
                </div>
              </div>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>

      <!-- Graphiques -->
      <v-row v-if="statistics">
        <!-- Par Mouvement (Doughnut) -->
        <v-col cols="12" md="6">
          <v-card elevation="2">
            <v-card-title class="bg-grey-lighten-3">
              <v-icon class="mr-2">mdi-chart-pie</v-icon>
              Repartition par Mouvement
            </v-card-title>
            <v-card-text class="pa-4">
              <DoughnutChartComponent
                v-if="mouvementChartData.labels.length > 0"
                :labels="mouvementChartData.labels"
                :data="mouvementChartData.data"
                title="Poids Net par Mouvement"
              />
              <div v-else class="text-center py-8 text-grey">Aucune donnee</div>
            </v-card-text>
          </v-card>
        </v-col>

        <!-- Par Fournisseur (Bar) -->
        <v-col cols="12" md="6">
          <v-card elevation="2">
            <v-card-title class="bg-grey-lighten-3">
              <v-icon class="mr-2">mdi-chart-bar</v-icon>
              Top Fournisseurs
            </v-card-title>
            <v-card-text class="pa-4">
              <BarChartComponent
                v-if="fournisseurChartData.labels.length > 0"
                :labels="fournisseurChartData.labels"
                :data="fournisseurChartData.data"
                label="Poids Net (kg)"
                backgroundColor="#1976D2"
              />
              <div v-else class="text-center py-8 text-grey">Aucune donnee</div>
            </v-card-text>
          </v-card>
        </v-col>

        <!-- Par Client (Bar) -->
        <v-col cols="12" md="6">
          <v-card elevation="2">
            <v-card-title class="bg-grey-lighten-3">
              <v-icon class="mr-2">mdi-chart-bar</v-icon>
              Top Clients
            </v-card-title>
            <v-card-text class="pa-4">
              <BarChartComponent
                v-if="clientChartData.labels.length > 0"
                :labels="clientChartData.labels"
                :data="clientChartData.data"
                label="Poids Net (kg)"
                backgroundColor="#43A047"
              />
              <div v-else class="text-center py-8 text-grey">Aucune donnee</div>
            </v-card-text>
          </v-card>
        </v-col>

        <!-- Par Produit (Bar) -->
        <v-col cols="12" md="6">
          <v-card elevation="2">
            <v-card-title class="bg-grey-lighten-3">
              <v-icon class="mr-2">mdi-chart-bar</v-icon>
              Top Produits
            </v-card-title>
            <v-card-text class="pa-4">
              <BarChartComponent
                v-if="produitChartData.labels.length > 0"
                :labels="produitChartData.labels"
                :data="produitChartData.data"
                label="Poids Net (kg)"
                backgroundColor="#FB8C00"
              />
              <div v-else class="text-center py-8 text-grey">Aucune donnee</div>
            </v-card-text>
          </v-card>
        </v-col>

        <!-- Evolution par Jour (Line) -->
        <v-col cols="12">
          <v-card elevation="2">
            <v-card-title class="bg-grey-lighten-3">
              <v-icon class="mr-2">mdi-chart-line</v-icon>
              Evolution Journaliere (30 derniers jours)
            </v-card-title>
            <v-card-text class="pa-4">
              <LineChartComponent
                v-if="dailyChartData.labels.length > 0"
                :labels="dailyChartData.labels"
                :datasets="dailyChartData.datasets"
                title="Poids Net par Jour"
              />
              <div v-else class="text-center py-8 text-grey">Aucune donnee</div>
            </v-card-text>
          </v-card>
        </v-col>

        <!-- Evolution par Mois (Line) -->
        <v-col cols="12">
          <v-card elevation="2">
            <v-card-title class="bg-grey-lighten-3">
              <v-icon class="mr-2">mdi-chart-line</v-icon>
              Evolution Mensuelle (12 derniers mois)
            </v-card-title>
            <v-card-text class="pa-4">
              <LineChartComponent
                v-if="monthlyChartData.labels.length > 0"
                :labels="monthlyChartData.labels"
                :datasets="monthlyChartData.datasets"
                title="Poids Net par Mois"
              />
              <div v-else class="text-center py-8 text-grey">Aucune donnee</div>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>

      <!-- Tableaux detailles -->
      <v-row v-if="statistics" class="mt-4">
        <!-- Tableau Fournisseurs -->
        <v-col cols="12" md="6">
          <v-card elevation="2">
            <v-card-title class="bg-grey-lighten-3">
              <v-icon class="mr-2">mdi-truck-delivery</v-icon>
              Details par Fournisseur
            </v-card-title>
            <v-data-table
              :headers="entityHeaders"
              :items="statistics.parFournisseur"
              :items-per-page="5"
              density="compact"
            >
              <template v-slot:item.poidsNetTotal="{ item }">
                {{ formatWeight(item.poidsNetTotal) }}
              </template>
              <template v-slot:item.poidsMoyen="{ item }">
                {{ formatWeight(item.poidsMoyen) }}
              </template>
            </v-data-table>
          </v-card>
        </v-col>

        <!-- Tableau Clients -->
        <v-col cols="12" md="6">
          <v-card elevation="2">
            <v-card-title class="bg-grey-lighten-3">
              <v-icon class="mr-2">mdi-account-group</v-icon>
              Details par Client
            </v-card-title>
            <v-data-table
              :headers="entityHeaders"
              :items="statistics.parClient"
              :items-per-page="5"
              density="compact"
            >
              <template v-slot:item.poidsNetTotal="{ item }">
                {{ formatWeight(item.poidsNetTotal) }}
              </template>
              <template v-slot:item.poidsMoyen="{ item }">
                {{ formatWeight(item.poidsMoyen) }}
              </template>
            </v-data-table>
          </v-card>
        </v-col>

        <!-- Tableau Produits -->
        <v-col cols="12">
          <v-card elevation="2">
            <v-card-title class="bg-grey-lighten-3">
              <v-icon class="mr-2">mdi-package-variant</v-icon>
              Details par Produit
            </v-card-title>
            <v-data-table
              :headers="entityHeaders"
              :items="statistics.parProduit"
              :items-per-page="5"
              density="compact"
            >
              <template v-slot:item.poidsNetTotal="{ item }">
                {{ formatWeight(item.poidsNetTotal) }}
              </template>
              <template v-slot:item.poidsMoyen="{ item }">
                {{ formatWeight(item.poidsMoyen) }}
              </template>
            </v-data-table>
          </v-card>
        </v-col>
      </v-row>

      <!-- Message si pas de donnees -->
      <v-card v-if="!statistics && !loading" class="pa-8 text-center" elevation="2">
        <v-icon size="64" color="grey-lighten-1">mdi-chart-box-outline</v-icon>
        <div class="text-h6 mt-4 text-grey">Selectionnez une periode et cliquez sur "Generer Rapports"</div>
      </v-card>
    </v-container>
  </DefaultLayout>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import DefaultLayout from '@/layouts/DefaultLayout.vue'
import BarChartComponent from '@/components/charts/BarChartComponent.vue'
import DoughnutChartComponent from '@/components/charts/DoughnutChartComponent.vue'
import LineChartComponent from '@/components/charts/LineChartComponent.vue'
import { statisticsAPI, type AdvancedStatistics, type StatisticsFilter } from '@/services/api'
import html2canvas from 'html2canvas'
import { jsPDF } from 'jspdf'

const loading = ref(false)
const exportingPDF = ref(false)
const statistics = ref<AdvancedStatistics | null>(null)

const filters = ref<StatisticsFilter>({
  tableName: 'pesee',
  dateDebut: new Date(new Date().getFullYear(), 0, 1).toISOString().split('T')[0],
  dateFin: new Date().toISOString().split('T')[0],
  topN: 10
})

const sites = [
  { text: 'SOPRECI GUTRI', value: 'pesee' },
  { text: 'SOPRECI OLODIO', value: 'peseep2' }
]

const entityHeaders = [
  { title: 'Nom', key: 'nom', sortable: true },
  { title: 'Nb Pesees', key: 'nombrePesees', sortable: true },
  { title: 'Poids Net Total', key: 'poidsNetTotal', sortable: true },
  { title: 'Poids Moyen', key: 'poidsMoyen', sortable: true }
]

// Computed pour les donnees des graphiques
const mouvementChartData = computed(() => {
  if (!statistics.value?.parMouvement) return { labels: [], data: [] }
  return {
    labels: statistics.value.parMouvement.map(m => m.mouvement),
    data: statistics.value.parMouvement.map(m => m.poidsNetTotal)
  }
})

const fournisseurChartData = computed(() => {
  if (!statistics.value?.parFournisseur) return { labels: [], data: [] }
  return {
    labels: statistics.value.parFournisseur.map(f => truncateLabel(f.nom, 20)),
    data: statistics.value.parFournisseur.map(f => f.poidsNetTotal)
  }
})

const clientChartData = computed(() => {
  if (!statistics.value?.parClient) return { labels: [], data: [] }
  return {
    labels: statistics.value.parClient.map(c => truncateLabel(c.nom, 20)),
    data: statistics.value.parClient.map(c => c.poidsNetTotal)
  }
})

const produitChartData = computed(() => {
  if (!statistics.value?.parProduit) return { labels: [], data: [] }
  return {
    labels: statistics.value.parProduit.map(p => truncateLabel(p.nom, 20)),
    data: statistics.value.parProduit.map(p => p.poidsNetTotal)
  }
})

const dailyChartData = computed(() => {
  if (!statistics.value?.parJour) return { labels: [], datasets: [] }
  return {
    labels: statistics.value.parJour.map(d => formatDateLabel(d.periode)),
    datasets: [
      {
        label: 'Poids Net (kg)',
        data: statistics.value.parJour.map(d => d.poidsNetTotal)
      },
      {
        label: 'Nb Pesees',
        data: statistics.value.parJour.map(d => d.nombrePesees * 1000) // Scale for visibility
      }
    ]
  }
})

const monthlyChartData = computed(() => {
  if (!statistics.value?.parMois) return { labels: [], datasets: [] }
  return {
    labels: statistics.value.parMois.map(m => formatMonthLabel(m.periode)),
    datasets: [
      {
        label: 'Poids Net (kg)',
        data: statistics.value.parMois.map(m => m.poidsNetTotal)
      }
    ]
  }
})

const loadStatistics = async () => {
  loading.value = true
  try {
    const filterData: StatisticsFilter = {
      ...filters.value,
      dateDebut: filters.value.dateDebut ? `${filters.value.dateDebut}T00:00:00` : undefined,
      dateFin: filters.value.dateFin ? `${filters.value.dateFin}T23:59:59` : undefined
    }

    const response = await statisticsAPI.getAdvancedStatistics(filterData)
    statistics.value = response.data
  } catch (error) {
    console.error('Erreur lors du chargement des statistiques:', error)
  } finally {
    loading.value = false
  }
}

const resetFilters = () => {
  filters.value = {
    tableName: 'pesee',
    dateDebut: new Date(new Date().getFullYear(), 0, 1).toISOString().split('T')[0],
    dateFin: new Date().toISOString().split('T')[0],
    topN: 10
  }
  statistics.value = null
}

const formatNumber = (value: number | null | undefined) => {
  if (value === null || value === undefined) return '0'
  return new Intl.NumberFormat('fr-FR').format(value)
}

const formatWeight = (value: number | null | undefined) => {
  if (value === null || value === undefined) return '0 kg'
  if (value >= 1000000) {
    return `${(value / 1000000).toFixed(2)} Mt`
  } else if (value >= 1000) {
    return `${(value / 1000).toFixed(2)} t`
  }
  return `${formatNumber(value)} kg`
}

const truncateLabel = (label: string, maxLength: number) => {
  if (!label) return ''
  return label.length > maxLength ? label.substring(0, maxLength) + '...' : label
}

const formatDateLabel = (dateStr: string) => {
  // Format: YYYY-MM-DD -> DD/MM
  const parts = dateStr.split('-')
  if (parts.length >= 3) {
    return `${parts[2]}/${parts[1]}`
  }
  return dateStr
}

const formatMonthLabel = (monthStr: string) => {
  // Format: YYYY-MM -> MM/YYYY
  const parts = monthStr.split('-')
  if (parts.length >= 2 && parts[0] && parts[1]) {
    const months = ['Jan', 'Fev', 'Mar', 'Avr', 'Mai', 'Jun', 'Jul', 'Aou', 'Sep', 'Oct', 'Nov', 'Dec']
    const monthIndex = parseInt(parts[1]!, 10) - 1
    return `${months[monthIndex]} ${parts[0]}`
  }
  return monthStr
}

const exportToPDF = async () => {
  if (!statistics.value) return

  exportingPDF.value = true
  try {
    const pdf = new jsPDF('p', 'mm', 'a4')
    const pageWidth = pdf.internal.pageSize.getWidth()
    const pageHeight = pdf.internal.pageSize.getHeight()
    let yPosition = 20

    // Title
    pdf.setFontSize(18)
    pdf.setFont('helvetica', 'bold')
    pdf.text('Rapports Avances - Statistiques de Pesee', pageWidth / 2, yPosition, { align: 'center' })

    yPosition += 10
    pdf.setFontSize(10)
    pdf.setFont('helvetica', 'normal')
    const siteName = sites.find(s => s.value === filters.value.tableName)?.text || 'Tous les sites'
    pdf.text(`Site: ${siteName}`, 15, yPosition)
    yPosition += 5
    pdf.text(`Periode: ${filters.value.dateDebut || 'N/A'} - ${filters.value.dateFin || 'N/A'}`, 15, yPosition)
    yPosition += 5
    pdf.text(`Date d'export: ${new Date().toLocaleDateString('fr-FR')}`, 15, yPosition)

    yPosition += 10

    // Summary statistics
    pdf.setFontSize(14)
    pdf.setFont('helvetica', 'bold')
    pdf.text('Resumé', 15, yPosition)
    yPosition += 7

    pdf.setFontSize(10)
    pdf.setFont('helvetica', 'normal')
    const summary = statistics.value.summary
    pdf.text(`Total Pesees: ${formatNumber(summary.totalPesees)}`, 15, yPosition)
    yPosition += 5
    pdf.text(`Poids Net Total: ${formatWeight(summary.totalPoidsNet)}`, 15, yPosition)
    yPosition += 5
    pdf.text(`Poids Moyen: ${formatWeight(summary.poidsMoyen)}`, 15, yPosition)
    yPosition += 5
    pdf.text(`Nombre de Fournisseurs: ${summary.nombreFournisseurs}`, 15, yPosition)
    yPosition += 5
    pdf.text(`Nombre de Clients: ${summary.nombreClients}`, 15, yPosition)
    yPosition += 5
    pdf.text(`Nombre de Produits: ${summary.nombreProduits}`, 15, yPosition)

    yPosition += 10

    // Capture charts as images
    const charts = document.querySelectorAll('.v-card')
    let chartIndex = 0

    for (const chart of Array.from(charts)) {
      if (chartIndex >= 6) break // Limit to the 6 main charts

      const cardTitle = chart.querySelector('.v-card-title')?.textContent?.trim()
      if (!cardTitle || !cardTitle.includes('par') && !cardTitle.includes('Evolution')) continue

      // Check if we need a new page
      if (yPosition > pageHeight - 80) {
        pdf.addPage()
        yPosition = 20
      }

      try {
        const canvas = await html2canvas(chart as HTMLElement, {
          scale: 2,
          logging: false,
          useCORS: true,
          backgroundColor: '#ffffff'
        })

        const imgData = canvas.toDataURL('image/png')
        const imgWidth = pageWidth - 30
        const imgHeight = (canvas.height * imgWidth) / canvas.width

        // Add chart title
        pdf.setFontSize(12)
        pdf.setFont('helvetica', 'bold')
        pdf.text(cardTitle, 15, yPosition)
        yPosition += 5

        // Add chart image
        pdf.addImage(imgData, 'PNG', 15, yPosition, imgWidth, Math.min(imgHeight, 80))
        yPosition += Math.min(imgHeight, 80) + 10

        chartIndex++
      } catch (error) {
        console.error('Error capturing chart:', error)
      }
    }

    // Save PDF
    const fileName = `rapport-avance-${filters.value.dateDebut || 'debut'}-${filters.value.dateFin || 'fin'}.pdf`
    pdf.save(fileName)
  } catch (error) {
    console.error('Error generating PDF:', error)
    alert('Erreur lors de la génération du PDF')
  } finally {
    exportingPDF.value = false
  }
}

onMounted(() => {
  // Auto-load on mount
  loadStatistics()
})
</script>

<style scoped>
.gap-2 {
  gap: 8px;
}
</style>
