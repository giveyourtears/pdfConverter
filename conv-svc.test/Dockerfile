#FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src

COPY ["./shared", "shared"]
COPY ["./ConvSvc/conv-svc.test", "ConvSvc/conv-svc.test"]
COPY ["./ConvSvc/conv-svc", "ConvSvc/conv-svc"]
COPY ["./UmkClasses/umk-classes", "UmkClasses/umk-classes"]
COPY ["./Metrics/metrics", "Metrics/metrics"]
COPY ["./snk", "snk"]

RUN dotnet restore "ConvSvc/conv-svc.test/conv-svc.test.csproj"

RUN dotnet build /nowarn:CS8002 "ConvSvc/conv-svc.test/conv-svc.test.csproj" -o /app

FROM build AS publish
RUN dotnet publish /nowarn:CS8002 "ConvSvc/conv-svc.test/conv-svc.test.csproj" -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "conv-svc.test.dll"]
#ENTRYPOINT ["/bin/bash"]
