@echo off
F:
CD F:\MedicalHealthSYCode
"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\TF.exe" GET . /R
timeout /nobreak /t 5

if "%1" == "" goto :end
pause

:end