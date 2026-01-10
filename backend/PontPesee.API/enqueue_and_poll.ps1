$body = '{"tableName":"pesee","dateDebut":"2025-01-01","dateFin":"2025-01-10","mouvement":"ENTREE","acceptes":true,"page":1,"pageSize":50}'
try {
    $resp = Invoke-RestMethod -Uri 'http://localhost:5059/api/reports/statistics/pdf/queue' -Method Post -Body $body -ContentType 'application/json' -ErrorAction Stop
} catch {
    Write-Host "Enqueue failed:`n$($_.Exception.Message)"
    exit 1
}
$jobId = $resp.jobId
Write-Host "Enqueued job: $jobId"
for ($i=0; $i -lt 60; $i++) {
    try {
        $s = Invoke-RestMethod -Uri "http://localhost:5059/api/reports/statistics/pdf/queue/$jobId/status" -Method Get -ErrorAction Stop
        Write-Host "Status: $($s.status)"
        if ($s.status -eq 'Completed') {
            Invoke-WebRequest "http://localhost:5059/api/reports/statistics/pdf/queue/$jobId/download" -OutFile "statistiques_$jobId.pdf"
            Write-Host "Downloaded: statistiques_$jobId.pdf"
            exit 0
        }
        if ($s.status -eq 'Failed') {
            Write-Host "Job failed: $($s.error)"
            exit 2
        }
    } catch {
        Write-Host "Poll error: $($_.Exception.Message)"
    }
    Start-Sleep -Seconds 2
}
Write-Host "Timed out"
exit 3
