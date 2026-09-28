FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["src/DevHabit.Api/DevHabit.Api.csproj", "src/DevHabit.Api/"]
RUN dotnet restore "src/DevHabit.Api/DevHabit.Api.csproj"
COPY . .
WORKDIR "/src/src/DevHabit.Api"
RUN dotnet build "DevHabit.Api.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "DevHabit.Api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DevHabit.Api.dll"]
