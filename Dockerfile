FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /app

COPY . .

ENV PATH="$PATH:/root/.dotnet/tools/"

CMD dotnet watch --project app
