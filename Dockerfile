# TEST, IS NOT WORKING DOCKERFILE
FROM ubuntu:24.04
FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app
COPY . ./
WORKDIR /app/src/PupaMVCF.ExampleProcess
RUN dotnet restore
RUN dotnet publish -c Release
WORKDIR /app/src/PupaMVCF.ExampleProcess/bin/Release/net8.0/
EXPOSE 51141
ENTRYPOINT ["dotnet", "PupaMVCF.ExampleProcess.dll"]
