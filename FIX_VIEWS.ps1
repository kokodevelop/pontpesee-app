# Script PowerShell pour corriger les vues Vue.js
# Retire les balises <v-app> et </v-app> des vues individuelles

$files = @(
    "D:\PROJETVBNET2015\PONT VBPP2 CAM2ADMIN\PONT_WEBAPP\frontend\pont-pesee-app\src\views\DashboardView.vue",
    "D:\PROJETVBNET2015\PONT VBPP2 CAM2ADMIN\PONT_WEBAPP\frontend\pont-pesee-app\src\views\StatsView.vue",
    "D:\PROJETVBNET2015\PONT VBPP2 CAM2ADMIN\PONT_WEBAPP\frontend\pont-pesee-app\src\views\ReeditionView.vue"
)

foreach ($file in $files) {
    if (Test-Path $file) {
        $content = Get-Content $file -Raw

        # Remplacer <v-app> par <div>
        $content = $content -replace '<template>\s*<v-app>', '<template>
  <div>'

        # Remplacer </v-app> avant </template> par </div>
        $content = $content -replace '</v-app>\s*</template>', '  </div>
</template>'

        Set-Content -Path $file -Value $content -NoNewline

        Write-Host "✅ Corrigé: $file" -ForegroundColor Green
    } else {
        Write-Host "❌ Fichier non trouvé: $file" -ForegroundColor Red
    }
}

Write-Host "`n🎉 Correction terminée! Rechargez la page dans le navigateur." -ForegroundColor Cyan
