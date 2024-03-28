# Use the official Microsoft .NET SDK 6.0 image as the base image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

# Docker build arguments
ARG databaseConnectionString
ARG jwtKey
ARG jwtIssuer
ARG jwtAudience

# Set the working directory in the container to /App
WORKDIR /App

# Copy everything from the current directory to the /App directory in the container
COPY . ./

# Restore the necessary packages for the application
RUN dotnet restore

# Build the application in Release mode and output the built files to the /out directory
RUN dotnet publish -c Release -o out

# Use the official Microsoft ASP.NET 6.0 runtime image as the base image
FROM mcr.microsoft.com/dotnet/aspnet:6.0

# Set the working directory in the container to /App
WORKDIR /App

# Copy the built files from the /App/out directory in the build-env image to the /App directory in this image
COPY --from=build-env /App/out .

# Set the environment variables
ENV DatabaseConnectionString $databaseConnectionString
ENV JwtKey $jwtKey
ENV JwtIssuer $jwtIssuer
ENV JwtAudience $jwtAudience

# Inform Docker that the container is listening on the specified port at runtime
EXPOSE 8080

# Specify what command to run when the container starts
ENTRYPOINT ["dotnet", "CookingRecipes.Api.dll"]
