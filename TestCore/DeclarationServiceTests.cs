using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Hourregistration.Core.Data.Repositories;
using Hourregistration.Core.Services;
using Hourregistration.Core.Models;

namespace TestCore
{
    [TestFixture]
    public class DeclarationServiceTests
    {
        private DeclaredHoursRepository _declaredRepo;
        private DeclaredHoursService _declaredService;
        private DeclarationService _declarationService;
        private LocalUserRepository _localUserRepo;
        private DraftDeclarationRepository _draftRepo;

        [SetUp]
        public void Setup()
        {
            _localUserRepo = new LocalUserRepository();
            _declaredRepo = new DeclaredHoursRepository(_localUserRepo);
            _declaredService = new DeclaredHoursService(_declaredRepo);
            _draftRepo = new DraftDeclarationRepository();
            _declarationService = new DeclarationService(_declaredService, _draftRepo);
        }

        [Test]
        public async Task IndienenAsync_ShouldAllow_WhenTotalForSameDayIsWithinLimit()
        {
            // arrange
            var today = DateOnly.FromDateTime(DateTime.Today);
            // seed existing 3 hours for user 2
            var existing = await _declaredRepo.AddAsync(today, 3, "Seed", "seed", 2);
            Assert.That(existing.WorkedHours, Is.EqualTo(3));

            var toSubmit = await _declaredRepo.ReserveIdAsync();
            toSubmit.Date = today;
            toSubmit.WorkedHours = 5; // total 8 == allowed
            toSubmit.Reason = "Werk";
            toSubmit.UserId = 2;

            // act
            var (ok, message) = await _declarationService.IndienenAsync(toSubmit);

            // assert - should be accepted
            Assert.That(ok, Is.True, $"Declaration should be accepted when same-day total is equal or below 8 hours. Message: {message}");
        }
    }
}