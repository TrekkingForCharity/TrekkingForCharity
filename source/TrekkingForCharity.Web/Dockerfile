FROM trekkingforcharity/aspcore-devbase AS base
EXPOSE 80
WORKDIR /src
COPY ["source/TrekkingForCharity.Web/TrekkingForCharity.Web.csproj", "source/TrekkingForCharity.Web/"]
COPY ["source/TrekkingForCharity.Infrastructure/TrekkingForCharity.Infrastructure.csproj", "source/TrekkingForCharity.Infrastructure/"]
COPY ["source/TrekkingForCharity.Core/TrekkingForCharity.Core.csproj", "source/TrekkingForCharity.Core/"]
COPY ["source/TrekkingForCharity.Domain/TrekkingForCharity.Domain.csproj", "source/TrekkingForCharity.Domain/"]
COPY ["source/TrekkingForCharity.Queries/TrekkingForCharity.Queries.csproj", "source/TrekkingForCharity.Queries/"]
RUN dotnet restore "source/TrekkingForCharity.Web/TrekkingForCharity.Web.csproj"
COPY . .
WORKDIR "/src/source/TrekkingForCharity.Web"
ENV ASPNETCORE_URLS "http://0.0.0.0:80"
ENTRYPOINT ["dotnet", "run", "TrekkingForCharity.Web.csproj", "-c", "Debug", "-o", "/app"]

# FROM build AS publish
# RUN dotnet publish "TrekkingForCharity.Web.csproj" -c Release -o /app

# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app .
#CMD dotnet TrekkingForCharity.Web.dll