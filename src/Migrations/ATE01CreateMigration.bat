@echo off
cd /d "%~dp0"
echo ===================================================================================
echo Please enter the appsettings.json file directory relative to the migration project.
echo default: ..\KSW.ATE01.Start
echo ===================================================================================
set /p basePath=
if "%basePath%" EQU "" set basePath="..\KSW.ATE01.Start"

:enterMigrationName
echo ============================
echo Please enter migration name.
echo ============================
set /p migrationName=
if "%migrationName%" EQU "" goto :enterMigrationName

echo.
echo ===========================================
echo Add Migration for KSW.ATE01.Sqlite.
echo ===========================================
dotnet ef migrations add %migrationName% --project ../KSW.ATE01.Sqlite -- --basePath %basePath%

pause