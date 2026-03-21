@echo off
setlocal
dotnet nuget locals all --clear

cd .\src\PupaMVCF.ExampleAuthProcess
dotnet add package PupaMVCF.Framework
dotnet add package PupaMVCF.Framework.Database.PgSql

cd ..\PupaMVCF.ExampleTest
dotnet add package PupaMVCF.Framework
dotnet add package PupaMVCF.Framework.Database.PgSql
dotnet run
echo @_@ its done!
endlocal