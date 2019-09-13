# Build image
# FROM mono:5.20.1.34 AS mono
# WORKDIR /sln
# ENV CAKE_VERSION 0.26.1
# ENV CAKE_TARGET_FRAMEWORK netcoreapp2.0
# 
#COPY ./build/tools/packages.config   ./build/tools/
#RUN cd build && ls
# RUN ./build/build.sh build.cake --target=Build


FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS builder

ARG SC_LOGIN="UNKNOWN"

WORKDIR /sln

RUN apt-get update -yq \
    && apt-get install curl gnupg -yq \
    && curl -sL https://deb.nodesource.com/setup_10.x | bash \
    && apt-get install nodejs -yq \
    && node -v \
    && npm -v


COPY ./build/build.sh ./build/build.cake   ./build/

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

RUN dotnet tool install -g Cake.Tool
RUN dotnet tool install -g dotnet-sonarscanner

ENV PATH="${PATH}:/root/.dotnet/tools"

RUN dotnet sonarscanner begin /k:"trekking-for-charity" /o:"TrekkingForCharity" /d:sonar.host.url="https://sonarcloud.io" /d:sonar.cs.opencover.reportsPaths=**/results.opencover.xml /d:sonar.login=${SC_LOGIN}; exit 0

RUN dotnet cake ./build/build.cake --target=Clean --verbosity=diagnostic

RUN npm install --prefix ./source/TrekkingForCharity.Web/ \
    && npm run release:build --prefix ./source/TrekkingForCharity.Web/


LABEL test=true
RUN dotnet cake ./build/build.cake --target=Publish --verbosity=diagnostic

RUN dotnet sonarscanner end /d:sonar.login=${SC_LOGIN}; exit 0


#App image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT Production
ENTRYPOINT ["dotnet", "TrekkingForCharity.Web.dll"]
COPY --from=builder ./sln/build/build-artifacts/publish .