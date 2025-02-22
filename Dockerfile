# Создаем слой из базового образа (c докерхаюа)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Устанавливаем внутри контейнера базовую директорию (создаем директорию)
WORKDIR  /src

# Копируем нужные файлы (зависимости проекта) с хоста в рабочую директорию будущего контейнера. / - означает папка, "" - нужно использовать, есди в пути есть пробелы
# 1-ое - это источник, 2-ое куда скопировать файл (обязательно указывать полный путь из рабочей директори)
COPY ["./src/OzonEdu.StockApi/OzonEdu.StockApi.csproj", "src/OzonEdu.StockApi/"]

# Так как в контейнере уже установлен sdk, то значит есть утилита dotnet, а у нее есть комнада restore,  команда будет выполняться и восстанавливать зависимости проекта
# (обязательно указывать полный путь из рабочей директори)
RUN dotnet restore "src/OzonEdu.StockApi/OzonEdu.StockApi.csproj"

# копируем все из рабочей директирии хоста в рабочую директорию контейнера
COPY . . 
# изменили рабочую директорию проекта, теперь нам надо рабоатть из папки, где лежат все файды для сборки проекта
WORKDIR  "/src/src/OzonEdu.StockApi"

# Запускаем команду для билдинга проекта. В конце указываем директорию, куда уходит наш билд
RUN dotnet build "OzonEdu.StockApi.csproj" -c Release -o /app/build

FROM build AS publish
# Теперь опубликуем наш билд (это нужно для того, чтобы в новом образе был только опубликованный прект)
RUN dotnet publish "OzonEdu.StockApi.csproj" -c Release -o /app/publish

# Используем официальный образ ASP.NET Core Runtime (без SDK)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

WORKDIR /app

# Открываем порты 80 (HTTP) и 443 (HTTPS) внутри контейнера
EXPOSE 80
EXPOSE 443

FROM runtime AS final
WORKDIR /app

# Копируем из прошлого образа из папки /app/publish в текущую директорию нового образа (/app)
COPY --from=publish /app/publish .

# Задает точку исполления приложения (приложения на .NET на самом деле dll, там нет исполняемого файла), чтобы запустить приложение, нужно написать dotnet и сформированную dll
ENTRYPOINT [ "dotnet", "OzonEdu.StockApi.dll" ]


