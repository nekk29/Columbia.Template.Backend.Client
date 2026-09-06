# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["__NAMESPACE__.Apis/__NAMESPACE__.Apis.csproj", "__NAMESPACE__.Apis/"]
COPY ["__NAMESPACE__.Application/__NAMESPACE__.Application.csproj", "__NAMESPACE__.Application/"]
COPY ["__NAMESPACE__.Application.Abstractions/__NAMESPACE__.Application.Abstractions.csproj", "__NAMESPACE__.Application.Abstractions/"]
COPY ["__NAMESPACE__.Dto/__NAMESPACE__.Dto.csproj", "__NAMESPACE__.Dto/"]
COPY ["__NAMESPACE__.Domain/__NAMESPACE__.Domain.csproj", "__NAMESPACE__.Domain/"]
COPY ["__NAMESPACE__.Common/__NAMESPACE__.Common.csproj", "__NAMESPACE__.Common/"]
COPY ["__NAMESPACE__.EmailClient/__NAMESPACE__.EmailClient.csproj", "__NAMESPACE__.EmailClient/"]
COPY ["__NAMESPACE__.Repository/__NAMESPACE__.Repository.csproj", "__NAMESPACE__.Repository/"]
COPY ["__NAMESPACE__.Repository.Abstractions/__NAMESPACE__.Repository.Abstractions.csproj", "__NAMESPACE__.Repository.Abstractions/"]
COPY ["__NAMESPACE__.Entity/__NAMESPACE__.Entity.csproj", "__NAMESPACE__.Entity/"]
RUN dotnet restore "./__NAMESPACE__.Apis/__NAMESPACE__.Apis.csproj"
COPY . .
WORKDIR "/src/__NAMESPACE__.Apis"
RUN dotnet build "./__NAMESPACE__.Apis.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./__NAMESPACE__.Apis.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "__NAMESPACE__.Apis.dll"]