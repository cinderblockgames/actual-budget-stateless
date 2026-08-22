FROM mcr.microsoft.com/dotnet/sdk:10.0 as build-env

WORKDIR /app
COPY ./ActualBudgetStateless ./
RUN dotnet restore

WORKDIR /app/ABS
RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app
COPY --from=build-env /app/ABS/out .

RUN apt-get update && \
    apt-get install -y dumb-init
    

# env variables go here


ENTRYPOINT ["/usr/bin/dumb-init", "--"]
CMD [ "dotnet", "/app/ABS.dll" ]