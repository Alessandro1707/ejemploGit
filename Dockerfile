FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copia los archivos del proyecto y restaura las dependencias
COPY ["miweb/miweb.csproj", "miweb/"]
RUN dotnet restore "miweb/miweb.csproj"

# Copia todo el contenido al contenedor
COPY . .

# Se para dentro de la carpeta miweb para compilar
WORKDIR "/src/miweb"
RUN dotnet build "miweb.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "miweb.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Configura el entorno de ejecución final con .NET 10
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "miweb.dll"]