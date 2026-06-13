FROM mcr.microsoft.com/dotnet/nightly/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/nightly/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet remove HospitalManagementSystem.API/HospitalManagementSystem.API.csproj package Microsoft.EntityFrameworkCore.SqlServer
RUN dotnet publish "HospitalManagementSystem.API/HospitalManagementSystem.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "HospitalManagementSystem.API.dll"]
