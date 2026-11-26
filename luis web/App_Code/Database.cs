using MySqlConnector;

public static class Database
{
    public const string ConnectionString = "server=127.0.0.1;port=3306;database=sgl;user id=root;password=;";

    public static MySqlConnection CreateConnection()
    {
        return new MySqlConnection(ConnectionString);
    }
}
