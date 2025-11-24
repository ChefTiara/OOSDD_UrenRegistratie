using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hourregistration.Core.Interfaces.Services;
using Hourregistration.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace Hourregistration.App.ViewModels
{
    public partial class MedewerkerUrenoverzichtViewModel : BaseViewModel
    {
        private readonly IDeclaredHoursService _declaredHoursService;

        // show only the currently visible week's items
        public ObservableCollection<DeclaredHours> DeclaredHoursList { get; set; } = new ObservableCollection<DeclaredHours>();

        // current week's Monday
        private DateTime _currentWeekStart = GetStartOfWeek(DateTime.Today);

        public string WeekLabel => FormatWeekLabel(_currentWeekStart);

        private string textSearch = string.Empty;
        public string TextSearch
        {
            get => textSearch;
            set
            {
                textSearch = value;
                if (!string.IsNullOrEmpty(textSearch) && textSearch.Length >= 2)
                {
                    var filtered = DeclaredHoursList
                        .Where(x => x.Voornaam.Contains(textSearch, StringComparison.OrdinalIgnoreCase) ||
                                    x.Achternaam.Contains(textSearch, StringComparison.OrdinalIgnoreCase) ||
                                    x.FunctieName.Contains(textSearch, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                    DeclaredHoursList.Clear();
                    foreach (var item in filtered)
                        DeclaredHoursList.Add(item);
                }
                else
                {
                    ApplyWeek(); // reload the week if search text is less than 2 characters
                }
            }
        }
        public string TotalWorkedHours
        {
            get
            {
                var total = DeclaredHoursList?.Sum(x => x.WorkedHours) ?? 0;
                return $"Totaal aantal uren: {total}u";
            }
        }

        private bool isFilterVisible;
        public bool IsFilterVisible { get => isFilterVisible; set => SetProperty(ref isFilterVisible, value); }

        public ObservableCollection<string> FunctieOptions { get; } = new() { "Medewerker", "Teamleider" };

        private string selectedFunctie = string.Empty;
        public string SelectedFunctie { get => selectedFunctie; set => SetProperty(ref selectedFunctie, value); }

        private DateOnly selectedDate = DateOnly.FromDateTime(DateTime.Today);
        public DateOnly SelectedDate { get => selectedDate; set => SetProperty(ref selectedDate, value); }

        public ICommand FilterCommand { get; }
        public ICommand ApplyFiltersCommand { get; }
        public ICommand ResetFiltersCommand { get; }

        public MedewerkerUrenoverzichtViewModel(IDeclaredHoursService declaredHoursService)
        {
            _declaredHoursService = declaredHoursService;
            ApplyWeek(); // load initial week items

            FilterCommand = new Command(ToggleFilter);
            ApplyFiltersCommand = new Command(ApplyFilters);
            ResetFiltersCommand = new Command(ResetFilters);

            // defaultwaarden
            SelectedDate = DateOnly.FromDateTime(System.DateTime.Today);
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

        private void ToggleFilter() => IsFilterVisible = !IsFilterVisible;

        private void ApplyFilters()
        {
            // Pas hier je filtering toe op je collectie (bv. oproep service of filter lokale lijst)
        }

        private void ResetFilters()
        {
            SelectedFunctie = string.Empty;
            SelectedDate = DateOnly.FromDateTime(System.DateTime.Today);
            // reset overige filters en herlaad data
        }
    }
}