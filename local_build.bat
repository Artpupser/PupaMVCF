@echo off
setlocal
mkdir C:\LocalNuget
dotnet nuget locals all --clear
dotnet nuget add source C:\LocalNuget -n Local
cd .\src\PupaMVCF.Framework
echo Generate package PupaMVCF.Framework...
dotnet publish
dotnet pack -c Release
dotnet nuget push .\bin\Release\PupaMVCF.Framework.0.8.0.nupkg -s C:\LocalNuget
echo Generate PgSql package PupaMVCF.Framework.Database.PgSql...
cd ..\PupaMVCF.Framework.Database.PgSql
dotnet add package PupaMVCF.Framework
dotnet publish
dotnet pack -c Release
dotnet nuget push .\bin\Release\PupaMVCF.Framework.Database.PgSql.1.0.0.nupkg -s C:\LocalNuget
echo Killing process
taskkill /F /IM dotnet.exe
taskkill /F /IM MsBuild.exe
@REM echo Runing ExampleProcess
@REM cd ..\PupaMVCF.ExampleProcess
@REM dotnet run
echo @_@ its done!
endlocal