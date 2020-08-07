FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src

# copy csproj and restore as distinct layers
COPY *.sln .
COPY TagTool.Data/*.csproj ./TagTool.Data/
COPY TagTool.Test/*.csproj ./TagTool.Test/
COPY TagTool.Web/*.csproj ./TagTool.Web/
RUN dotnet restore

# copy everything else and build app
COPY . .
WORKDIR "/src/."
RUN dotnet build --configuration Release -o /app/build

# Unit Test
RUN dotnet test

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish --no-restore

# final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TagTool.Web.dll"]