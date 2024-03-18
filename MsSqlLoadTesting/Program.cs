using MsSqlLoadTesting;
using NBomber.CSharp;
using Microsoft.Extensions.Logging;
using NBomber.Contracts;
using NDesk.Options;
using NLog.Extensions.Logging;

const int defaultInitRows = 1_000;

bool isInMemory = false;
bool forceTableReCreate = false;
var scenarios = new List<string> ();
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
        "s|scenarios=", "one of scenario to run",
        s => scenarios.Add(s.ToLower())
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
    Console.WriteLine($"scenarios: {string.Join(",", scenarios)}");
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

// todo: check names in init section
var nameToScenario = new Dictionary<string, ScenarioProps>()
{
    { "insert", scenariosHelper.GetInsertScenario(copies: 1_000) },
    { "update", scenariosHelper.GetUpdateScenario(copies: 1_000, maxId: 100) },
    { "read", scenariosHelper.GetReadScenario(copies: 5_000, maxId: defaultInitRows) },
};

NBomberRunner
    .RegisterScenarios(scenarios.Select(s => nameToScenario[s]).ToArray())
    .Run();
