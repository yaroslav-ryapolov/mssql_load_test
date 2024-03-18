using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MsSqlLoadTesting;

public class MsSqlHelper(bool inMemory, IsolationLevel transactionIsolationLevel = IsolationLevel.ReadCommitted, string? dbHost = null, string? dbName = null, string? dbUser = null, string? dbPassword = null, ILogger? logger = null)
{
    private const string ConnectionStringRegular = "Data Source=130.193.51.89;Initial Catalog=test_load;User Id=sa;Password=TheStrongPassword123;TrustServerCertificate=True;Max Pool Size=15000;";
    private const string ConnectionStringInMemory = "Data Source=130.193.51.89;Initial Catalog=test_load_in_memory;User Id=sa;Password=TheStrongPassword123;TrustServerCertificate=True;Max Pool Size=15000;";

    private readonly ThreadLocal<Random> _random = new(() => new Random(Seed: (int)(DateTime.UtcNow.Ticks % int.MaxValue)));

    private string ConnectionString
    {
        get
        {
            if (string.IsNullOrWhiteSpace(dbHost))
            {
                return inMemory ? ConnectionStringInMemory : ConnectionStringRegular;
            }

            return $"Data Source={dbHost};Initial Catalog={dbName};User Id={dbUser};Password={dbPassword};TrustServerCertificate=True;Max Pool Size=15000;";
        }
    }

    private Random Random => _random.Value!;

    public async Task EnsureTable(int rowCount, bool forceReCreate)
    {
        await using SqlConnection connection = new(ConnectionString);
        await connection.OpenAsync();

        SqlCommand checkTableCommand = new(
            cmdText: @"
                SELECT COUNT(*) 
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_SCHEMA = 'dbo' 
                AND  TABLE_NAME = 'test'
            ",
            connection);
        var isTableExists = (int)((await checkTableCommand.ExecuteScalarAsync()) ?? 0) > 0;
        if (!forceReCreate && isTableExists)
        {
            return;
        }

        SqlCommand dropCommands = new(
            cmdText: @"
                DROP TABLE IF EXISTS dbo.test;
                DROP SEQUENCE IF EXISTS dbo.test_seq;
            ",
            connection);
        await dropCommands.ExecuteNonQueryAsync();

        SqlCommand createCommands = new(
            cmdText: inMemory
                ?
                    @"
                        CREATE SEQUENCE dbo.test_seq AS BIGINT START WITH 0;

                        CREATE TABLE [dbo].[test](
                            [Id] [bigint] NOT NULL,
                            [Name] [nvarchar](50) NOT NULL,
                            [Data] [nvarchar](max) NULL,
                            [Version] [int] NOT NULL DEFAULT (0),

                            CONSTRAINT [PK_test] PRIMARY KEY NONCLUSTERED HASH
                            (
                                [Id]
                            ) WITH (BUCKET_COUNT=1000000)
                        ) WITH (MEMORY_OPTIMIZED=ON, DURABILITY = SCHEMA_ONLY);
                    "
                :
                    @"
                        CREATE SEQUENCE dbo.test_seq AS BIGINT START WITH 0;

                        CREATE TABLE [dbo].[test](
                            [Id] [bigint] NOT NULL PRIMARY KEY CLUSTERED,
                            [Name] [nvarchar](50) NOT NULL,
                            [Data] [nvarchar](max) NULL,
                            [Version] [int] NOT NULL DEFAULT (0),
                        );
                    ",
            connection);
        await createCommands.ExecuteNonQueryAsync();

        for (int i = 0; i < rowCount; i++)
        {
            SqlCommand insertCommand = new(
                cmdText: "INSERT INTO dbo.test (Id, Name, Data) VALUES (NEXT VALUE FOR dbo.test_seq, @name, @data);",
                connection);
            insertCommand.Parameters.AddWithValue("@name", this.GetName());
            insertCommand.Parameters.AddWithValue("@data", this.GetData());

            await insertCommand.ExecuteNonQueryAsync();
        }
    }

    public async Task CreateRowInTransaction(bool useTabLock = false)
    {
        await using SqlConnection connection = new(ConnectionString);
        await connection.OpenAsync();

        var transaction = await connection.BeginTransactionAsync(transactionIsolationLevel) as SqlTransaction;

        SqlCommand insertCommand = new(
            cmdText: useTabLock
                ? "INSERT INTO dbo.test WITH(TABLOCKX) (Name, Data) VALUES (@name, @data)"
                : "INSERT INTO dbo.test (Name, Data) VALUES (@name, @data)",
            connection,
            transaction);
        insertCommand.Parameters.AddWithValue("@name", this.GetName());
        insertCommand.Parameters.AddWithValue("@data", this.GetData());

        await insertCommand.ExecuteNonQueryAsync();
        await transaction!.CommitAsync();
    }

    public async Task JustQueryRow(int? idParam = null, int maxId = 1_000_000)
    {
        await using SqlConnection connection = new(ConnectionString);
        await connection.OpenAsync();

        var transaction = await connection.BeginTransactionAsync(transactionIsolationLevel) as SqlTransaction;

        var id = idParam ?? Random.Next(maxId);
        SqlCommand queryCommand = new(
            cmdText: "SELECT * FROM dbo.test WHERE Id = @id",
            connection,
            transaction);
        queryCommand.Parameters.AddWithValue("@id", id);

        int i = 0;
        try
        {
            connection.Open();
            SqlDataReader reader = await queryCommand.ExecuteReaderAsync();
            while (reader.Read())
            {
                i++;
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            logger?.LogError(exception: ex, message: "issue while query data");
        }

        await transaction!.CommitAsync();
    }

    public async Task<int> UpdateRow(int? idParam = null, int maxId = 1_000_000)
    {
        await using SqlConnection connection = new(ConnectionString);
        await connection.OpenAsync();

        var id = idParam ?? Random.Next(maxId);
        SqlCommand updateCommand = new(
            cmdText: "UPDATE dbo.test SET [Name] = @name, [Data] = @data, [Version] = [Version] + 1 WHERE Id = @id",
            connection);
        updateCommand.Parameters.AddWithValue("@id", id);
        updateCommand.Parameters.AddWithValue("@name", this.GetName());
        updateCommand.Parameters.AddWithValue("@data", this.GetData());

        await updateCommand.ExecuteNonQueryAsync();
        return id;
    }

    private string GetName()
    {
        return $"the name {Random.Next(1_000_000)}";
    }

    private string GetData()
    {
        const string chars = "$%#@!*abcdefghijklmnopqrstuvwxyz1234567890?;:ABCDEFGHIJKLMNOPQRSTUVWXYZ^&";
        StringBuilder result = new StringBuilder();
        for (int i = 0; i < 30; i++)
        {
            result.Append(chars[Random.Next(0, chars.Length - 1)]);
        }

        return result.ToString();
    }
}