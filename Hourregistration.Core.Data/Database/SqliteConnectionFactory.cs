using Microsoft.Data.Sqlite;

namespace Hourregistration.Core.Data.Database
{
    public interface ISqliteConnectionFactory
    {
        Task<SqliteConnection> CreateOpenConnectionAsync();
    }

    public class SqliteConnectionFactory : ISqliteConnectionFactory
    {
        private readonly string _connectionString;

        public SqliteConnectionFactory(string dbPath)
        {
            _connectionString = new SqliteConnectionStringBuilder
            {
                DataSource = dbPath,
                Mode = SqliteOpenMode.ReadWriteCreate,
                ForeignKeys = true
            }.ToString();
        }

        public async Task<SqliteConnection> CreateOpenConnectionAsync()
        {
            var conn = new SqliteConnection(_connectionString);
            await conn.OpenAsync();
            return conn;
        }
    }
}