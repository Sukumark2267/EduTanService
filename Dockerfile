# Use .NET SDK for build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Set working directory
WORKDIR /src

# Copy all projects and solution file (preserve structure)
COPY PovVoyage/PovVoyage.sln PovVoyage/
COPY PovVoyage/ PovVoyage/
COPY PovVoyage.Data/ PovVoyage.Data/
COPY PovVoyage.Services/ PovVoyage.Services/

# Go into the directory containing the .sln
WORKDIR /src/PovVoyage

# Restore and build from there
RUN dotnet restore PovVoyage.sln
RUN dotnet publish PovVoyage.sln -c Release -o /app/publish

# Use runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "PovVoyage.dll"]
