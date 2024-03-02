using MsSqlLoadTesting;
using NBomber.CSharp;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddNLog());
ILogger logger = factory.CreateLogger("Program");
var msSqlHelper = new MsSqlHelper(MsSqlHelper.ConnectionStringRegular, logger: logger);

var insertScenario = Scenario.Create("ms_sql_inserts", async context =>
    {
        try
        {
            await msSqlHelper.CreateRowInTransaction();

            return Response.Ok(statusCode: "ok");
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
var readScenario = Scenario.Create("ms_sql_read", async context =>
    {
        try
        {
            await msSqlHelper.JustQueryRow();

            return Response.Ok(statusCode: "ok");
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
var updateFirstNScenario = Scenario.Create("ms_sql_updates", async context =>
    {
        var attempts = 3;
        string lastExceptionMessage = null;
        int? id = null;
        while (attempts > 0)
        {
            try
            {
                id = await msSqlHelper.UpdateRow(idParam: id, maxId: 100);
                return Response.Ok(statusCode: "ok", sizeBytes: 1);
            }
            catch (Exception ex)
            {
                logger.LogError(exception: ex, message: $"attempt={4 - attempts}");
                attempts--;
                lastExceptionMessage = ex.Message;
            }
        }

        return Response.Fail(statusCode: "fail", lastExceptionMessage);
    })
    .WithLoadSimulations(
        Simulation.Inject(rate: 10,
            interval: TimeSpan.FromSeconds(1),
            during: TimeSpan.FromSeconds(30))
    );

NBomberRunner
    .RegisterScenarios(updateFirstNScenario)
    .Run();
