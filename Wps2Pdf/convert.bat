@echo off

set TMPFILE="%~dp0~args.bat"

del /f /q "%~dp0docs\*.pdf" 2>nul >nul

echo set FILES=>%TMPFILE%
for %%i in ("%~dp0docs\*") do (
	echo set FILES=%%FILES%% "%%i">>%TMPFILE%
)

call %TMPFILE%

del /q %TMPFILE%

"%~dp0bin\Release\Wps2Pdf.exe" %FILES%

pause
