# Use official .NET SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy project files and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the app files and build the application
COPY . ./
RUN dotnet publish -c Release -o /publish

# Use ASP.NET runtime image for running the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /publish .

# Expose the port used by your app
EXPOSE 5000

# Start the application
CMD ["dotnet", "DraganaMakeup.dll"]
