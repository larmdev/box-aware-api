# ---------- build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy solution
COPY Box/Box.sln Box/

# copy csproj files
COPY Box/Box.API/Box.API.csproj Box/Box.API/
COPY Box/Box.Application/Box.Application.csproj Box/Box.Application/
COPY Box/Box.Domain/Box.Domain.csproj Box/Box.Domain/
COPY Box/Box.Infrastructure/Box.Infrastructure.csproj Box/Box.Infrastructure/

# restore
RUN dotnet restore Box/Box.API/Box.API.csproj

# copy everything
COPY Box/ Box/

# publish
RUN dotnet publish Box/Box.API/Box.API.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


# ---------- runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Box.API.dll"]
