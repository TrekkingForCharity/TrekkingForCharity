FROM trekkingforcharity/aspcore-prodbase:latest AS builder

ARG SC_LOGIN="UNKNOWN"
ARG SC_VERSION="UNKNOWN"

WORKDIR /sln

# COPY AND MAKE BUILD FILES
COPY ./build/SonarQube.Analysis.xml ./SonarQube.Analysis.xml 
COPY ./TrekkingForCharity.sln ./
COPY ./shared/TrekkingForCharity.Shared/TrekkingForCharity.Shared.shproj  ./shared/TrekkingForCharity.Shared/TrekkingForCharity.Shared.shproj
COPY ./source/TrekkingForCharity.Web/TrekkingForCharity.Web.csproj  ./source/TrekkingForCharity.Web/TrekkingForCharity.Web.csproj
COPY ./source/TrekkingForCharity.Infrastructure/TrekkingForCharity.Infrastructure.csproj  ./source/TrekkingForCharity.Infrastructure/TrekkingForCharity.Infrastructure.csproj
COPY ./source/TrekkingForCharity.Core/TrekkingForCharity.Core.csproj  ./source/TrekkingForCharity.Core/TrekkingForCharity.Core.csproj
COPY ./source/TrekkingForCharity.Domain/TrekkingForCharity.Domain.csproj  ./source/TrekkingForCharity.Domain/TrekkingForCharity.Domain.csproj
COPY ./source/TrekkingForCharity.Queries/TrekkingForCharity.Queries.csproj  ./source/TrekkingForCharity.Queries/TrekkingForCharity.Queries.csproj
COPY ./tests/TrekkingForCharity.Tests/TrekkingForCharity.Tests.csproj  ./tests/TrekkingForCharity.Tests/TrekkingForCharity.Tests.csproj

COPY ./tests ./tests
COPY ./shared ./shared
COPY ./source ./source

RUN mkdir -p ./build/cover && \
    mkdir -p ./build/publish

RUN dotnet sonarscanner begin \
    /o:"trekking-for-charity" \
    /k:"TrekkingForCharity_TrekkingForCharity" \
    /v:${SC_VERSION} \
    /d:sonar.login=${SC_LOGIN} \
    /s:"/sln/SonarQube.Analysis.xml"; \
    exit 0;

RUN dotnet restore "./TrekkingForCharity.sln" \
    && npm install --prefix "./source/TrekkingForCharity.Web/"

RUN dotnet build "./TrekkingForCharity.sln" --configuration Release --no-restore \
    && npm run release:build --prefix "./source/TrekkingForCharity.Web/"

LABEL test=true
RUN dotnet test "./tests/TrekkingForCharity.Tests/TrekkingForCharity.Tests.csproj" \
    --logger "xunit;LogFilePath=/sln/build/cover/results.xunit.xml" \
    --configuration Release \
    --no-build \
    --no-restore \
    /property:CollectCoverage=True \
    /property:CoverletOutputFormat=\"cobertura,opencover\" \
    /property:CoverletOutput="/sln/build/cover/results"

RUN dotnet publish "./source/TrekkingForCharity.Web/TrekkingForCharity.Web.csproj" \
    --output "/sln/build/build-artifacts/publish" \
    --configuration Release \
    --no-restore

RUN dotnet sonarscanner end /d:sonar.login=${SC_LOGIN}; exit 0


#App image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.0
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT Production
COPY --from=builder ./sln/build/build-artifacts/publish .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet TrekkingForCharity.Web.dll