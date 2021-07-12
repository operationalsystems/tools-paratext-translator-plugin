FROM mcr.microsoft.com/dotnet/framework/sdk:4.8 AS build-env

WORKDIR /app

# Restore NuGet dependencies
COPY packages.config ./
RUN nuget.exe restore -SolutionDirectory ./

# Copy everything else and build
COPY . ./
RUN msbuild.exe TvpMain.csproj -p:Configuration=Release -p:Platform=AnyCPU