using Hourregistration.Core.Models;

namespace Hourregistration.Core.Models
{
    public partial class DeclaredHours : Model
    {
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public double WorkedHours { get; set; }
        public string ProjectName { get; set; }
        public DeclaredState State { get; set; } = DeclaredState.Pending;

        public DeclaredHours(int id, DateOnly date, TimeOnly startTime, TimeOnly endTime, string projectName) : base(id)
        {
            Date = date;
            StartTime = startTime;
            EndTime = endTime;
            ProjectName = projectName;

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