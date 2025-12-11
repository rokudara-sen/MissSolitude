# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["MissSolitude.UI/MissSolitude.UI.csproj", "MissSolitude.UI/"]
COPY ["MissSolitude.Contracts/MissSolitude.Contracts.csproj", "MissSolitude.Contracts/"]
COPY ["MissSolitude.Domain/MissSolitude.Domain.csproj", "MissSolitude.Domain/"]
RUN dotnet restore "MissSolitude.UI/MissSolitude.UI.csproj"

COPY . .
WORKDIR "/src/MissSolitude.UI"
RUN dotnet publish "MissSolitude.UI.csproj" -c $BUILD_CONFIGURATION -o /app/publish

# Serve static files
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
RUN rm -rf ./*
COPY --from=build /app/publish/wwwroot .
COPY MissSolitude.UI/nginx.conf /etc/nginx/conf.d/default.conf

# ✅ Remove UTF-8 BOM if present (fixes "﻿server")
RUN sed -i '1s/^\xEF\xBB\xBF//' /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
