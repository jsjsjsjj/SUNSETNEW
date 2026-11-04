@echo off
REM Fecha as janelas abertas pelo iniciar_tudo.bat
echo Encerrando Api, site e app do SUNSET...
taskkill /F /T /FI "WINDOWTITLE eq SUNSET - Api*" >nul 2>nul
taskkill /F /T /FI "WINDOWTITLE eq SUNSET - Site*" >nul 2>nul
taskkill /F /T /FI "WINDOWTITLE eq SUNSET - App*" >nul 2>nul
echo Pronto.
pause
