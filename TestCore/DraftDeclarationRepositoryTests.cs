using System;
using System.Linq;
using NUnit.Framework;
using Hourregistration.Core.Data.Repositories;
using Hourregistration.Core.Models;

namespace TestCore
{
    [TestFixture]
    public class DraftDeclarationRepositoryTests
    {
        private DraftDeclarationRepository _repo;

        [SetUp]
        public void Setup()
        {
            _repo = new DraftDeclarationRepository();
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
    }
}