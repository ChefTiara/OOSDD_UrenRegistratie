using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Hourregistration.Core.Models;
using Hourregistration.Core.Interfaces.Services;
using Hourregistration.Core.Interfaces.Repositories;
using System.Collections.Generic;
using System.Threading;
using Hourregistration.Core.Services;

namespace TestCore
{
    [TestFixture]
    public class DeclarationServiceTests
    {
        private IDeclaredHoursService _declaredService;
        private DeclarationService _declarationService;
        private IDraftDeclarationRepository _draftRepo;

        [SetUp]
        public void Setup()
        {
            // use an in-memory declared hours service and a simple fake draft repo for unit tests
            _declaredService = new InMemoryDeclaredHoursService();
            _draftRepo = new FakeDraftRepository();
            _declarationService = new DeclarationService(_declaredService, _draftRepo);
        }

        // --- In-memory / fake test helpers ---

        private class InMemoryDeclaredHoursService : IDeclaredHoursService
        {
            private readonly List<DeclaredHours> _store = new();
            private long _nextId = 1;
            private readonly object _lock = new();

            public DeclaredHours? Get(long id) => _store.FirstOrDefault(d => d.Id == id);

            public List<DeclaredHours> GetByUserId(long userId) => _store.Where(d => d.UserId == userId).ToList();

            public List<DeclaredHours> GetByState(DeclaredState state) => _store.Where(d => d.State == state).ToList();

            public List<DeclaredHours> GetAll() => _store.ToList();

            public DeclaredHours GetLatestDeclarationFromUserId(long userId)
            {
                var item = _store.Where(d => d.UserId == userId).OrderByDescending(d => d.CreatedAt).FirstOrDefault();
                if (item == null) throw new InvalidOperationException("No declarations found for user.");
                return item;
            }

            public DeclaredHours Add(DeclaredHours declaredHour)
            {
                lock (_lock)
                {
                    if (declaredHour.Id == 0) declaredHour.GetType().GetProperty("Id")?.SetValue(declaredHour, _nextId++);
                    _store.Add(declaredHour);
                    return declaredHour;
                }
            }

            public DeclaredHours Update(DeclaredHours declaredHour)
            {
                var existing = Get(declaredHour.Id) ?? throw new ArgumentException("Not found");
                existing.WorkedHours = declaredHour.WorkedHours;
                existing.Reason = declaredHour.Reason;
                existing.ProjectName = declaredHour.ProjectName;
                existing.Description = declaredHour.Description;
                existing.State = declaredHour.State;
                return existing;
            }

            public DeclaredHours Delete(long id)
            {
                var existing = Get(id) ?? throw new ArgumentException("Not found");
                _store.Remove(existing);
                return existing;
            }

            public Task<DeclaredHours?> GetAsync(long id, CancellationToken ct = default) => Task.FromResult(Get(id));

            public Task<List<DeclaredHours>> GetByUserIdAsync(long userId, CancellationToken ct = default) => Task.FromResult(GetByUserId(userId));

            public Task<List<DeclaredHours>> GetByStateAsync(DeclaredState state, CancellationToken ct = default) => Task.FromResult(GetByState(state));

            public Task<List<DeclaredHours>> GetAllAsync(CancellationToken ct = default) => Task.FromResult(GetAll());

            public Task<DeclaredHours> GetLatestDeclarationFromUserIdAsync(long userId, CancellationToken ct = default) => Task.FromResult(GetLatestDeclarationFromUserId(userId));

            public Task<DeclaredHours> AddAsync(DateOnly date, int workedHours, string projectName, string description, long userId)
            {
                var dh = new DeclaredHours(0, date, workedHours, projectName, description, userId);
                return AddAsync(dh);
            }

            public Task<DeclaredHours> AddAsync(DeclaredHours declaredHour, CancellationToken ct = default)
            {
                lock (_lock)
                {
                    if (declaredHour.Id == 0) declaredHour.GetType().GetProperty("Id")?.SetValue(declaredHour, _nextId++);
                    declaredHour.CreatedAt = declaredHour.CreatedAt == default ? DateTime.UtcNow : declaredHour.CreatedAt;
                    _store.Add(declaredHour);
                    return Task.FromResult(declaredHour);
                }
            }

            public Task<DeclaredHours> ReserveIdAsync(CancellationToken ct = default)
            {
                lock (_lock)
                {
                    var id = _nextId++;
                    var dh = new DeclaredHours(id, DateOnly.FromDateTime(DateTime.UtcNow), 0, string.Empty, string.Empty, 0);
                    return Task.FromResult(dh);
                }
            }

            public Task<DeclaredHours> UpdateAsync(DeclaredHours declaredHour, CancellationToken ct = default)
            {
                var updated = Update(declaredHour);
                return Task.FromResult(updated);
            }

            public Task<DeclaredHours> DeleteAsync(long id, CancellationToken ct = default)
            {
                var deleted = Delete(id);
                return Task.FromResult(deleted);
            }

            public Task<DeclaredHours?> GetAsync(long id)
            {
                throw new NotImplementedException();
            }

            public Task<List<DeclaredHours>> GetByUserIdAsync(long userId)
            {
                throw new NotImplementedException();
            }

            public Task<List<DeclaredHours>> GetByStateAsync(DeclaredState state)
            {
                throw new NotImplementedException();
            }

            public Task<List<DeclaredHours>> GetAllAsync()
            {
                throw new NotImplementedException();
            }

            public Task<DeclaredHours> GetLatestDeclarationFromUserIdAsync(long userId)
            {
                throw new NotImplementedException();
            }

            public Task<DeclaredHours> AddAsync(DeclaredHours declaredHour)
            {
                throw new NotImplementedException();
            }

            public Task<DeclaredHours> ReserveIdAsync()
            {
                throw new NotImplementedException();
            }

            public Task<DeclaredHours> UpdateAsync(DeclaredHours declaredHour)
            {
                throw new NotImplementedException();
            }

            public Task<DeclaredHours> DeleteAsync(long id)
            {
                throw new NotImplementedException();
            }
        }

        private class FakeDraftRepository : IDraftDeclarationRepository
        {
            // minimal in-memory implementation to satisfy the service dependency
            private readonly List<DeclaredHours> _drafts = new();

            public DeclaredHours AddDraft(DeclaredHours declaration)
            {
                var copy = new DeclaredHours(
                    _drafts.Count + 1,
                    declaration.Date,
                    (int)declaration.WorkedHours,
                    declaration.Reason,
                    declaration.Description ?? string.Empty, // Fix: ensure non-null string
                    declaration.UserId)
                {
                    CreatedAt = declaration.CreatedAt == default ? DateTime.UtcNow : declaration.CreatedAt
                };
                _drafts.Add(copy);
                return copy;
            }

            public List<DeclaredHours> GetAllDrafts() => _drafts.ToList();

            public DeclaredHours? DeleteDraft(DeclaredHours declaration)
            {
                var existing = _drafts.FirstOrDefault(d => d.Id == declaration.Id);
                if (existing != null) _drafts.Remove(existing);
                return existing;
            }
        }
    }
}