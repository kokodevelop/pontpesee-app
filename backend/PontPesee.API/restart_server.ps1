$owning = (Get-NetTCPConnection -LocalPort 5059 -ErrorAction SilentlyContinue | Select-Object -ExpandProperty OwningProcess -First 1)
if ($owning) { Write-Output "Killing PID $owning"; Stop-Process -Id $owning -Force -ErrorAction SilentlyContinue }
Start-Process -FilePath 'dotnet' -ArgumentList 'run','--urls','http://localhost:5059' -WorkingDirectory 'd:\PROJETVBNET2015\PONT VBPP2 CAM2ADMIN\PONT_WEBAPP\backend\PontPesee.API' -WindowStyle Hidden
Start-Sleep -Seconds 3
Write-Output 'Started new process'
Get-Process -Name dotnet | Select-Object -First 5 | Format-Table Id,ProcessName,StartTime -AutoSize
