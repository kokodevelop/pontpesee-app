<script setup lang="ts">
import { computed } from 'vue'
import { Line } from 'vue-chartjs'
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
  Filler
} from 'chart.js'

ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
  Filler
)

const props = defineProps<{
  labels: string[]
  datasets: {
    label: string
    data: number[]
    borderColor?: string
    backgroundColor?: string
  }[]
  title?: string
}>()

const defaultColors = [
  { border: '#1976D2', background: 'rgba(25, 118, 210, 0.1)' },
  { border: '#43A047', background: 'rgba(67, 160, 71, 0.1)' },
  { border: '#FB8C00', background: 'rgba(251, 140, 0, 0.1)' }
]

const chartData = computed(() => ({
  labels: props.labels,
  datasets: props.datasets.map((ds, index) => {
    const colorSet = defaultColors[index % defaultColors.length]!
    return {
      label: ds.label,
      data: ds.data,
      borderColor: ds.borderColor || colorSet.border,
      backgroundColor: ds.backgroundColor || colorSet.background,
      fill: true,
      tension: 0.3
    }
  })
}))

const chartOptions = computed(() => ({
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      display: true,
      position: 'top' as const
    },
    title: {
      display: !!props.title,
      text: props.title || ''
    }
  },
  scales: {
    y: {
      beginAtZero: true,
      ticks: {
        callback: function(value: number | string) {
          const numValue = Number(value)
          if (numValue >= 1000000) {
            return (numValue / 1000000).toFixed(1) + 'M'
          } else if (numValue >= 1000) {
            return (numValue / 1000).toFixed(1) + 'k'
          }
          return numValue
        }
      }
    }
  },
  interaction: {
    intersect: false,
    mode: 'index' as const
  }
}))
</script>

<template>
  <div class="chart-container">
    <Line :data="chartData" :options="chartOptions" />
  </div>
</template>

<style scoped>
.chart-container {
  position: relative;
  height: 300px;
  width: 100%;
}
</style>
