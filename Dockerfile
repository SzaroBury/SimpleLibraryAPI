# Use the official .NET SDK image to build and publish the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the project files and restore dependencies
COPY *.sln                       ./
COPY src/API/*.csproj            ./src/API/
COPY src/Application/*.csproj    ./src/Application/
COPY src/Domain/*.csproj         ./src/Domain/
COPY src/Infrastructure/*.csproj ./src/Infrastructure/
RUN dotnet restore ./src/API/API.csproj

# Copy the rest of the application and build it
COPY . .
RUN dotnet publish ./src/API/API.csproj -c Release -o publish

# Use the runtime image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Expose the port the app runs on
EXPOSE 8080
ENTRYPOINT ["dotnet", "API.dll"]