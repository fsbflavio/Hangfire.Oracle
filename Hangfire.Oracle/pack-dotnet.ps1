# Script to pack Hangfire.Oracle
# usage: pack-dotnet.ps1 version ex: pack-dotnet.ps1 1.8.14
dotnet pack Hangfire.Oracle.csproj --configuration Release /p:PackageVersion=$Args