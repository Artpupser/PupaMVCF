@echo off
setlocal
mkdir C:\LocalNuget
dotnet nuget locals all --clear
dotnet nuget add source C:\LocalNuget -n Local
cd ..\src\PupaMVCF.Framework
echo Generate package PupaMVCF.Framework...
dotnet publish
dotnet pack -c Release -o C:\LocalNuget
echo Killing process
taskkill /F /IM dotnet.exe
taskkill /F /IM MsBuild.exe
cd ..\PupaMVCF.ExampleProcess
dotnet build
dotnet run
@REM echo Runing ExampleProcess
@REM cd ..\PupaMVCF.ExampleProcess
@REM dotnet run
echo @_@ its done!
endlocal