using MsSqlLoadTesting;
using NBomber.CSharp;
using Microsoft.Extensions.Logging;
using NDesk.Options;
using NLog.Extensions.Logging;

// bool isInMemory = false;
// bool forceTableReCreate = false;
// List<string> scenarios = new List<string> ();
// int durationInMinutes = 1;
//
// var p = new OptionSet () {
//     { "n|name=", "the {NAME} of someone to greet.",
//         v => names.Add (v) },
// };
//
// List<string> extra;
// try {
//     extra = p.Parse (args);
// }
// catch (OptionException e) {
//     Console.Write ("greet: ");
//     Console.WriteLine (e.Message);
//     Console.WriteLine ("Try `greet --help' for more information.");
//     return;
// }

using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddNLog());
ILogger logger = factory.CreateLogger("Program");
var msSqlHelper = new MsSqlHelper(inMemory: true, logger: logger);
var scenariosHelper = new ScenariosHelper(msSqlHelper, initDuration: TimeSpan.FromMinutes(3), logger);

await msSqlHelper.EnsureTable(rowCount: 1_000, forceReCreate: false);

NBomberRunner
    .RegisterScenarios(
        scenariosHelper.GetUpdateScenario(10_000, 100)
        // ,scenariosHelper.GetReadScenario(10_000)
    )
    .Run();
