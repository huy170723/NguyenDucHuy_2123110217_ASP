# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy file csproj từ thư mục con vào
COPY ["NguyenDucHuy_2123110217_ASP/NguyenDucHuy_2123110217_ASP.csproj", "NguyenDucHuy_2123110217_ASP/"]
RUN dotnet restore "NguyenDucHuy_2123110217_ASP/NguyenDucHuy_2123110217_ASP.csproj"

# Copy toàn bộ và build
COPY . .
WORKDIR "/src/NguyenDucHuy_2123110217_ASP"
RUN dotnet build "NguyenDucHuy_2123110217_ASP.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NguyenDucHuy_2123110217_ASP.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NguyenDucHuy_2123110217_ASP.dll"]
