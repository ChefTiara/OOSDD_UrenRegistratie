using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Hourregistration.Core.Models;

namespace Hourregistration.Core.Models
{
    public class DeclaredHoursEmployee : Model
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public long UserId { get; set; } = 0;

        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        public double WorkedHours { get; set; }

        public string FullName => $"{FirstName} {LastName}";
        public string PlannedHours => $"{StartTime:HH\\:mm}-{EndTime:HH\\:mm}";

        public DeclaredHoursEmployee(
            long id,
            string firstName,
            string lastName,
            string role,
            long userId,
            DateOnly date,
            TimeOnly startTime,
            TimeOnly endTime
        ) : base(id)
        {
            FirstName = firstName;
            LastName = lastName;
            Role = role;
            UserId = userId;
            Date = date;
            StartTime = startTime;
            EndTime = endTime;

            TimeSpan diff = endTime.ToTimeSpan() - startTime.ToTimeSpan();
            if (diff < TimeSpan.Zero)
                diff += TimeSpan.FromDays(1);

            WorkedHours = diff.TotalHours;
        }
    }
}