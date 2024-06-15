#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ProductService/Product.API/Product.API.csproj", "ProductService/Product.API/"]
COPY ["ProductService/Product.Infrastructure/Product.Infrastructure.csproj", "ProductService/Product.Infrastructure/"]
COPY ["ProductService/Product.Service/Product.Service.csproj", "ProductService/Product.Service/"]
COPY ["Shared/Paginate/Paginate.csproj", "Shared/Paginate/"]
COPY ["ProductService/Product.Data/Product.Data.csproj", "ProductService/Product.Data/"]
COPY ["Shared/DataHelper/DataHelper.csproj", "Shared/DataHelper/"]
COPY ["Shared/Common/Common.csproj", "Shared/Common/"]
COPY ["ProductService/Product.Domain/Product.Domain.csproj", "ProductService/Product.Domain/"]
RUN dotnet restore "ProductService/Product.API/Product.API.csproj"
COPY . .
WORKDIR "/src/ProductService/Product.API"
RUN dotnet build "Product.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Product.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Product.API.dll"]
