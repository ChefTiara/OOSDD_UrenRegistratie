using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hourregistration.Core.Data.Database;
using Hourregistration.Core.Interfaces.Repositories;
using Hourregistration.Core.Models;
using Microsoft.Data.Sqlite;

namespace Hourregistration.Core.Data.Repositories
{
    public class LocalUserRepository : ILocalUserRepository
    {
        private readonly ISqliteConnectionFactory _factory;

        public LocalUserRepository(ISqliteConnectionFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        // Keep a synchronous Authenticate surface (interface) but implement via DB-backed async call.
        // This avoids changing public API while keeping storage in the database.
        public LocalUser? Authenticate(string username, string password)
        {
            // call the async method synchronously (acceptable here because callers expect sync)
            var users = GetByUsernameAsync(username).GetAwaiter().GetResult();
            var user = users.FirstOrDefault();
            if (user == null) return null;
            if (!user.IsActive) return null;

            var stored = user.Password ?? string.Empty;
            // check bcrypt or plain
            if (stored.StartsWith("$2a$") || stored.StartsWith("$2b$") || stored.StartsWith("$2y$"))
                return BCrypt.Net.BCrypt.Verify(password, stored) ? user : null;

            return stored == password ? user : null;
        }

        // -------------------------
        // Async, DB-backed methods
        // -------------------------

        private static LocalUser MapReaderToLocalUser(SqliteDataReader reader)
        {
            var id = reader.GetInt64(0);
            var username = !reader.IsDBNull(1) ? reader.GetString(1) : string.Empty;
            var password = !reader.IsDBNull(2) ? reader.GetString(2) : string.Empty;

            // role stored as integer OR text in DB; parse both safely
            Role role = Role.Werknemer;
            if (!reader.IsDBNull(3))
            {
                try
                {
                    var raw = reader.GetValue(3);
                    if (raw is long || raw is int)
                    {
                        role = (Role)Convert.ToInt32(raw);
                    }
                    else if (raw is string s)
                    {
                        // try parse as int first, then as enum name
                        if (int.TryParse(s, out var rInt) && Enum.IsDefined(typeof(Role), rInt))
                        {
                            role = (Role)rInt;
                        }
                        else if (Enum.TryParse<Role>(s, ignoreCase: true, out var parsedRole))
                        {
                            role = parsedRole;
                        }
                    }
                }
                catch
                {
                    // keep default if parsing fails
                    role = Role.Werknemer;
                }
            }

            var isActive = true;
            if (!reader.IsDBNull(4))
            {
                var v = reader.GetInt32(4);
                isActive = v != 0;
            }

            var user = new LocalUser(id, username, password, role)
            {
                IsActive = isActive,
                LatestDeclaration = DateTime.MinValue
            };

            return user;
        }

        private async Task<List<LocalUser>> GetByUsernameAsync(string username, CancellationToken ct = default)
        {
            using var conn = await _factory.CreateOpenConnectionAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT user_id, username, password, role, is_active
                FROM Users
                WHERE LOWER(username) = LOWER($username);";
            cmd.Parameters.AddWithValue("$username", username ?? string.Empty);

            using var reader = await cmd.ExecuteReaderAsync(ct);
            var list = new List<LocalUser>();
            while (await reader.ReadAsync(ct))
            {
                list.Add(MapReaderToLocalUser(reader));
            }
            return list;
        }

        public async Task<LocalUser?> Get(long userId, CancellationToken ct = default)
        {
            using var conn = await _factory.CreateOpenConnectionAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT user_id, username, password, role, is_active
                FROM Users
                WHERE user_id = $id;";
            cmd.Parameters.AddWithValue("$id", userId);

            using var reader = await cmd.ExecuteReaderAsync(ct);
            if (await reader.ReadAsync(ct))
            {
                return MapReaderToLocalUser(reader);
            }
            return null;
        }

        public async Task<List<LocalUser>> GetAll(CancellationToken ct = default)
        {
            using var conn = await _factory.CreateOpenConnectionAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT user_id, username, password, role, is_active
                FROM Users
                ORDER BY username;";
            using var reader = await cmd.ExecuteReaderAsync(ct);
            var list = new List<LocalUser>();
            while (await reader.ReadAsync(ct))
            {
                list.Add(MapReaderToLocalUser(reader));
            }
            return list;
        }

        public async Task<List<LocalUser>> GetAllFromRole(Role role, CancellationToken ct = default)
        {
            using var conn = await _factory.CreateOpenConnectionAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT user_id, username, password, role, is_active
                FROM Users
                WHERE role = $role
                ORDER BY username;";
            cmd.Parameters.AddWithValue("$role", (int)role);

            using var reader = await cmd.ExecuteReaderAsync(ct);
            var list = new List<LocalUser>();
            while (await reader.ReadAsync(ct))
            {
                list.Add(MapReaderToLocalUser(reader));
            }
            return list;
        }

        public async Task<LocalUser> AddAsync(string username, string password, Role role, CancellationToken ct = default)
        {
            using var conn = await _factory.CreateOpenConnectionAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Users (username, password, role, is_active)
                VALUES ($username, $password, $role, $isActive);
                SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("$username", username ?? string.Empty);
            cmd.Parameters.AddWithValue("$password", password ?? string.Empty);
            cmd.Parameters.AddWithValue("$role", (int)role);
            cmd.Parameters.AddWithValue("$isActive", 1);

            var scalar = await cmd.ExecuteScalarAsync(ct);
            long newId = scalar is long l ? l : Convert.ToInt64(scalar);

            var user = new LocalUser(newId, username ?? string.Empty, password ?? string.Empty, role)
            {
                IsActive = true,
                LatestDeclaration = DateTime.MinValue
            };

            return user;
        }

        public async Task UpdateAsync(LocalUser updated, CancellationToken ct = default)
        {
            if (updated == null) throw new ArgumentNullException(nameof(updated));
            using var conn = await _factory.CreateOpenConnectionAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE Users
                SET username = $username,
                    password = $password,
                    role = $role,
                    is_active = $isActive
                WHERE user_id = $id;";
            cmd.Parameters.AddWithValue("$id", updated.Id);
            cmd.Parameters.AddWithValue("$username", updated.Username ?? string.Empty);
            cmd.Parameters.AddWithValue("$password", updated.Password ?? string.Empty);
            cmd.Parameters.AddWithValue("$role", (int)updated.Role);
            cmd.Parameters.AddWithValue("$isActive", updated.IsActive ? 1 : 0);

            await cmd.ExecuteNonQueryAsync(ct);
        }
    }
}