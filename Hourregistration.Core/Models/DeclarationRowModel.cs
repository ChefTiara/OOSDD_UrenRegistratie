using System;
using System.Threading.Tasks;
using Hourregistration.Core.Interfaces.Services;
using Hourregistration.Core.Services;

namespace Hourregistration.Core.Models
{
    // Edit-DTO used by the declaration page. This maps to DeclaredHours when submitting.
    public class DeclarationRowModel
    {
        public DateTime Date { get; set; } = DateTime.Today;
        public double? WorkedHours { get; set; }
        public string? Reason { get; set; }
        public string? Description { get; set; }

        public async Task<DeclaredHours> ToDeclaredHoursAsync(IDeclaredHoursService declaredHoursService, long userId = 0)
        {
            var reserved = await declaredHoursService.ReserveIdAsync();
            reserved.Date = DateOnly.FromDateTime(Date);
            reserved.WorkedHours = WorkedHours ?? 0;
            reserved.Reason = Reason ?? string.Empty;
            reserved.Description = Description ?? string.Empty;
            reserved.UserId = userId;
            return reserved;
        }
    }
}
