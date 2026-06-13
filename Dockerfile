FROM mcr.microsoft.com/dotnet/nightly/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/nightly/sdk:10.0 AS build
WORKDIR /src
COPY ["HospitalManagementSystem.API/HospitalManagementSystem.API.csproj", "HospitalManagementSystem.API/"]
RUN dotnet restore "HospitalManagementSystem.API/HospitalManagementSystem.API.csproj"
COPY . .
WORKDIR "/src/HospitalManagementSystem.API"
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "HospitalManagementSystem.API.dll"]
