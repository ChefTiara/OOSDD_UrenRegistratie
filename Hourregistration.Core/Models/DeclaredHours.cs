using Hourregistration.Core.Interfaces.Services;
using Hourregistration.Core.Models;
using Hourregistration.Core.Services;
using System.Globalization;

namespace Hourregistration.Core.Models
{
    public partial class DeclaredHours : Model
    {
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public double WorkedHours { get; set; } = 0.0;
        public string ProjectName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Client? Client { get; set; }

        public DeclaredState State { get; set; } = DeclaredState.Verzonden;

        public string PlannedHours => $"{StartTime.ToString("HH:mm")}-{EndTime.ToString("HH:mm")}";

        // Expose day name and a pre-formatted planned hours string for XAML binding
        public string Day
        {
            get
            {
                var dayName = Date.ToString("dddd", CultureInfo.CurrentCulture);
                if (string.IsNullOrEmpty(dayName))
                    return dayName;
                return char.ToUpper(dayName[0], CultureInfo.CurrentCulture) + dayName.Substring(1);
            }
        }

        public DeclaredHours(int id, DateOnly date, TimeOnly startTime, TimeOnly endTime, string projectName, string description, Client? client = null) : base(id)
        {
            Date = date;
            StartTime = startTime;
            EndTime = endTime;
            ProjectName = projectName;
            Description = description;
            Client = client;

            // Calculate worked hours inbetween endTime and startTime and apply to WorkedHours
            TimeSpan startSpan = StartTime.ToTimeSpan();
            TimeSpan endSpan = EndTime.ToTimeSpan();
            TimeSpan difference = endSpan - startSpan;
            if (difference < TimeSpan.Zero)
            {
                difference += TimeSpan.FromDays(1);
            }

            WorkedHours = (double)difference.TotalHours;
        }
    }
}