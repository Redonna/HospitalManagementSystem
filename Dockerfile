FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src
COPY ["HospitalManagementSystem.API/HospitalManagementSystem.API.csproj", "HospitalManagementSystem.API/"]
RUN dotnet restore "HospitalManagementSystem.API/HospitalManagementSystem.API.csproj"
COPY . .
WORKDIR "/src/HospitalManagementSystem.API"
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "HospitalManagementSystem.API.dll"]
