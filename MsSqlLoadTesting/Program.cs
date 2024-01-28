using Microsoft.Extensions.Configuration;
using MsSqlLoadTesting;
using NBomber.CSharp;
using Microsoft.Extensions.Logging;
using NBomber.Contracts;
using NLog.Extensions.Logging;

using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddNLog());
ILogger logger = factory.CreateLogger("Program");

var random = new Random();

// await MsSqlHelper.JustCreateRow(random);
// await MsSqlHelper.JustQueryRow(random);

var insertScenario = Scenario.Create("ms_sql_inserts", async context =>
    {
        try
        {
            var createdAndReadCount = await MsSqlHelper.JustCreateRow(random);

            return Response.Ok(statusCode: "ok", sizeBytes: createdAndReadCount);
        }
        catch (Exception ex)
        {
            logger.LogError(exception: ex, message: null);
            throw;
        }
    })
    .WithLoadSimulations(
        Simulation.Inject(rate: 1,
            interval: TimeSpan.FromSeconds(10),
            during: TimeSpan.FromMinutes(3))
    );
var updateScenario = Scenario.Create("ms_sql_updates", async context =>
    {
        try
        {
            var createdAndReadCount = await MsSqlHelper.JustUpdateRow(random);

            return Response.Ok(statusCode: "ok", sizeBytes: createdAndReadCount);
        }
        catch (Exception ex)
        {
            logger.LogError(exception: ex, message: null);
            throw;
        }
    })
    .WithLoadSimulations(
        Simulation.Inject(rate: 3000,
            interval: TimeSpan.FromSeconds(1),
            during: TimeSpan.FromMinutes(3))
    );
var readScenario = Scenario.Create("ms_sql_read", async context =>
    {
        try
        {
            var createdAndReadCount = await MsSqlHelper.JustQueryRow(random);

            return Response.Ok(statusCode: "ok", sizeBytes: createdAndReadCount);
        }
        catch (Exception ex)
        {
            logger.LogError(exception: ex, message: null);
            throw;
        }
    })
    .WithLoadSimulations(
        Simulation.Inject(rate: 10_000,
            interval: TimeSpan.FromSeconds(5),
            during: TimeSpan.FromMinutes(3))
    );

NBomberRunner
    .RegisterScenarios(insertScenario, updateScenario, readScenario)
    .Run();
