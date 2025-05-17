# 使用 SDK 做建置階段
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# 複製專案檔並還原相依套件
COPY *.csproj ./
RUN dotnet restore

# 複製整個專案並建置
COPY . ./
RUN dotnet publish -c Release -o out

# 用 Runtime image 執行
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# 曝露 port（Render 預設支援的 port 是 $PORT）
ENV ASPNETCORE_URLS=http://+:$PORT
EXPOSE 80

ENTRYPOINT ["dotnet", "VocabularyApp.dll"]
