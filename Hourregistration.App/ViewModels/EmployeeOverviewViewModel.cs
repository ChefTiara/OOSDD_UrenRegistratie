using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hourregistration.Core.Interfaces.Services;
using Hourregistration.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Hourregistration.App.ViewModels
{
    public partial class EmployeeOverviewViewModel : BaseViewModel
    {
        private readonly IDeclaredHoursService _declaredHoursService;

        // show only the currently visible week's items
        public ObservableCollection<DeclaredHours> DeclaredHoursList { get; set; } = [];

        // current week's Monday
        private DateTime _currentWeekStart = GetStartOfWeek(DateTime.Today);

        public string WeekLabel => FormatWeekLabel(_currentWeekStart);

        public string TotalWorkedHours
        {
            get
            {
                var total = DeclaredHoursList?.Sum(x => x.WorkedHours) ?? 0;
                return $"Totaal aantal uren: {total}u";
            }
        }

        public EmployeeOverviewViewModel(IDeclaredHoursService declaredHoursService)
        {
            _declaredHoursService = declaredHoursService;
            ApplyWeek(); // load initial week items
        }

        public override void Load() => ApplyWeek();

        public override void OnAppearing() => Load();

        public override void OnDisappearing() => DeclaredHoursList.Clear();

        // refresh / reload the current week
        [RelayCommand]
        private void Refresh() => ApplyWeek();

        // previous week
        [RelayCommand]
        private void PreviousWeek()
        {
            _currentWeekStart = _currentWeekStart.AddDays(-7);
            ApplyWeek();
        }

        // next week
        [RelayCommand]
        private void NextWeek()
        {
            _currentWeekStart = _currentWeekStart.AddDays(7);
            ApplyWeek();
        }

        // Load items for the current week and notify UI
        private void ApplyWeek()
        {
            var all = _declaredHoursService.GetAll() ?? Enumerable.Empty<DeclaredHours>();

            var weekStart = _currentWeekStart.Date;
            var weekEnd = weekStart.AddDays(6).Date;

            var weekStartDateOnly = DateOnly.FromDateTime(weekStart);
            var weekEndDateOnly = DateOnly.FromDateTime(weekEnd);

            // ORDER BY ascending so the first day (Monday) appears at the top.
            // ThenBy StartTime ensures items on the same day are ordered by start time.
            var items = all
                .Where(i => i.Date >= weekStartDateOnly && i.Date <= weekEndDateOnly)
                .OrderBy(i => i.Date)
                .ThenBy(i => i.StartTime);

            DeclaredHoursList.Clear();
            foreach (var item in items)
                DeclaredHoursList.Add(item);

            OnPropertyChanged(nameof(WeekLabel));
            OnPropertyChanged(nameof(TotalWorkedHours));
        }

        private static DateTime GetStartOfWeek(DateTime date)
        {
            // ISO week: Monday is first day
            var day = date.DayOfWeek;
            int diff = day == DayOfWeek.Sunday ? -6 : DayOfWeek.Monday - day;
            return date.Date.AddDays(diff);
        }

        private static string FormatWeekLabel(DateTime weekStart)
        {
            var weekNumber = ISOWeek.GetWeekOfYear(weekStart);
            var start = weekStart;
            var end = weekStart.AddDays(6);

            // show full day-month-year for both start and end, e.g.:
            // "Week 46 (10-11-2025 / 17-11-2025)"
            var startStr = start.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
            var endStr = end.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
            return $"Week {weekNumber} ({startStr} / {endStr})";
        }
    }
}