using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hourregistration.Core.Data.Database;
using Hourregistration.Core.Interfaces.Repositories;
using Hourregistration.Core.Models;
using Microsoft.Data.Sqlite;
using System.Globalization;

namespace Hourregistration.Core.Data.Repositories
{
    public class DeclaredHoursRepository : IDeclaredHoursRepository
    {
        private readonly ISqliteConnectionFactory _factory;
        private readonly ILocalUserRepository _localUserRepository;

        public DeclaredHoursRepository(ILocalUserRepository localUserRepository, ISqliteConnectionFactory factory)
        {
            _factory = factory;
            _localUserRepository = localUserRepository;
        }

        // ---------- synchronous wrappers (delegate to async DB methods) ----------
        public DeclaredHours? Get(long id)
        {
            return GetAsync(id).GetAwaiter().GetResult();
        }

        public List<DeclaredHours> GetByUserId(long userId)
        {
            return GetByUserIdAsync(userId).GetAwaiter().GetResult();
        }

        public List<DeclaredHours> GetByState(DeclaredState state)
        {
            return GetByStateAsync(state).GetAwaiter().GetResult();
        }

        public List<DeclaredHours> GetAll()
        {
            return GetAllAsync().GetAwaiter().GetResult();
        }

        public DeclaredHours GetLatestDeclarationFromUserId(long userId)
        {
            return GetLatestDeclarationFromUserIdAsync(userId).GetAwaiter().GetResult();
        }

        public DeclaredHours Add(DeclaredHours declaredHour)
        {
            return AddAsync(declaredHour).GetAwaiter().GetResult();
        }

        public DeclaredHours Update(DeclaredHours declaredHour)
        {
            return UpdateAsync(declaredHour).GetAwaiter().GetResult();
        }

        public DeclaredHours Delete(long id)
        {
            return DeleteAsync(id).GetAwaiter().GetResult();
        }

        // ----------------------------
        // Async database-backed methods
        // ----------------------------

        private static DeclaredHours MapReaderToDeclaredHours(SqliteDataReader reader)
        {
            // columns in Declarations table (see migrator)
            // declaration_id, declaration_date, start_time, end_time, worked_hours, reason,
            // project_name, description, state, user_id, created_at, updated_at
            var id = reader.GetInt64(0);
            DateOnly date = DateOnly.MinValue;
            if (!reader.IsDBNull(1))
            {
                // SQLite may store date as text; try DateTime then convert
                try
                {
                    var dt = reader.GetDateTime(1);
                    date = DateOnly.FromDateTime(dt);
                }
                catch
                {
                    var s = reader.GetString(1);
                    if (DateOnly.TryParse(s, out var d)) date = d;
                    else if (DateTime.TryParse(s, out var dt2)) date = DateOnly.FromDateTime(dt2);
                }
            }

            double workedHours = !reader.IsDBNull(4) ? reader.GetDouble(4) : 0.0;
            string reason = !reader.IsDBNull(5) ? reader.GetString(5) : string.Empty;
            string projectName = !reader.IsDBNull(6) ? reader.GetString(6) : string.Empty;
            string description = !reader.IsDBNull(7) ? reader.GetString(7) : string.Empty;
            var state = !reader.IsDBNull(8) ? (DeclaredState)reader.GetInt32(8) : DeclaredState.Verzonden;
            var userId = !reader.IsDBNull(9) ? reader.GetInt64(9) : 0L;

            var createdAt = !reader.IsDBNull(10) ? reader.GetDateTime(10) : DateTime.Now;
            DateTime? updatedAt = null;
            if (!reader.IsDBNull(11)) updatedAt = reader.GetDateTime(11);

            var dh = new DeclaredHours(id, date, (int)workedHours, reason, description, userId)
            {
                State = state,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };
            return dh;
        }

        public async Task<DeclaredHours?> GetAsync(long id, CancellationToken ct = default)
        {
            using var conn = await _factory.CreateOpenConnectionAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT declaration_id, declaration_date, start_time, end_time,
                       worked_hours, reason, project_name, description, state, user_id,
                       created_at, updated_at
                FROM Declarations
                WHERE declaration_id = $id;";
            cmd.Parameters.AddWithValue("$id", id);

            using var reader = await cmd.ExecuteReaderAsync(ct);
            if (await reader.ReadAsync(ct))
            {
                var dh = MapReaderToDeclaredHours(reader);
                // populate referenced user if available
                try
                {
                    dh.User = (await _localUserRepository.Get(dh.UserId, ct)) ?? dh.User;
                }
                catch { }
                return dh;
            }
            return null;
        }

        public async Task<List<DeclaredHours>> GetByUserIdAsync(long userId, CancellationToken ct = default)
        {
            using var conn = await _factory.CreateOpenConnectionAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT declaration_id, declaration_date, start_time, end_time,
                       worked_hours, reason, project_name, description, state, user_id,
                       created_at, updated_at
                FROM Declarations
                WHERE user_id = $userId
                ORDER BY declaration_date, created_at;";
            cmd.Parameters.AddWithValue("$userId", userId);

            using var reader = await cmd.ExecuteReaderAsync(ct);
            var list = new List<DeclaredHours>();
            while (await reader.ReadAsync(ct))
            {
                var dh = MapReaderToDeclaredHours(reader);
                try { dh.User = (await _localUserRepository.Get(dh.UserId, ct)) ?? dh.User; } catch { }
                list.Add(dh);
            }
            return list;
        }

        public async Task<List<DeclaredHours>> GetByStateAsync(DeclaredState state, CancellationToken ct = default)
        {
            using var conn = await _factory.CreateOpenConnectionAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT declaration_id, declaration_date, start_time, end_time,
                       worked_hours, reason, project_name, description, state, user_id,
                       created_at, updated_at
                FROM Declarations
                WHERE state = $state
                ORDER BY declaration_date, created_at;";
            cmd.Parameters.AddWithValue("$state", (int)state);

            using var reader = await cmd.ExecuteReaderAsync(ct);
            var list = new List<DeclaredHours>();
            while (await reader.ReadAsync(ct))
            {
                var dh = MapReaderToDeclaredHours(reader);
                try { dh.User = (await _localUserRepository.Get(dh.UserId, ct)) ?? dh.User; } catch { }
                list.Add(dh);
            }
            return list;
        }

        public async Task<List<DeclaredHours>> GetAllAsync(CancellationToken ct = default)
        {
            using var conn = await _factory.CreateOpenConnectionAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT declaration_id, declaration_date, start_time, end_time,
                       worked_hours, reason, project_name, description, state, user_id,
                       created_at, updated_at
                FROM Declarations
                ORDER BY declaration_date, created_at;";
            using var reader = await cmd.ExecuteReaderAsync(ct);
            var list = new List<DeclaredHours>();
            while (await reader.ReadAsync(ct))
            {
                var dh = MapReaderToDeclaredHours(reader);
                try { dh.User = (await _localUserRepository.Get(dh.UserId, ct)) ?? dh.User; } catch { }
                list.Add(dh);
            }
            return list;
        }

        public async Task<DeclaredHours> GetLatestDeclarationFromUserIdAsync(long userId, CancellationToken ct = default)
        {
            using var conn = await _factory.CreateOpenConnectionAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT declaration_id, declaration_date, start_time, end_time,
                       worked_hours, reason, project_name, description, state, user_id,
                       created_at, updated_at
                FROM Declarations
                WHERE user_id = $userId
                ORDER BY declaration_date DESC, created_at DESC
                LIMIT 1;";
            cmd.Parameters.AddWithValue("$userId", userId);

            using var reader = await cmd.ExecuteReaderAsync(ct);
            if (await reader.ReadAsync(ct))
            {
                var dh = MapReaderToDeclaredHours(reader);
                try { dh.User = (await _localUserRepository.Get(dh.UserId, ct)) ?? dh.User; } catch { }
                return dh;
            }
            throw new InvalidOperationException("No declarations found for user.");
        }

        public async Task<DeclaredHours> AddAsync(DateOnly date, int workedHours, string projectName, string description, long userId, CancellationToken ct = default)
        {
            using var conn = await _factory.CreateOpenConnectionAsync();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
                INSERT INTO Declarations (declaration_date, worked_hours, reason, project_name, description, state, user_id, created_at)
                VALUES ($date, $workedHours, $reason, $projectName, $description, $state, $userId, $createdAt);
                SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("$date", date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            cmd.Parameters.AddWithValue("$workedHours", workedHours);
            cmd.Parameters.AddWithValue("$reason", projectName ?? string.Empty); // historical mapping: projectName used as reason in some seeds
            cmd.Parameters.AddWithValue("$projectName", projectName ?? string.Empty);
            cmd.Parameters.AddWithValue("$description", description ?? string.Empty);
            cmd.Parameters.AddWithValue("$state", (int)DeclaredState.Verzonden);
            cmd.Parameters.AddWithValue("$userId", userId);
            cmd.Parameters.AddWithValue("$createdAt", DateTime.UtcNow);

            var scalar = await cmd.ExecuteScalarAsync(ct);
            long newId = scalar is long l ? l : Convert.ToInt64(scalar);
            var declaredHour = new DeclaredHours(newId, date, workedHours, projectName ?? string.Empty, description ?? string.Empty, userId)
            {
                CreatedAt = DateTime.UtcNow
            };
            return declaredHour;
        }

        public async Task<DeclaredHours> AddAsync(DeclaredHours declaredHour, CancellationToken ct = default)
        {
            using var conn = await _factory.CreateOpenConnectionAsync();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
                INSERT INTO Declarations (declaration_date, start_time, end_time, worked_hours, reason, project_name, description, state, user_id, created_at, updated_at)
                VALUES ($date, $startTime, $endTime, $workedHours, $reason, $projectName, $description, $state, $userId, $createdAt, $updatedAt);
                SELECT last_insert_rowid();";

            cmd.Parameters.AddWithValue("$date", declaredHour.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            cmd.Parameters.AddWithValue("$startTime", declaredHour.StartTime == default ? (object)DBNull.Value : declaredHour.StartTime.ToString("HH:mm:ss"));
            cmd.Parameters.AddWithValue("$endTime", declaredHour.EndTime == default ? (object)DBNull.Value : declaredHour.EndTime.ToString("HH:mm:ss"));
            cmd.Parameters.AddWithValue("$workedHours", declaredHour.WorkedHours);
            cmd.Parameters.AddWithValue("$reason", declaredHour.Reason ?? string.Empty);
            cmd.Parameters.AddWithValue("$projectName", declaredHour.ProjectName ?? string.Empty);
            cmd.Parameters.AddWithValue("$description", declaredHour.Description ?? string.Empty);
            cmd.Parameters.AddWithValue("$state", (int)declaredHour.State);
            cmd.Parameters.AddWithValue("$userId", declaredHour.UserId);
            cmd.Parameters.AddWithValue("$createdAt", declaredHour.CreatedAt == default ? DateTime.UtcNow : declaredHour.CreatedAt);
            cmd.Parameters.AddWithValue("$updatedAt", declaredHour.UpdatedAt.HasValue ? (object)declaredHour.UpdatedAt.Value : DBNull.Value);

            var scalar = await cmd.ExecuteScalarAsync(ct);
            long newId = scalar is long l ? l : Convert.ToInt64(scalar);
            // set returned id on object
            declaredHour.GetType().GetProperty("Id")?.SetValue(declaredHour, newId);
            return declaredHour;
        }

        public async Task<DeclaredHours> ReserveIdAsync(CancellationToken ct = default)
        {
            using var conn = await _factory.CreateOpenConnectionAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT IFNULL(MAX(declaration_id), 0) + 1 FROM Declarations;";
            var scalar = await cmd.ExecuteScalarAsync(ct);
            var nextId = scalar is long l ? l : Convert.ToInt64(scalar);
            var dh = new DeclaredHours(nextId, DateOnly.FromDateTime(DateTime.UtcNow), 0, string.Empty, string.Empty, 0);
            return dh;
        }

        public async Task<DeclaredHours> UpdateAsync(DeclaredHours declaredHour, CancellationToken ct = default)
        {
            using var conn = await _factory.CreateOpenConnectionAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE Declarations
                SET declaration_date = $date,
                    start_time = $startTime,
                    end_time = $endTime,
                    worked_hours = $workedHours,
                    reason = $reason,
                    project_name = $projectName,
                    description = $description,
                    state = $state,
                    user_id = $userId,
                    updated_at = $updatedAt
                WHERE declaration_id = $id;";
            cmd.Parameters.AddWithValue("$id", declaredHour.Id);
            cmd.Parameters.AddWithValue("$date", declaredHour.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            cmd.Parameters.AddWithValue("$startTime", declaredHour.StartTime == default ? (object)DBNull.Value : declaredHour.StartTime.ToString("HH:mm:ss"));
            cmd.Parameters.AddWithValue("$endTime", declaredHour.EndTime == default ? (object)DBNull.Value : declaredHour.EndTime.ToString("HH:mm:ss"));
            cmd.Parameters.AddWithValue("$workedHours", declaredHour.WorkedHours);
            cmd.Parameters.AddWithValue("$reason", declaredHour.Reason ?? string.Empty);
            cmd.Parameters.AddWithValue("$projectName", declaredHour.ProjectName ?? string.Empty);
            cmd.Parameters.AddWithValue("$description", declaredHour.Description ?? string.Empty);
            cmd.Parameters.AddWithValue("$state", (int)declaredHour.State);
            cmd.Parameters.AddWithValue("$userId", declaredHour.UserId);
            cmd.Parameters.AddWithValue("$updatedAt", DateTime.UtcNow);

            await cmd.ExecuteNonQueryAsync(ct);
            return declaredHour;
        }

        public async Task<DeclaredHours> DeleteAsync(long id, CancellationToken ct = default)
        {
            // read existing first
            var existing = await GetAsync(id, ct) ?? throw new ArgumentException("Declared hour not found");
            using var conn = await _factory.CreateOpenConnectionAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Declarations WHERE declaration_id = $id;";
            cmd.Parameters.AddWithValue("$id", id);
            await cmd.ExecuteNonQueryAsync(ct);
            return existing;
        }
    }
}