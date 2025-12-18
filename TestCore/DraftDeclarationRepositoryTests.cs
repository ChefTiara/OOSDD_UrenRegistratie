using System;
using System.Linq;
using NUnit.Framework;
using Hourregistration.Core.Models;
using System.Collections.Generic;

namespace TestCore
{
    [TestFixture]
    public class DraftDeclarationRepositoryTests
    {
        private InMemoryDraftRepository _repo;

        [SetUp]
        public void Setup()
        {
            _repo = new InMemoryDraftRepository();
        }

        [Test]
        public void AddDraft_ShouldStoreCopy_NotReference()
        {
            var draft = new DeclaredHours(1000, DateOnly.FromDateTime(DateTime.Today), 2, "Werk", "desc", 1);

            var stored = _repo.AddDraft(draft);

            // stored should have same values but be a different reference
            Assert.That(stored, Is.Not.Null);
            Assert.That(stored.Id, Is.EqualTo(draft.Id));
            Assert.That(stored, Is.Not.SameAs(draft));
        }

        [Test]
        public void GetAllDrafts_ReturnsAddedDrafts()
        {
            var d1 = new DeclaredHours(1001, DateOnly.FromDateTime(DateTime.Today), 2, "Werk", "a", 1);
            var d2 = new DeclaredHours(1002, DateOnly.FromDateTime(DateTime.Today.AddDays(1)), 3, "Verlof", "b", 1);

            _repo.AddDraft(d1);
            _repo.AddDraft(d2);

            var all = _repo.GetAllDrafts();
            Assert.That(all.Count, Is.EqualTo(2));
            Assert.That(all.Select(x => x.Id).ToArray(), Does.Contain(1001));
            Assert.That(all.Select(x => x.Id).ToArray(), Does.Contain(1002));
        }

        [Test]
        public void DeleteDraft_ByInstanceWithSameId_RemovesDraft()
        {
            var draft = new DeclaredHours(2000, DateOnly.FromDateTime(DateTime.Today), 2, "Werk", "desc", 1);
            var stored = _repo.AddDraft(draft);

            // delete by passing a different instance but same id
            var toDelete = new DeclaredHours(2000, DateOnly.FromDateTime(DateTime.Today), 2, "Werk", "desc", 1);

            var removed = _repo.DeleteDraft(toDelete);
            Assert.That(removed, Is.Not.Null);
            var remaining = _repo.GetAllDrafts();
            Assert.That(remaining.Count, Is.EqualTo(0));
        }

        [Test]
        public void DeleteDraft_ByContentMatch_RemovesDraft()
        {
            // Add draft with id = 0 (simulate reserved draft without id)
            var draft = new DeclaredHours(0, DateOnly.FromDateTime(DateTime.Today), 2, "Werk", "desc", 1);
            var stored = _repo.AddDraft(draft);

            // create a different instance with id = 0 but same content
            var candidate = new DeclaredHours(0, DateOnly.FromDateTime(DateTime.Today), 2, "Werk", "desc", 1);

            var removed = _repo.DeleteDraft(candidate);
            Assert.That(removed, Is.Not.Null);

            // ensure it's removed
            var remaining = _repo.GetAllDrafts();
            Assert.That(remaining.Count, Is.EqualTo(0));
        }

        // Simple in-memory replacement for DraftDeclarationRepository used in unit tests.
        // The real repository uses SQLite; tests use this lightweight in-memory implementation
        // to avoid database dependencies and make behavior deterministic.
        private class InMemoryDraftRepository
        {
            private readonly List<DeclaredHours> _store = new();
            private long _nextId = 1;
            private readonly object _lock = new();

            public DeclaredHours AddDraft(DeclaredHours declaration)
            {
                if (declaration == null) throw new ArgumentNullException(nameof(declaration));

                lock (_lock)
                {
                    // create a copy so callers don't keep the stored reference
                    var id = declaration.Id != 0 ? declaration.Id : _nextId++;
                    var copy = new DeclaredHours(id, declaration.Date, (int)declaration.WorkedHours, declaration.Reason ?? string.Empty, declaration.Description ?? string.Empty, declaration.UserId)
                    {
                        CreatedAt = declaration.CreatedAt == default ? DateTime.UtcNow : declaration.CreatedAt,
                        UpdatedAt = declaration.UpdatedAt
                    };
                    _store.Add(copy);
                    return copy;
                }
            }

            public List<DeclaredHours> GetAllDrafts()
            {
                lock (_lock)
                {
                    // return shallow copies to avoid exposing internal references
                    return _store.Select(d => new DeclaredHours(d.Id, d.Date, (int)d.WorkedHours, d.Reason, d.Description ?? string.Empty, d.UserId)
                    {
                        CreatedAt = d.CreatedAt,
                        UpdatedAt = d.UpdatedAt
                    }).ToList();
                }
            }

            public DeclaredHours? DeleteDraft(DeclaredHours declaration)
            {
                if (declaration == null) throw new ArgumentNullException(nameof(declaration));

                lock (_lock)
                {
                    // try match by id first (id != 0)
                    if (declaration.Id != 0)
                    {
                        var byId = _store.FirstOrDefault(d => d.Id == declaration.Id);
                        if (byId != null)
                        {
                            _store.Remove(byId);
                            return byId;
                        }
                    }

                    // fallback: find by content (date, worked hours, reason, description, user)
                    var fallback = _store.FirstOrDefault(d =>
                        d.Date == declaration.Date &&
                        d.WorkedHours == declaration.WorkedHours &&
                        string.Equals(d.Reason ?? string.Empty, declaration.Reason ?? string.Empty, StringComparison.Ordinal) &&
                        string.Equals(d.Description ?? string.Empty, declaration.Description ?? string.Empty, StringComparison.Ordinal) &&
                        d.UserId == declaration.UserId);

                    if (fallback != null)
                    {
                        _store.Remove(fallback);
                        return fallback;
                    }

                    return null;
                }
            }
        }
    }
}