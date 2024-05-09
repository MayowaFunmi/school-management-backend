# Use the official .NET Core SDK image from Microsoft
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory in the container
WORKDIR /app

# Copy the project file and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the remaining source code
COPY . ./

# Build the application
RUN dotnet publish -c Release -o out

# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Set the working directory in the container
WORKDIR /app

# Copy the published application from the build stage
COPY --from=build /app/out ./

# Expose the port the app runs on
EXPOSE 80
EXPOSE 443

# Start the application
ENTRYPOINT ["dotnet", "SchoolManagementApi.dll"]


# FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# WORKDIR /app
# EXPOSE 8080
# EXPOSE 8081

# FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
# ARG BUILD_CONFIGURATION=Release
# WORKDIR /src

# COPY ["SchoolManagementApi/SchoolManagementApi.csproj", "SchoolManagementApi/"]
# RUN dotnet restore "SchoolManagementApi/SchoolManagementApi.csproj"
# COPY . .
# WORKDIR "/src/SchoolManagementApi"
# RUN dotnet build "SchoolManagementApi.csproj" -c $BUILD_CONFIGURATION -o /app/build
# FROM build AS publish
# ARG BUILD_CONFIGURATION=Release
# RUN dotnet publish "SchoolManagementApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app/publish .
# ENTRYPOINT [ "dotnet", "SchoolManagementApi.dll" ]