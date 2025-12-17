namespace Hourregistration.Core.Data.Database
{
    public class SqliteSchemaMigrator
    {
        private readonly ISqliteConnectionFactory _factory;
        public SqliteSchemaMigrator(ISqliteConnectionFactory factory) => _factory = factory;

        public async Task MigrateAsync()
        {
            using var conn = await _factory.CreateOpenConnectionAsync();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Users (
                user_id INTEGER PRIMARY KEY AUTOINCREMENT,
    
                username TEXT NOT NULL UNIQUE,
                password TEXT NOT NULL,
                role INTEGER NOT NULL,

                is_active INTEGER NOT NULL DEFAULT 1
            );

            CREATE TABLE IF NOT EXISTS Declarations (
                declaration_id INTEGER PRIMARY KEY AUTOINCREMENT,
    
                declaration_date DATE NOT NULL,
                start_time TIME,
                end_time TIME,
    
                worked_hours REAL NOT NULL,
                reason TEXT NOT NULL,
    
                project_name TEXT,
                description TEXT,
                state INTEGER NOT NULL,
    
                user_id INTEGER NOT NULL,

                created_at DATETIME NOT NULL,
                updated_at DATETIME,
    
                FOREIGN KEY(user_id) REFERENCES Users(user_id) ON DELETE NO ACTION
            );

            CREATE TABLE IF NOT EXISTS DraftDeclarations (
                draft_id INTEGER PRIMARY KEY AUTOINCREMENT,
    
                declaration_date DATE NOT NULL,
                start_time TIME,
                end_time TIME,
    
                worked_hours REAL NOT NULL,
                reason TEXT NOT NULL,
    
                project_name TEXT,
                description TEXT,
    
                user_id INTEGER NOT NULL,

                created_at DATETIME NOT NULL,
                updated_at DATETIME,
    
                FOREIGN KEY(user_id) REFERENCES Users(user_id) ON DELETE NO ACTION
            );

            CREATE INDEX IF NOT EXISTS IX_Declarations_UserID
            ON Declarations (user_id);
            ";
            await cmd.ExecuteNonQueryAsync();
        }
    }

}
