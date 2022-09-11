#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Services/Meetings/Meetings.API/Meetings.API.csproj", "Services/Meetings/Meetings.API/"]
COPY ["Services/Meetings/Meetings.Models/Meetings.Models.csproj", "Services/Meetings/Meetings.Models/"]
RUN dotnet restore "Services/Meetings/Meetings.API/Meetings.API.csproj"
COPY . .
WORKDIR "/src/Services/Meetings/Meetings.API"
RUN dotnet build "Meetings.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Meetings.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Meetings.API.dll"]