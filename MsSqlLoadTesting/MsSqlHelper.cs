using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;

namespace MsSqlLoadTesting;

public static class MsSqlHelper
{
    private const string ConnectionString = "Data Source=localhost;Initial Catalog=test_load;User Id=cpu_limited;Password=TheStrongPassword123;TrustServerCertificate=True;Max Pool Size=15000;";

    public static async Task<int> JustCreateRow(Random random)
    {
        string nameValue = $"the name {random.Next(1_000_000)}";
        string dataValue = String.Join(
            string.Empty,
            Enumerable.Range(0, random.Next(1_000)).Select(_ => "a"));

        await using SqlConnection connection = new(ConnectionString);
        await connection.OpenAsync();

        var transaction = await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted) as SqlTransaction;

        SqlCommand insertCommand = new(
            cmdText: "INSERT INTO dbo.test WITH(TABLOCKX) (Name, Data) VALUES (@name, @data)",
            connection,
            transaction);
        insertCommand.Parameters.AddWithValue("@name", nameValue);
        insertCommand.Parameters.AddWithValue("@data", dataValue);

        SqlCommand queryCommand = new(
            cmdText: "SELECT * FROM dbo.test WHERE Id = @id",
            connection,
            transaction);
        queryCommand.Parameters.AddWithValue("@id", random.Next(1_000_000));

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
        catch (Exception)
        {
            // ignored
        }

        var result = await insertCommand.ExecuteNonQueryAsync() + i;
        await transaction!.CommitAsync();
        return result;
    }

    public static async Task<int> JustUpdateRow(Random random)
    {
        string nameValue = $"the name {random.Next(1_000_000)}";
        string dataValue = String.Join(
            string.Empty,
            Enumerable.Range(0, random.Next(1_000)).Select(_ => "b"));

        await using SqlConnection connection = new(ConnectionString);
        await connection.OpenAsync();

        var transaction = await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted) as SqlTransaction;

        SqlCommand updateCommand = new(
            cmdText: "UPDATE dbo.test SET  Name = @name, Data = @data WHERE Id = @id",
            connection,
            transaction);
        updateCommand.Parameters.AddWithValue("@id", random.Next(1_000_000));
        updateCommand.Parameters.AddWithValue("@name", nameValue);
        updateCommand.Parameters.AddWithValue("@data", dataValue);

        SqlCommand queryCommand = new(
            cmdText: "SELECT * FROM dbo.test WHERE Id = @id",
            connection,
            transaction);
        queryCommand.Parameters.AddWithValue("@id", random.Next(1_000_000));

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
        catch (Exception)
        {
            // ignored
        }

        var result = await updateCommand.ExecuteNonQueryAsync() + i;
        await transaction!.CommitAsync();
        return result;
    }

    public static async Task<int> JustQueryRow(Random random)
    {
        await using SqlConnection connection = new(ConnectionString);
        await connection.OpenAsync();

        var transaction = await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted) as SqlTransaction;

        SqlCommand queryCommand = new(
            cmdText: "SELECT * FROM dbo.test WHERE Id = @id",
            connection,
            transaction);
        queryCommand.Parameters.AddWithValue("@id", random.Next(1_000_000));

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
        catch (Exception)
        {
            // ignored
        }

        await transaction!.CommitAsync();

        return i;
    }
}