using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hourregistration.Core.Interfaces.Services;
using Hourregistration.Core.Models;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Hourregistration.App.ViewModels
{
    public partial class EmployeeOverviewViewModel : BaseViewModel
    {
        private readonly IDeclaredHoursService _declaredHoursService;

        // show only the currently visible week's items
        public ObservableCollection<DeclaredHours> DeclaredHoursList { get; set; } = [];

        // Sorting options exposed to the UI (picker)
        public List<string> SortOptions { get; } = [
            "Date ↑",
            "Date ↓",
            "Project A→Z",
            "Project Z→A",
            "Hours ↑",
            "Hours ↓",
        ];

        private int _sortIndex = 0;
        private string? _selectedSortOption;
        public string SelectedSortOption
        {
            get => _selectedSortOption ?? SortOptions[0];
            set
            {
                if (_selectedSortOption == value) return;
                _selectedSortOption = value;
                _sortIndex = SortOptions.IndexOf(value);
                OnPropertyChanged(nameof(SelectedSortOption));
                OnPropertyChanged(nameof(SortButtonText));
                ApplyWeek(); // re-apply filter + sort when selection changes
            }
        }

        // helper to support the existing cycle-sort button in the view
        public string SortButtonText => SortOptions.ElementAtOrDefault(_sortIndex) ?? SortOptions[0];

        [RelayCommand]
        private void CycleSort()
        {
            _sortIndex = (_sortIndex + 1) % SortOptions.Count;
            _selectedSortOption = SortOptions[_sortIndex];
            OnPropertyChanged(nameof(SelectedSortOption));
            OnPropertyChanged(nameof(SortButtonText));
            ApplyWeek();
        }

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
            _selectedSortOption = SortOptions[0];
            _sortIndex = 0;
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
            OnPropertyChanged(nameof(WeekLabel));
            ApplyWeek();
        }

        // next week
        [RelayCommand]
        private void NextWeek()
        {
            _currentWeekStart = _currentWeekStart.AddDays(7);
            OnPropertyChanged(nameof(WeekLabel));
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

            var items = all
                .Where(i => i.Date >= weekStartDateOnly && i.Date <= weekEndDateOnly);

            // apply sorting chosen by the picker / cycle button
            var ordered = ApplySort(items);

            DeclaredHoursList.Clear();
            foreach (var item in ordered)
                DeclaredHoursList.Add(item);

            OnPropertyChanged(nameof(TotalWorkedHours));
        }

        private IEnumerable<DeclaredHours> ApplySort(IEnumerable<DeclaredHours> items)
        {
            var selected = _selectedSortOption ?? SortOptions.ElementAtOrDefault(_sortIndex) ?? SortOptions[0];

            return selected switch
            {
                "Date ↑" => items.OrderBy(i => i.Date).ThenBy(i => i.StartTime),
                "Date ↓" => items.OrderByDescending(i => i.Date).ThenBy(i => i.StartTime),
                "Project A→Z" => items.OrderBy(i => i.ProjectName ?? string.Empty).ThenBy(i => i.Date),
                "Project Z→A" => items.OrderByDescending(i => i.ProjectName ?? string.Empty).ThenBy(i => i.Date),
                "Hours ↑" => items.OrderBy(i => i.WorkedHours).ThenBy(i => i.Date),
                "Hours ↓" => items.OrderByDescending(i => i.WorkedHours).ThenBy(i => i.Date),
                _ => items.OrderBy(i => i.Date).ThenBy(i => i.StartTime),
            };
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