@echo off
REM Simple build-for-all-archs script, generates binaries for x86, x64 and ARM
REM You need to use the ARM binary ot install on a actual device
REM ------------------------------------------------------------------------
REM Only use x86 for emulating
REM And i really dont know why but i also added an x64 support


REM PowerShell -NoProfile -ExecutionPolicy Bypass -File ".\build.ps1" -Platforms x86,x64,ARM

PowerShell -NoProfile -ExecutionPolicy Bypass -File ".\build.ps1" -Platforms ARM