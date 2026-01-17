<script setup lang="ts">
import { computed } from 'vue'
import { Doughnut } from 'vue-chartjs'
import {
  Chart as ChartJS,
  ArcElement,
  Title,
  Tooltip,
  Legend
} from 'chart.js'

ChartJS.register(ArcElement, Title, Tooltip, Legend)

const props = defineProps<{
  labels: string[]
  data: number[]
  title?: string
}>()

const backgroundColors = [
  '#1976D2', // Blue
  '#43A047', // Green
  '#FB8C00', // Orange
  '#E53935', // Red
  '#8E24AA', // Purple
  '#00ACC1', // Cyan
  '#FFB300', // Amber
  '#5E35B1', // Deep Purple
  '#00897B', // Teal
  '#D81B60'  // Pink
]

const chartData = computed(() => ({
  labels: props.labels,
  datasets: [
    {
      backgroundColor: backgroundColors.slice(0, props.labels.length),
      data: props.data
    }
  ]
}))

const chartOptions = computed(() => ({
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      display: true,
      position: 'right' as const
    },
    title: {
      display: !!props.title,
      text: props.title || ''
    },
    tooltip: {
      callbacks: {
        label: function(context: any) {
          const value = context.raw as number
          const total = context.dataset.data.reduce((a: number, b: number) => a + b, 0)
          const percentage = ((value / total) * 100).toFixed(1)
          return `${context.label}: ${value.toLocaleString('fr-FR')} kg (${percentage}%)`
        }
      }
    }
  }
}))
</script>

<template>
  <div class="chart-container">
    <Doughnut :data="chartData" :options="chartOptions" />
  </div>
</template>

<style scoped>
.chart-container {
  position: relative;
  height: 300px;
  width: 100%;
}
</style>
