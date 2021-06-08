# stage - base
FROM mcr.microsoft.com/dotnet/sdk:5.0 as base

WORKDIR /workspace

COPY . .

RUN dotnet restore --force-evaluate
RUN dotnet build --no-restore
RUN dotnet publish --configuration Debug --no-build --output out RVTR.Media.Service/*.csproj

# stage - final
FROM mcr.microsoft.com/dotnet/aspnet:5.0

WORKDIR /workspace

COPY --from=base /workspace/out /workspace

CMD [ "dotnet", "RVTR.Media.Service.dll" ]
