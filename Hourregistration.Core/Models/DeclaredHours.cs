using Hourregistration.Core.Models;
using System.Globalization;

namespace Hourregistration.Core.Models
{
    public partial class DeclaredHours : Model
    {
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public double WorkedHours { get; set; }
        public string FunctieName { get; set; }
        public string Voornaam { get; set; }
        public string Achternaam { get; set; }
        public DeclaredState State { get; set; } = DeclaredState.Uren;

        public string PlannedHours => $"{StartTime.ToString("HH:mm")}-{EndTime.ToString("HH:mm")}";

   

        public int V1 { get; }
        public DateOnly DateOnly { get; }
        public TimeOnly TimeOnly1 { get; }
        public TimeOnly TimeOnly2 { get; }
        public string V2 { get; }

        public DeclaredHours(int id, string voornaam, string achternaam, string name, DateOnly date, TimeOnly startTime, TimeOnly endTime, string functieName) : base(id, name)
        {
            Date = date;
            StartTime = startTime;
            EndTime = endTime;
            FunctieName = functieName;
            Voornaam = voornaam;
            Achternaam = achternaam; 

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