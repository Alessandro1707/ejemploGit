FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Copia el archivo del proyecto y restaura dependencias
# (Ojo: si tu carpeta se llama diferente a 'miweb', cambia esa palabra por el nombre real de tu carpeta)
COPY ["miweb/miweb.csproj", "miweb/"]
RUN dotnet restore "miweb/miweb.csproj"

# 2. Copia todo el código restante y compila
COPY . .
WORKDIR "/src/miweb"
RUN dotnet build "miweb.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "miweb.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 3. Configura el entorno de ejecución final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "miweb.dll"]