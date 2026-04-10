# Sử dụng image SDK để build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy file csproj và restore các dependencies
COPY ["NguyenDucHuy_2123110217_ASP.csproj", "./"]
RUN dotnet restore "./NguyenDucHuy_2123110217_ASP.csproj"

# Copy toàn bộ code và build
COPY . .
RUN dotnet build "NguyenDucHuy_2123110217_ASP.csproj" -c Release -o /app/build

# Publish ứng dụng
FROM build AS publish
RUN dotnet publish "NguyenDucHuy_2123110217_ASP.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Sử dụng image runtime để chạy ứng dụng
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NguyenDucHuy_2123110217_ASP.dll"]