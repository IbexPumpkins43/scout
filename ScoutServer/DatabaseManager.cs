using System.Runtime.CompilerServices;
using Microsoft.Data.Sqlite;

namespace ScoutServer;

public static class DatabaseManager
{
    private static SqliteConnection? _connection;
    public static SqliteConnection Connection
    {
        get
        {
            if (_connection == null)
            {
                throw new DatabaseException("Open the database connection before trying anything");
            }

            return _connection;
        }
        set => _connection = value;
    }

    public static void Open()
    {
        // Open the database
        Connection = new("Data Source=/home/ptarmigan/Git/scout/ScoutServer/Data/Scout.db");
        Connection.Open();

        // Load & run the schema
        string schema = File.ReadAllText("/home/ptarmigan/Git/scout/ScoutServer/Schema.sql");

        using SqliteCommand command = Connection.CreateCommand();
        command.CommandText = schema;
        command.ExecuteNonQuery();
    }
}
