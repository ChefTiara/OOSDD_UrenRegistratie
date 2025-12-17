using System;
using System.Threading.Tasks;
using Hourregistration.Core.Interfaces.Services;
using Hourregistration.Core.Services;

namespace Hourregistration.Core.Models
{
    // Edit-DTO used by the declaration page. This maps to DeclaredHours when submitting.
    public class DeclarationRowModel
    {
        // Bindable from XAML DatePicker (DateTime)
        public DateTime Date { get; set; } = DateTime.Today;

        // Bindable from XAML Entry (numeric)
        public double? WorkedHours { get; set; }

        // Bindable from XAML Picker
        public string? Reason { get; set; }

        // Bindable from XAML Entry
        public string? Description { get; set; }

        // Helper: convert this edit-row to a DeclaredHours instance (async id reservation)
        public async Task<DeclaredHours> ToDeclaredHoursAsync(IDeclaredHoursService declaredHoursService, long userId = 0)
        {
            // Reserve id from repository/service
            var reserved = await declaredHoursService.ReserveIdAsync();
            reserved.Date = DateOnly.FromDateTime(Date);
            reserved.WorkedHours = WorkedHours ?? 0;

            // MAP to Reason (not ProjectName) — fix for missing reason
            reserved.Reason = Reason ?? string.Empty;

            // Still set ProjectName if you use it elsewhere (optional)
            // reserved.ProjectName = Reason ?? string.Empty;

            reserved.Description = Description ?? string.Empty;
            reserved.UserId = userId;
            return reserved;
        }
    }
}
