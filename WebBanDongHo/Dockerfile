FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["WebBanDongHo.csproj", "./"]
RUN dotnet restore "WebBanDongHo.csproj"
COPY . .
RUN dotnet publish "WebBanDongHo.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WebBanDongHo.dll"]
