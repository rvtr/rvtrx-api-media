# stage - base
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as base

WORKDIR /workspace

COPY . .

RUN dotnet restore
RUN dotnet build --no-restore
RUN dotnet publish --configuration Debug --output out --no-build RVTR.Media.Service/*.csproj

# stage - final
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

WORKDIR /workspace

COPY --from=base /workspace/out /workspace

CMD [ "dotnet", "RVTR.Media.Service.dll" ]
