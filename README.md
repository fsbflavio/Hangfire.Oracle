# Hangfire.Oracle - Oracle Storage for Hangfire Background Jobs
This repository contains an Oracle implementation of Hangfire for running fire-and-forget, delayed and recurring background jobs in .NET.

It **supports both .NET Core and .NET Framework** applications.

Hangfire.Oracle was originally forked from [Hangfire.Oracle.Core](https://github.com/akoylu/Hangfire.Oracle.Core), which is no longer maintained. This project aims to provide an up-to-date Oracle storage option for Hangfire with support for recent .NET versions.

Changes include:
- Removed Dapper.Oracle dependency
- Upgraded packages
- Added support for .NET 6.0
- General maintenance and improvements

Hangfire.Oracle keeps parity with the main Hangfire project as much as possible.

[![Latest version](https://img.shields.io/nuget/v/Th.Hangfire.Oracle.svg)](https://www.nuget.org/packages/TH.Hangfire.Oracle/1.8.4)

Oracle storage implementation of [Hangfire](http://hangfire.io/) - fire-and-forget, delayed and recurring tasks runner for .NET. Scalable and reliable background job runner. Supports multiple servers, CPU and I/O intensive, long-running and short-running jobs.

**Some features of Oracle storage implementation is under development!**

## Installation
Install Oracle

Run the following command in the NuGet Package Manager console to install Hangfire.Oracle.Core:

```
Install-Package TH.Hangfire.Oracle
```

## Contributing
If you are interested in contributing, please open a pull request! All improvements are appreciated.

## Usage

Use one the following ways to initialize `OracleStorage`: 
- Create new instance of `OracleStorage` with connection string constructor parameter and pass it to `Configuration` with `UseStorage` method:
```csharp
  GlobalConfiguration.Configuration.UseStorage(
    new OracleStorage(connectionString));
```
- Alternatively one or more options can be passed as a parameter to `OracleStorage`:
```csharp
GlobalConfiguration.Configuration.UseStorage(
    new OracleStorage(
        connectionString, 
        new OracleStorageOptions
        {
            TransactionIsolationLevel = IsolationLevel.ReadCommitted,
            QueuePollInterval = TimeSpan.FromSeconds(15),
            JobExpirationCheckInterval = TimeSpan.FromHours(1),
            CountersAggregateInterval = TimeSpan.FromMinutes(5),
            PrepareSchemaIfNecessary = true,
            DashboardJobListLimit = 50000,
            TransactionTimeout = TimeSpan.FromMinutes(1),
            SchemaName = "HANGFIRE"
        }));
```
- With version 1.1 you can provide your own connection factory.
```csharp
GlobalConfiguration.Configuration.UseStorage(
    new OracleStorage(
        () => new OracleConnection(connectionString), 
        new OracleStorageOptions
        {
            SchemaName = "HANGFIRE"
        }));
```
Description of optional parameters:
- `TransactionIsolationLevel` - transaction isolation level. Default is read committed. Didn't test with other options!
- `QueuePollInterval` - job queue polling interval. Default is 15 seconds.
- `JobExpirationCheckInterval` - job expiration check interval (manages expired records). Default is 1 hour.
- `CountersAggregateInterval` - interval to aggregate counter. Default is 5 minutes.
- `PrepareSchemaIfNecessary` - if set to `true`, it creates database tables. Default is `true`.
- `DashboardJobListLimit` - dashboard job list limit. Default is 50000.
- `TransactionTimeout` - transaction timeout. Default is 1 minute.
- `SchemaName` - schema name. Default is empty

### How to limit number of open connections

Number of opened connections depends on Hangfire worker count. You can limit worker count by setting `WorkerCount` property value in `BackgroundJobServerOptions`:
```csharp
app.UseHangfireServer(
   new BackgroundJobServerOptions
   {
      WorkerCount = 1
   });
```

More info: http://hangfire.io/features.html#concurrency-level-control

## Dashboard
Hangfire provides a dashboard
![Dashboard](https://camo.githubusercontent.com/f263ab4060a09e4375cc4197fb5bfe2afcacfc20/687474703a2f2f68616e67666972652e696f2f696d672f75692f64617368626f6172642d736d2e706e67)  
More info: [Hangfire Overview](http://hangfire.io/overview.html#integrated-monitoring-ui)

## Build
Please use Visual Studio or any other tool of your choice to build the solution.

## Known Issues
If the DB User/Schema does not exist, Install.sql is not deployed. As a workaround for this issue, you can create the User/Schema in the database and give CRUD grants to it. When Hangfire runs for the first time, it will create the tables for the User/Schema specified in the connection string.
