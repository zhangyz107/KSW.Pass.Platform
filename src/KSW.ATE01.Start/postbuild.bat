@echo off
cd /d "%~dp0"
echo param[1] = %1
echo param[2] = %2
xcopy %1template\ %2 /s /e /y
pause
