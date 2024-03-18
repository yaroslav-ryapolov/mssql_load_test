using Microsoft.Extensions.Logging;
using NBomber.Contracts;
using NBomber.CSharp;

namespace MsSqlLoadTesting;

public class ScenariosHelper(MsSqlHelper msSqlHelper, TimeSpan? initDuration = null, ILogger? logger = null)
{
    private TimeSpan Duration { get; } = initDuration ?? TimeSpan.FromMinutes(3);

    public ScenarioProps GetInsertScenario(int copies)
    {
        var result = Scenario.Create("ms_sql_inserts", async context =>
            {
                try
                {
                    await msSqlHelper.CreateRowInTransaction();

                    return Response.Ok(statusCode: "ok");
                }
                catch (Exception ex)
                {
                    logger?.LogError(exception: ex, message: "during row creation");
                    throw;
                }
            });
        return this.KeepConstant(result, copies);
    }

    public ScenarioProps GetReadScenario(int copies, int maxId)
    {
        var result = Scenario.Create("ms_sql_reads", async context =>
            {
                try
                {
                    await msSqlHelper.JustQueryRow(maxId);

                    return Response.Ok(statusCode: "ok");
                }
                catch (Exception ex)
                {
                    logger?.LogError(exception: ex, message: "during reading");
                    throw;
                }
            });
        return this.KeepConstant(result, copies);
    }

    public ScenarioProps GetUpdateScenario(int copies, int maxId)
    {
        const int maxAttempts = 3;
        var result = Scenario.Create("ms_sql_updates", async context =>
            {
                var attempts = maxAttempts;
                string? lastExceptionMessage = null;
                int? id = null;
                while (attempts > 0)
                {
                    try
                    {
                        id = await msSqlHelper.UpdateRow(idParam: id, maxId);
                        return Response.Ok(statusCode: "ok");
                    }
                    catch (Exception ex)
                    {
                        logger?.LogError(exception: ex, message: "{Attempts}", maxAttempts + 1 - attempts);
                        attempts--;
                        lastExceptionMessage = ex.Message;
                    }
                }

                return Response.Fail(statusCode: "fail", lastExceptionMessage);
            });
        return this.KeepConstant(result, copies);
    }

    private ScenarioProps KeepConstant(ScenarioProps scenario, int copies)
    {
        return scenario
            .WithMaxFailCount(int.MaxValue)
            .WithLoadSimulations(
                Simulation.KeepConstant(
                    copies: copies,
                    during: Duration)
            );
    }
}