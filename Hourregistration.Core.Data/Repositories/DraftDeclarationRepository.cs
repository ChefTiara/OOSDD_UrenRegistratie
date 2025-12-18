using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hourregistration.Core.Data.Database;
using Hourregistration.Core.Interfaces.Repositories;
using Hourregistration.Core.Models;
using Microsoft.Data.Sqlite;

namespace Hourregistration.Core.Data.Repositories
{
    public class DraftDeclarationRepository : IDraftDeclarationRepository
    {
        private readonly ISqliteConnectionFactory _factory;

        public DraftDeclarationRepository(ISqliteConnectionFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        // synchronous surface required by existing callers - delegates to async implementations
        public DeclaredHours AddDraft(DeclaredHours declaration)
        {
            return AddDraftAsync(declaration).GetAwaiter().GetResult();
        }

        public List<DeclaredHours> GetAllDrafts()
        {
            return GetAllDraftsAsync().GetAwaiter().GetResult();
        }

        // return null when no matching draft was found (matches updated interface contract)
        public DeclaredHours? DeleteDraft(DeclaredHours declaration)
        {
            return DeleteDraftAsync(declaration).GetAwaiter().GetResult();
        }

        // -----------------------
        // Async DB-backed methods
        // -----------------------

        private static DeclaredHours MapReaderToDeclaredHours(SqliteDataReader reader)
        {
            // columns in DraftDeclarations table:
            // draft_id, declaration_date, start_time, end_time,
            // worked_hours, reason, project_name, description, user_id, created_at, updated_at
            var id = reader.GetInt64(0);

            DateOnly date = DateOnly.MinValue;
            if (!reader.IsDBNull(1))
            {
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
            var userId = !reader.IsDBNull(8) ? reader.GetInt64(8) : 0L;
            var createdAt = !reader.IsDBNull(9) ? reader.GetDateTime(9) : DateTime.UtcNow;
            DateTime? updatedAt = null;
            if (!reader.IsDBNull(10)) updatedAt = reader.GetDateTime(10);

            var dh = new DeclaredHours(id, date, (int)workedHours, reason ?? string.Empty, description ?? string.Empty, userId)
            {
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };
            return dh;
        }

        public async Task<List<DeclaredHours>> GetAllDraftsAsync(CancellationToken ct = default)
        {
            using var conn = await _factory.CreateOpenConnectionAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT draft_id, declaration_date, start_time, end_time,
                       worked_hours, reason, project_name, description, user_id,
                       created_at, updated_at
                FROM DraftDeclarations
                ORDER BY created_at;";
            using var reader = await cmd.ExecuteReaderAsync(ct);
            var list = new List<DeclaredHours>();
            while (await reader.ReadAsync(ct))
            {
                list.Add(MapReaderToDeclaredHours(reader));
            }
            return list;
        }

        public async Task<DeclaredHours> AddDraftAsync(DeclaredHours declaration, CancellationToken ct = default)
        {
            if (declaration == null) throw new ArgumentNullException(nameof(declaration));

            using var conn = await _factory.CreateOpenConnectionAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO DraftDeclarations
                (declaration_date, start_time, end_time, worked_hours, reason, project_name, description, user_id, created_at, updated_at)
                VALUES ($date, $startTime, $endTime, $workedHours, $reason, $projectName, $description, $userId, $createdAt, $updatedAt);
                SELECT last_insert_rowid();";

            cmd.Parameters.AddWithValue("$date", declaration.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            cmd.Parameters.AddWithValue("$startTime", declaration.StartTime == default ? (object)DBNull.Value : declaration.StartTime.ToString("HH:mm:ss"));
            cmd.Parameters.AddWithValue("$endTime", declaration.EndTime == default ? (object)DBNull.Value : declaration.EndTime.ToString("HH:mm:ss"));
            cmd.Parameters.AddWithValue("$workedHours", declaration.WorkedHours);
            cmd.Parameters.AddWithValue("$reason", declaration.Reason ?? string.Empty);
            cmd.Parameters.AddWithValue("$projectName", declaration.ProjectName ?? string.Empty);
            cmd.Parameters.AddWithValue("$description", declaration.Description ?? string.Empty);
            cmd.Parameters.AddWithValue("$userId", declaration.UserId);
            cmd.Parameters.AddWithValue("$createdAt", declaration.CreatedAt == default ? DateTime.UtcNow : declaration.CreatedAt);
            cmd.Parameters.AddWithValue("$updatedAt", declaration.UpdatedAt.HasValue ? (object)declaration.UpdatedAt.Value : DBNull.Value);

            var scalar = await cmd.ExecuteScalarAsync(ct);
            long newId = scalar is long l ? l : Convert.ToInt64(scalar);

            var copy = new DeclaredHours(newId, declaration.Date, (int)declaration.WorkedHours, declaration.Reason ?? string.Empty, declaration.Description ?? string.Empty, declaration.UserId)
            {
                CreatedAt = declaration.CreatedAt == default ? DateTime.UtcNow : declaration.CreatedAt,
                UpdatedAt = declaration.UpdatedAt
            };

            return copy;
        }

        public async Task<DeclaredHours?> DeleteDraftAsync(DeclaredHours declaration, CancellationToken ct = default)
        {
            if (declaration == null) throw new ArgumentNullException(nameof(declaration));

            using var conn = await _factory.CreateOpenConnectionAsync();
            using var selectCmd = conn.CreateCommand();

            selectCmd.CommandText = @"
                SELECT draft_id, declaration_date, start_time, end_time,
                       worked_hours, reason, project_name, description, user_id,
                       created_at, updated_at
                FROM DraftDeclarations
                WHERE draft_id = $id
                LIMIT 1;";
            selectCmd.Parameters.AddWithValue("$id", declaration.Id);

            using var reader = await selectCmd.ExecuteReaderAsync(ct);
            DeclaredHours? existing = null;
            if (await reader.ReadAsync(ct))
            {
                existing = MapReaderToDeclaredHours(reader);
            }
            await reader.CloseAsync();

            if (existing == null)
            {
                // fallback: try to find by content match (date+hours+reason+description+user)
                using var fallbackCmd = conn.CreateCommand();
                fallbackCmd.CommandText = @"
                    SELECT draft_id, declaration_date, start_time, end_time,
                           worked_hours, reason, project_name, description, user_id,
                           created_at, updated_at
                    FROM DraftDeclarations
                    WHERE declaration_date = $date
                      AND ABS(worked_hours - $workedHours) < 0.0001
                      AND reason = $reason
                      AND description = $description
                      AND user_id = $userId
                    LIMIT 1;";
                fallbackCmd.Parameters.AddWithValue("$date", declaration.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
                fallbackCmd.Parameters.AddWithValue("$workedHours", declaration.WorkedHours);
                fallbackCmd.Parameters.AddWithValue("$reason", declaration.Reason ?? string.Empty);
                fallbackCmd.Parameters.AddWithValue("$description", declaration.Description ?? string.Empty);
                fallbackCmd.Parameters.AddWithValue("$userId", declaration.UserId);

                using var fbReader = await fallbackCmd.ExecuteReaderAsync(ct);
                if (await fbReader.ReadAsync(ct))
                {
                    existing = MapReaderToDeclaredHours(fbReader);
                }
                await fbReader.CloseAsync();
            }

            if (existing == null)
            {
                // not found
                return null;
            }

            using var deleteCmd = conn.CreateCommand();
            deleteCmd.CommandText = "DELETE FROM DraftDeclarations WHERE draft_id = $id;";
            deleteCmd.Parameters.AddWithValue("$id", existing.Id);
            await deleteCmd.ExecuteNonQueryAsync(ct);

            return existing;
        }
    }
}