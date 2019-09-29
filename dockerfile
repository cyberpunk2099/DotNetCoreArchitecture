# .NET Core SDK
FROM mcr.microsoft.com/dotnet/core/sdk:3.0-alpine AS dotnetcore-sdk

WORKDIR /source

# Copy Projects
COPY source/Application/DotNetCoreArchitecture.Application.csproj ./Application/
COPY source/CrossCutting/DotNetCoreArchitecture.CrossCutting.csproj ./CrossCutting/
COPY source/Database/DotNetCoreArchitecture.Database.csproj ./Database/
COPY source/Infra/DotNetCoreArchitecture.Infra.csproj ./Infra/
COPY source/Domain/DotNetCoreArchitecture.Domain.csproj ./Domain/
COPY source/Model/DotNetCoreArchitecture.Model.csproj ./Model/
COPY source/Web/DotNetCoreArchitecture.Web.csproj ./Web/

# .NET Core Restore
RUN dotnet restore ./Web/DotNetCoreArchitecture.Web.csproj

# Copy All Files
COPY source .

# .NET Core Build and Publish
FROM dotnetcore-sdk AS dotnetcore-build
RUN dotnet publish ./Web/DotNetCoreArchitecture.Web.csproj -c Release -o /publish

# Angular
FROM node:12.2.0-alpine AS angular-build
ARG ANGULAR_ENVIRONMENT
WORKDIR /frontend
ENV PATH /frontend/node_modules/.bin:$PATH
COPY source/Web/Frontend/package.json .
RUN npm run restore
COPY source/Web/Frontend .
RUN npm run $ANGULAR_ENVIRONMENT

# ASP.NET Core Runtime
FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-alpine AS aspnetcore-runtime
ARG ASPNETCORE_ENVIRONMENT
ENV ASPNETCORE_ENVIRONMENT=$ASPNETCORE_ENVIRONMENT

RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

WORKDIR /app
COPY --from=dotnetcore-build /publish .
COPY --from=angular-build /frontend/dist ./Frontend/dist
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "DotNetCoreArchitecture.Web.dll"]
