FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["source/TrekkingForCharity.Web/TrekkingForCharity.Web.csproj", "source/TrekkingForCharity.Web/"]
#COPY ["source/TrekkingForCharity.Infrastructure/TrekkingForCharity.Infrastructure.csproj", "source/TrekkingForCharity.Infrastructure/"]
#COPY ["source/TrekkingForCharity.Core/TrekkingForCharity.Core.csproj", "source/TrekkingForCharity.Core/"]
#COPY ["source/TrekkingForCharity.Domain/TrekkingForCharity.Domain.csproj", "source/TrekkingForCharity.Domain/"]
#COPY ["source/TrekkingForCharity.Queries/TrekkingForCharity.Queries.csproj", "source/TrekkingForCharity.Queries/"]
RUN dotnet restore "source/TrekkingForCharity.Web/TrekkingForCharity.Web.csproj"
# COPY . .
# WORKDIR "/src/source/TrekkingForCharity.Web"
# RUN dotnet build "TrekkingForCharity.Web.csproj" -c Release -o /app

# FROM build AS publish
# RUN dotnet publish "TrekkingForCharity.Web.csproj" -c Release -o /app

# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app .
# CMD dotnet TrekkingForCharity.Web.dll