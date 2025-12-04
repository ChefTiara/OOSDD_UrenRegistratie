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

        public int V1 { get; }
        public DateOnly DateOnly { get; }
        public TimeOnly TimeOnly1 { get; }
        public TimeOnly TimeOnly2 { get; }
        public string V2 { get; }
        
        //DeclaredHours van UC4 is nu gecomment omdat het in conflict komt met de Declaratie pagina van Ruben
        //public DeclaredHours(int id, string voornaam, string achternaam, string name, DateOnly date, TimeOnly startTime, TimeOnly endTime, string functieName) : base(id, name)
        
        public string ProjectName { get; set; }
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