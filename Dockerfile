FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /app

COPY . .

RUN dotnet tool install --global dotnet-ef

ENV PATH="$PATH:/root/.dotnet/tools/"

CMD dotnet watch --project app
