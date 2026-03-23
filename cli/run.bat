@echo off
setlocal
dotnet nuget locals all --clear

cd .\src\PupaMVCF.ExampleProcess
dotnet add package PupaMVCF.Framework
dotnet run
echo @_@ its done!
endlocal