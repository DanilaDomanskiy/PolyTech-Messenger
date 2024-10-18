FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 7205

ENV ASPNETCORE_URLS=http://+:7205

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /src
COPY Web.API/Web.API.csproj Web.API/
COPY Web.Application/Web.Application.csproj Web.Application/
COPY Web.Core/Web.Core.csproj Web.Core/
COPY Web.Persistence/Web.Persistence.csproj Web.Persistence/
COPY Web.Infrastructure/Web.Infrastructure.csproj Web.Infrastructure/
RUN dotnet restore Web.API/Web.API.csproj

COPY . .
WORKDIR /src/Web.API
RUN dotnet build Web.API.csproj -c Release -o /app/build
RUN dotnet publish Web.API.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build-env /app/publish .
ENTRYPOINT ["dotnet", "Web.API.dll"]
