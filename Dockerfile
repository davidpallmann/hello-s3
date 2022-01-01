#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["hello-s3.csproj", "."]
RUN dotnet restore "./hello-s3.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "hello-s3.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "hello-s3.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "hello-s3.dll"]