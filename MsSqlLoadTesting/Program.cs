using MsSqlLoadTesting;
using NBomber.CSharp;
using Microsoft.Extensions.Logging;
using NBomber.Contracts;
using NDesk.Options;
using NLog.Extensions.Logging;

const int defaultInitRows = 1_000;

bool isInMemory = false;
bool forceTableReCreate = false;
int insertScenarioCopies = 0;
int updateScenarioCopies = 0;
int readScenarioCopies = 0;
int durationInMinutes = 3;

string dbHost = "localhost";
string dbUser = "sa";
string dbPassword = "TheStrongPassword123";
string? dbName = null;

var p = new OptionSet() {
    {
        "m|in-memory=", "run in-memory table test",
        m => bool.TryParse(m, out isInMemory)
    },
    {
        "f|force-recreate=", "force re-creation of table",
        f => bool.TryParse(f, out forceTableReCreate)
    },
    {
        "d|duration=", "duration of loading test in minutes",
        d => int.TryParse(d, out durationInMinutes)
    },
    {
        "is|insert-scenario=", "copies of insert scenario to run",
        @is => int.TryParse(@is, out insertScenarioCopies)
    },
    {
        "us|update-scenario=", "copies of update scenario to run",
        us => int.TryParse(us, out updateScenarioCopies)
    },
    {
        "rs|read-scenario=", "copies of read scenario to run",
        rs => int.TryParse(rs, out readScenarioCopies)
    },
    {
        "h|host=", "db host",
        h => dbHost = h
    },
    {
        "u|user=", "user name for db",
        u => dbUser = u
    },
    {
        "p|password=", "password for db",
        p => dbPassword = p
    },
    {
        "c|initial-catalog=", "database name",
        c => dbName = c
    },
};

List<string> extra;
try {
    extra = p.Parse(args);

    if (string.IsNullOrWhiteSpace(dbName))
    {
        dbName = isInMemory ? "test_load_in_memory" : "test_load";
    }

    Console.WriteLine("The following configuration read");
    Console.WriteLine($"in-memory testing: {isInMemory}");
    Console.WriteLine($"force recreation of table: {forceTableReCreate}");
    Console.WriteLine($"durationInMinutes: {durationInMinutes}");
    Console.WriteLine($"insert scenarios: {insertScenarioCopies}");
    Console.WriteLine($"update scenarios: {updateScenarioCopies}");
    Console.WriteLine($"read scenarios: {readScenarioCopies}");
    Console.WriteLine($"db host: {dbHost}");
    Console.WriteLine($"db user: {dbUser}");
    Console.WriteLine($"db password: {dbPassword}");
    Console.WriteLine($"db name: {dbName}");
    Console.WriteLine($"other: {string.Join(";", extra)}");
}
catch (OptionException e) {
    Console.Write ("ms-sql-loading-testing: ");
    Console.WriteLine (e.Message);
    Console.WriteLine ("Something wrong with parameters.");
    return;
}

using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddNLog());
ILogger logger = factory.CreateLogger("Program");
var msSqlHelper = new MsSqlHelper(inMemory: isInMemory, dbHost: dbHost, dbName: dbName, dbPassword: dbPassword, dbUser: dbUser, logger: logger);
var scenariosHelper = new ScenariosHelper(msSqlHelper, initDuration: TimeSpan.FromMinutes(durationInMinutes), logger);

await msSqlHelper.EnsureTable(rowCount: defaultInitRows, forceReCreate: false);

var scenarios = new List<ScenarioProps>();
if (insertScenarioCopies > 0)
{
    scenarios.Add(scenariosHelper.GetInsertScenario(insertScenarioCopies));
}
if (updateScenarioCopies > 0)
{
    scenarios.Add(scenariosHelper.GetUpdateScenario(updateScenarioCopies, maxId: 100));
}
if (readScenarioCopies > 0)
{
    scenarios.Add(scenariosHelper.GetReadScenario(readScenarioCopies, maxId: defaultInitRows));
}

NBomberRunner
    .RegisterScenarios(scenarios.ToArray())
    .Run();
