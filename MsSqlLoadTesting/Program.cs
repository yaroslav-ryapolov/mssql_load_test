using MsSqlLoadTesting;
using NBomber.CSharp;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddNLog());
ILogger logger = factory.CreateLogger("Program");
var msSqlHelper = new MsSqlHelper(inMemory: false, logger: logger);
var scenariosHelper = new ScenariosHelper(msSqlHelper, initDuration: TimeSpan.FromMinutes(3), logger);

await msSqlHelper.EnsureTable(rowCount: 1_000, forceReCreate: true);

NBomberRunner
    .RegisterScenarios(
        scenariosHelper.GetUpdateScenario(1_000, 100)
        // ,scenariosHelper.GetReadScenario(10_000)
    )
    .Run();
