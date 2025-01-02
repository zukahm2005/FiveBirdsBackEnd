# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy project file và restore dependencies
COPY five-birds-be.csproj ./
RUN dotnet restore five-birds-be.csproj

# COPY wwwroot ./wwwroot

# Copy toàn bộ mã nguồn và build ứng dụng
COPY . ./
RUN dotnet publish five-birds-be.csproj -c Release -o /app/out


# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "five-birds-be.dll"]
