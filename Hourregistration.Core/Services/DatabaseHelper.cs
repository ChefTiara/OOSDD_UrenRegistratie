using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;

namespace Hourregistration.Core
{
    public static class DatabaseHelper
    {
        private static string DbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "userdb.db");

        // Initialiseer de database
        public static void InitializeDatabase()
        {
            if (!File.Exists(DbPath))
            {
                using (var connection = new SqliteConnection($"Data Source={DbPath}"))
                {
                    connection.Open();

                    string tableCommand = @"
                        CREATE TABLE IF NOT EXISTS Users (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Username TEXT NOT NULL UNIQUE,
                            Password TEXT NOT NULL,
                            Role TEXT NOT NULL
                        );";
                    var command = connection.CreateCommand();
                    command.CommandText = tableCommand;
                    command.ExecuteNonQuery();
                }
            }
        }

        // Voeg een gebruiker toe aan de database
        public static void AddUser(string username, string password, string role)
        {
            using (var connection = new SqliteConnection($"Data Source={DbPath}"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO Users (Username, Password, Role)
                    VALUES ($username, $password, $role);";
                command.Parameters.AddWithValue("$username", username);
                command.Parameters.AddWithValue("$password", password); 
                command.Parameters.AddWithValue("$role", role);
                command.ExecuteNonQuery();
            }
        }

        // Verkrijg een gebruiker op basis van gebruikersnaam en wachtwoord
        public static bool AuthenticateUser(string username, string password, out string role)
        {
            role = null;
            using (var connection = new SqliteConnection($"Data Source={DbPath}"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT Password, Role FROM Users WHERE Username = $username;";
                command.Parameters.AddWithValue("$username", username);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var storedPassword = reader.GetString(0);
                        role = reader.GetString(1);

                        // Vergelijk het ingevoerde wachtwoord met het opgeslagen gehashte wachtwoord
                        return BCrypt.Net.BCrypt.Verify(password, storedPassword);
                    }
                }
            }
            return false;
        }
    }
}
