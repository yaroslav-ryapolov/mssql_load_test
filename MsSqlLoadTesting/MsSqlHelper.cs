using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MsSqlLoadTesting;

public class MsSqlHelper(string connectionString, IsolationLevel transactionIsolationLevel = IsolationLevel.ReadCommitted, ILogger? logger = null)
{
    public const string ConnectionStringRegular = "Data Source=localhost;Initial Catalog=test_load;User Id=cpu_limited;Password=TheStrongPassword123;TrustServerCertificate=True;Max Pool Size=15000;";
    public const string ConnectionStringRegularAdmin = "Data Source=localhost;Initial Catalog=test_load;User Id=admin;Password=TheStrongPassword123;TrustServerCertificate=True;Max Pool Size=15000;";
    public const string ConnectionStringInMemory = "Data Source=localhost;Initial Catalog=test_load_in_memory;User Id=cpu_limited;Password=TheStrongPassword123;TrustServerCertificate=True;Max Pool Size=15000;";
    public const string ConnectionStringInMemoryAdmin = "Data Source=localhost;Initial Catalog=test_load_in_memory;User Id=admin;Password=TheStrongPassword123;TrustServerCertificate=True;Max Pool Size=15000;";

    private readonly Random _random = new((int)(DateTime.UtcNow.Ticks % int.MaxValue));

    public async Task CreateRowInTransaction(bool useTabLock = false)
    {
        await using SqlConnection connection = new(connectionString);
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

    public async Task<int> JustQueryRow(int? idParam = null, int maxId = 1_000_000)
    {
        await using SqlConnection connection = new(connectionString);
        await connection.OpenAsync();

        var transaction = await connection.BeginTransactionAsync(transactionIsolationLevel) as SqlTransaction;

        var id = idParam ?? _random.Next(maxId);
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

        return i;
    }

    public async Task<int> UpdateRow(int maxId = 1_000_000, int? idParam = null)
    {
        await using SqlConnection connection = new(connectionString);
        await connection.OpenAsync();

        var id = idParam ?? _random.Next(maxId);
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
        return $"the name {_random.Next(1_000_000)}";
    }

    private string GetData()
    {
        const string chars = "$%#@!*abcdefghijklmnopqrstuvwxyz1234567890?;:ABCDEFGHIJKLMNOPQRSTUVWXYZ^&";
        StringBuilder result = new StringBuilder();
        for (int i = 0; i < 30; i++)
        {
            result.Append(chars[_random.Next(0, chars.Length - 1)]);
        }

        return result.ToString();
    }
}