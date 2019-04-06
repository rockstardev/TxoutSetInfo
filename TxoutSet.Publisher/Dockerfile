FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY TxoutSet.Publisher/TxoutSet.Publisher.csproj TxoutSet.Publisher/
RUN dotnet restore TxoutSet.Publisher/TxoutSet.Publisher.csproj
COPY . .
WORKDIR /src/TxoutSet.Publisher
RUN dotnet build TxoutSet.Publisher.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish TxoutSet.Publisher.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TxoutSet.Publisher.dll"]
