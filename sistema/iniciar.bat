@echo off
REM ============================================================
REM SUNSET - inicia tudo de uma vez no seu computador:
REM   1) confere .NET e SQL Server
REM   2) cria o banco (se ainda nao existe) e os dados de exemplo
REM   3) abre a Api (porta 5218), o site (porta 5157) e o app Flutter
REM Uso: duplo clique, ou "iniciar_tudo.bat sem-app" para nao abrir o Flutter
REM ============================================================
setlocal
title SUNSET - iniciar sistema

REM ---------- configuracoes (mude aqui se precisar) ----------
REM Instancia do SQL Server. Se for a instancia padrao, use: set SERVIDOR=localhost
set SERVIDOR=localhost\SQLEXPRESS
set COM_APP=1
if /i "%~1"=="sem-app" set COM_APP=0

REM O codigo do sistema esta na pasta de cima (raiz do repositorio)
for %%I in ("%~dp0..") do set PASTA=%%~fI
if not exist "%PASTA%\Sunset.sln" goto sem_pasta

REM Faz Api, Web e o dotnet ef usarem o servidor definido acima (sem mexer nos appsettings)
set ConnectionStrings__Padrao=Server=%SERVIDOR%;Database=Sunset;Trusted_Connection=True;TrustServerCertificate=True
set PATH=%PATH%;%USERPROFILE%\.dotnet\tools

echo.
echo === 1/5 Conferindo o .NET
where dotnet >nul 2>nul
if errorlevel 1 goto sem_dotnet
dotnet --version

echo.
echo === 2/5 Conferindo o SQL Server (%SERVIDOR%)
if /i "%SERVIDOR%"=="localhost\SQLEXPRESS" (
    sc query "MSSQL$SQLEXPRESS" | find "RUNNING" >nul
    if errorlevel 1 (
        echo O servico do SQL Server Express nao esta rodando. Tentando iniciar...
        net start "MSSQL$SQLEXPRESS" >nul 2>nul
        sc query "MSSQL$SQLEXPRESS" | find "RUNNING" >nul
        if errorlevel 1 goto sem_sql
    )
)
echo SQL Server ok.

echo.
echo === 3/5 Criando o banco (se ja existir, nada muda)
where dotnet-ef >nul 2>nul
if errorlevel 1 (
    echo Instalando a ferramenta dotnet-ef...
    dotnet tool install --global dotnet-ef
)
pushd "%PASTA%"
dotnet ef database update --project Sunset.Repositorio --startup-project Sunset.Api
if errorlevel 1 (
    popd
    goto erro_banco
)

echo.
echo === 4/5 Dados de exemplo
where sqlcmd >nul 2>nul
if errorlevel 1 (
    echo sqlcmd nao encontrado. Pulando os dados de exemplo.
    echo Se quiser, rode banco\dados_exemplo.sql no SSMS.
    goto subir
)
REM So insere se o usuario de exemplo ainda nao existe, para nao duplicar
set JA_TEM=0
for /f "usebackq delims=" %%A in (`sqlcmd -S "%SERVIDOR%" -E -C -d Sunset -h -1 -W -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM Usuario WHERE Email='bruno.os@outlook.com'" 2^>nul`) do set JA_TEM=%%A
if "%JA_TEM%"=="0" (
    sqlcmd -S "%SERVIDOR%" -E -C -i "%PASTA%\banco\dados_exemplo.sql"
    echo Dados de exemplo inseridos.
) else (
    echo Dados de exemplo ja existem. Pulando.
)

:subir
popd
echo.
echo === 5/5 Abrindo Api, site e app
start "SUNSET - Api (porta 5218)" cmd /k "cd /d "%PASTA%" && dotnet run --project Sunset.Api --launch-profile http"
start "SUNSET - Site (porta 5157)" cmd /k "cd /d "%PASTA%" && dotnet run --project Sunset.Web --launch-profile http"

if "%COM_APP%"=="0" goto fim
where flutter >nul 2>nul
if errorlevel 1 (
    echo Flutter nao encontrado. O app nao sera aberto. Use "iniciar_tudo.bat sem-app" para esconder este aviso.
    goto fim
)
start "SUNSET - App Flutter" cmd /k "cd /d "%PASTA%\sunset_app" && flutter pub get && flutter run -d chrome"

:fim
echo.
echo Pronto. Cada programa abriu em uma janela propria.
echo   Site:  http://localhost:5157
echo   Api:   http://localhost:5218/scalar
echo   Login cliente: bruno.os@outlook.com / Senha1234
echo   Login admin:   admin@sunset.com / Admin@1234
echo Para encerrar tudo, rode parar_tudo.bat.
echo.
pause
exit /b 0

:sem_pasta
echo [ERRO] Nao achei "%PASTA%".
echo A pasta iniciar-sistema precisa ficar dentro da pasta do projeto, onde esta o Sunset.sln.
pause
exit /b 1

:sem_dotnet
echo [ERRO] .NET SDK nao encontrado. Instale o .NET SDK 9 ou mais novo (dotnet.microsoft.com/download) e abra um terminal novo.
pause
exit /b 1

:sem_sql
echo [ERRO] Nao consegui iniciar o SQL Server Express (%SERVIDOR%).
echo Abra "Servicos" do Windows, inicie "SQL Server (SQLEXPRESS)" e rode de novo.
echo Se o seu servidor tem outro nome, mude a linha "set SERVIDOR=" no inicio deste arquivo.
pause
exit /b 1

:erro_banco
echo [ERRO] Nao consegui criar o banco. Veja a mensagem acima.
echo Confira o nome do servidor na linha "set SERVIDOR=" deste arquivo.
pause
exit /b 1
