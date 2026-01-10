$cwd='d:\PROJETVBNET2015\PONT VBPP2 CAM2ADMIN\PONT_WEBAPP\backend\PontPesee.API'
Start-Process -FilePath 'dotnet' -ArgumentList 'run','--urls','http://localhost:5059' -WorkingDirectory $cwd -WindowStyle Hidden
$uri='http://localhost:5059/api/reports/testpdf'
$out='d:\PROJETVBNET2015\PONT VBPP2 CAM2ADMIN\PONT_WEBAPP\backend\PontPesee.API\test_output.pdf'
for ($i = 0; $i -lt 20; $i++) {
    try {
        Invoke-WebRequest -Uri $uri -Method GET -OutFile $out -UseBasicParsing -TimeoutSec 10
        Write-Output 'Downloaded'
        break
    } catch {
        Start-Sleep -Seconds 1
    }
}
if (-not (Test-Path $out)) { Write-Output 'Download failed'; exit 1 } else { Get-Item $out | Format-List }
