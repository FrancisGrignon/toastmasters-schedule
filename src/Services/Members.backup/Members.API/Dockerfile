#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM microsoft/dotnet:2.2-aspnetcore-runtime-nanoserver-1803 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk-nanoserver-1803 AS build
WORKDIR /src
COPY ["Members.API/Members.API.csproj", "Members.API/"]
COPY ["Members.DataAccess.Models/Members.DataAccess.Models.csproj", "Members.DataAccess.Models/"]
COPY ["Members.DataAccess/Members.DataAccess.csproj", "Members.DataAccess/"]
RUN dotnet restore "Members.API/Members.API.csproj"
COPY . .
WORKDIR "/src/Members.API"
RUN dotnet build "Members.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Members.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Members.API.dll"]