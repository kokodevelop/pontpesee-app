import axios, { type AxiosInstance } from 'axios'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL
  ? `${import.meta.env.VITE_API_BASE_URL}/api`
  : 'http://localhost:5000/api'

// Instance Axios avec configuration de base
const apiClient: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json'
  }
})

// Intercepteur pour ajouter le token JWT
apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => Promise.reject(error)
)

// Intercepteur pour gérer les erreurs d'authentification
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Token expiré, rediriger vers login
      localStorage.removeItem('token')
      localStorage.removeItem('user')
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

// Types
export interface LoginRequest {
  username: string
  password: string
}

export interface LoginResponse {
  token: string
  username: string
  profil: string
  expiresAt: string
}

export interface PeseeFilter {
  tableName?: string
  dateDebut?: string
  dateFin?: string
  mouvement?: string
  fournisseur?: string
  client?: string
  destination?: string
  provenance?: string
  produit?: string
  vehicule?: string
  chauffeur?: string
  codeSite?: string
  acceptes?: boolean
  annules?: boolean
  tous?: boolean
  page?: number
  pageSize?: number
}

export interface Pesee {
  codePesee: number
  codeSite?: string
  campagne?: string
  immatriculation?: string
  poids1?: number
  dateP1?: string
  dateP2?: string
  poids2?: number
  poidsNet?: number
  codeFournisseur?: string
  idFournisseur?: string
  codeClient?: string
  nomClient?: string
  mouvement?: string
  provenance?: string
  destination?: string
  imprime: boolean
  numTicket?: string
  chauffeur?: string
  peseur?: string
  peseur2?: string
  annuler: boolean
  label?: string
  dmv?: string
  remorque?: string
  datePesee?: string
}

export interface PeseeStatsResponse {
  nombrePesees: number
  poidsBrutTotal: number
  poidsNetTotal: number
  pesees: Pesee[]
  totalPages: number
  currentPage: number
}

// Types pour les statistiques avancées
export interface StatisticsFilter {
  tableName?: string
  dateDebut?: string
  dateFin?: string
  mouvement?: string
  fournisseur?: string
  client?: string
  produit?: string
  codeSite?: string
  topN?: number
}

export interface EntityStat {
  code: string
  nom: string
  nombrePesees: number
  poidsBrutTotal: number
  poidsNetTotal: number
  poidsMoyen: number
}

export interface MouvementStat {
  mouvement: string
  nombrePesees: number
  poidsBrutTotal: number
  poidsNetTotal: number
  pourcentage: number
}

export interface PeriodStat {
  periode: string
  dateDebut: string
  dateFin: string
  nombrePesees: number
  poidsBrutTotal: number
  poidsNetTotal: number
}

export interface StatisticsSummary {
  totalPesees: number
  totalPoidsBrut: number
  totalPoidsNet: number
  poidsMoyen: number
  nombreFournisseurs: number
  nombreClients: number
  nombreProduits: number
  nombreVehicules: number
  premierePesee?: string
  dernierePesee?: string
}

export interface AdvancedStatistics {
  summary: StatisticsSummary
  parFournisseur: EntityStat[]
  parClient: EntityStat[]
  parProduit: EntityStat[]
  parMouvement: MouvementStat[]
  parJour: PeriodStat[]
  parMois: PeriodStat[]
}

// API Services
export const authAPI = {
  login: (credentials: LoginRequest) =>
    apiClient.post<LoginResponse>('/Auth/login', credentials),

  logout: () =>
    apiClient.post('/Auth/logout')
}

export const peseeAPI = {
  search: (filter: PeseeFilter) =>
    apiClient.post<PeseeStatsResponse>('/Pesee/search', filter),

  getByTicket: (numTicket: string, tableName: string = 'pesee') =>
    apiClient.get<Pesee>('/Pesee/ticket', { params: { numTicket, tableName } }),

  getProvenances: (tableName: string = 'pesee') =>
    apiClient.get<string[]>('/Pesee/provenances', { params: { tableName } }),

  getDestinations: (tableName: string = 'pesee') =>
    apiClient.get<string[]>('/Pesee/destinations', { params: { tableName } }),

  getFournisseurs: (tableName: string = 'pesee') =>
    apiClient.get<Record<string, string>>('/Pesee/fournisseurs', { params: { tableName } }),

  getClients: (tableName: string = 'pesee') =>
    apiClient.get<Record<string, string>>('/Pesee/clients', { params: { tableName } }),

  getProduits: (tableName: string = 'pesee') =>
    apiClient.get<string[]>('/Pesee/produits', { params: { tableName } }),

  getVehicules: (tableName: string = 'pesee') =>
    apiClient.get<string[]>('/Pesee/vehicules', { params: { tableName } })

  ,
  exportStatisticsExcel: (filter: PeseeFilter) =>
    apiClient.post('/Reports/statistics/excel', filter, { responseType: 'blob' })
  ,
  exportStatisticsPdf: (filter: PeseeFilter) =>
    apiClient.post('/Reports/statistics/pdf', filter, { responseType: 'blob' })
  ,
  // Background queue endpoints
  enqueueStatisticsPdf: (filter: PeseeFilter) =>
    apiClient.post<{ jobId: string }>('/Reports/statistics/pdf/queue', filter),

  getReportJobStatus: (jobId: string) =>
    apiClient.get<{ status: string; error?: string }>(`/Reports/statistics/pdf/queue/${jobId}/status`),

  downloadReportJob: (jobId: string) =>
    apiClient.get(`/Reports/statistics/pdf/queue/${jobId}/download`, { responseType: 'blob' })
}

// API pour les statistiques avancées
export const statisticsAPI = {
  getAdvancedStatistics: (filter: StatisticsFilter) =>
    apiClient.post<AdvancedStatistics>('/Statistics/advanced', filter),

  getStatsByFournisseur: (filter: StatisticsFilter) =>
    apiClient.post<EntityStat[]>('/Statistics/by-fournisseur', filter),

  getStatsByClient: (filter: StatisticsFilter) =>
    apiClient.post<EntityStat[]>('/Statistics/by-client', filter),

  getStatsByProduit: (filter: StatisticsFilter) =>
    apiClient.post<EntityStat[]>('/Statistics/by-produit', filter),

  getStatsByMouvement: (filter: StatisticsFilter) =>
    apiClient.post<MouvementStat[]>('/Statistics/by-mouvement', filter),

  getStatsByPeriod: (filter: StatisticsFilter, periodType: string = 'jour') =>
    apiClient.post<PeriodStat[]>(`/Statistics/by-period/${periodType}`, filter)
}

export default apiClient
