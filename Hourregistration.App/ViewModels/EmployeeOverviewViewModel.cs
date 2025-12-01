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
        private bool _suppressApply = false;

        // show only the currently visible week's items
        public ObservableCollection<DeclaredHours> DeclaredHoursList { get; set; } = [];

        // Filter pickers (dropdowns) - project filter intentionally empty for now
        private ObservableCollection<string> _projectOptions = [];
        public ObservableCollection<string> ProjectOptions
        {
            get => _projectOptions;
            set
            {
                if (ReferenceEquals(_projectOptions, value)) return;
                _projectOptions = value;
                OnPropertyChanged(nameof(ProjectOptions));
            }
        }

        private ObservableCollection<string> _stateOptions = [];
        public ObservableCollection<string> StateOptions
        {
            get => _stateOptions;
            set
            {
                if (ReferenceEquals(_stateOptions, value)) return;
                _stateOptions = value;
                OnPropertyChanged(nameof(StateOptions));
            }
        }

        // Date filter: use a DatePicker (select a date) and allow clearing the filter.
        // SelectedDate is nullable — null == "All" (no date filter)
        private DateTime? _selectedDate;
        public DateTime? SelectedDate
        {
            get => _selectedDate;
            private set
            {
                if (_selectedDate == value) return;
                _selectedDate = value;
                OnPropertyChanged(nameof(SelectedDate));
                OnPropertyChanged(nameof(SelectedDateNonNull));
            }
        }

        // Backing for DatePicker binding: non-nullable DateTime used by the control.
        // Setting this enables the date filter.
        public DateTime SelectedDateNonNull
        {
            get => SelectedDate ?? DateTime.Today;
            set
            {
                // If we're making programmatic updates (initialization or clearing during navigation),
                // don't enable the date filter or run ApplyWeek. Use _suppressApply to signal that.
                if (_suppressApply)
                {
                    // update backing value so DatePicker shows the desired date,
                    // but do not treat this as a user selection.
                    _selectedDate = value;
                    OnPropertyChanged(nameof(SelectedDate));
                    OnPropertyChanged(nameof(SelectedDateNonNull));
                    return;
                }

                // if the same date and filter already enabled, nothing to do
                if (SelectedDate == value && _isDateFilterEnabled) return;

                // enable date filter and update backing value
                _isDateFilterEnabled = true;
                SelectedDate = value;

                // notify both properties (DatePicker reading SelectedDateNonNull, and the toggle)
                OnPropertyChanged(nameof(SelectedDateNonNull));
                OnPropertyChanged(nameof(IsDateFilterEnabled));

                if (!_suppressApply)
                {
                    if (MainThread.IsMainThread)
                        _ = ApplyWeek();
                    else
                        MainThread.BeginInvokeOnMainThread(() => _ = ApplyWeek());
                }
            }
        }

        private bool _isDateFilterEnabled = false;
        public bool IsDateFilterEnabled
        {
            get => _isDateFilterEnabled;
            set
            {
                if (_isDateFilterEnabled == value) return;
                _isDateFilterEnabled = value;
                if (!value)
                {
                    // clearing the filter
                    SelectedDate = null;
                }
                else
                {
                    // enabling without a selected date => set to today
                    SelectedDate ??= DateTime.Today;
                }
                OnPropertyChanged(nameof(IsDateFilterEnabled));
                OnPropertyChanged(nameof(SelectedDateNonNull));
                if (!_suppressApply) _ = ApplyWeek();
            }
        }

        private string? _selectedProjectOption;
        public string SelectedProjectOption
        {
            get => _selectedProjectOption ?? ProjectOptions.FirstOrDefault() ?? "All";
            set
            {
                if (_selectedProjectOption == value) return;
                _selectedProjectOption = value;
                OnPropertyChanged(nameof(SelectedProjectOption));
                if (!_suppressApply) _ = ApplyWeek();
            }
        }

        private string? _selectedStateOption;
        public string SelectedStateOption
        {
            get => _selectedStateOption ?? StateOptions.FirstOrDefault() ?? "All";
            set
            {
                if (_selectedStateOption == value) return;
                _selectedStateOption = value;
                OnPropertyChanged(nameof(SelectedStateOption));
                if (!_suppressApply) _ = ApplyWeek();
            }
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

            // Project filter intentionally empty for now
            ProjectOptions = new ObservableCollection<string>();

            // populate state options from the enum to always show available states
            var states = new ObservableCollection<string> { "All" };
            foreach (var name in Enum.GetNames(typeof(DeclaredState)).OrderBy(n => n))
                states.Add(name);
            StateOptions = states;

            // date filter starts disabled (All)
            SelectedDate = null;
            _isDateFilterEnabled = false;

            _selectedProjectOption = ProjectOptions.FirstOrDefault();
            _selectedStateOption = StateOptions.FirstOrDefault();

            // schedule initial load without blocking constructor
            if (MainThread.IsMainThread)
                _ = ApplyWeek();
            else
                MainThread.BeginInvokeOnMainThread(() => _ = ApplyWeek());
        }

        public override void Load() => _ = ApplyWeek();

        public override void OnAppearing() => Load();

        public override void OnDisappearing() => DeclaredHoursList.Clear();

        [RelayCommand]
        private async Task Refresh() => await ApplyWeek();

        [RelayCommand]
        private async Task PreviousWeek()
        {
            _currentWeekStart = _currentWeekStart.AddDays(-7);
            OnPropertyChanged(nameof(WeekLabel));

            // when navigating weeks, clear any active date filter so the new week shows all items
            _suppressApply = true;
            SelectedDate = null;
            _isDateFilterEnabled = false;
            OnPropertyChanged(nameof(IsDateFilterEnabled));
            OnPropertyChanged(nameof(SelectedDateNonNull));
            _suppressApply = false;

            await ApplyWeek();
        }

        [RelayCommand]
        private async Task NextWeek()
        {
            _currentWeekStart = _currentWeekStart.AddDays(7);
            OnPropertyChanged(nameof(WeekLabel));

            // when navigating weeks, clear any active date filter so the new week shows all items
            _suppressApply = true;
            SelectedDate = null;
            _isDateFilterEnabled = false;
            OnPropertyChanged(nameof(IsDateFilterEnabled));
            OnPropertyChanged(nameof(SelectedDateNonNull));
            _suppressApply = false;

            await ApplyWeek();
        }

        [RelayCommand]
        private void ClearDateFilter()
        {
            IsDateFilterEnabled = false;
            // SelectedDate will be set to null by IsDateFilterEnabled setter and ApplyWeek will run
        }

        // Load items for the current week and notify UI
        private async Task ApplyWeek()
        {
            var all = await _declaredHoursService.GetAllAsync() ?? Enumerable.Empty<DeclaredHours>();

            var weekStart = _currentWeekStart.Date;
            var weekEnd = weekStart.AddDays(6).Date;

            var weekStartDateOnly = DateOnly.FromDateTime(weekStart);
            var weekEndDateOnly = DateOnly.FromDateTime(weekEnd);

            var items = all
                .Where(i => i.Date >= weekStartDateOnly && i.Date <= weekEndDateOnly)
                .ToList();

            // prevent re-entrant ApplyWeek calls while we update lists
            _suppressApply = true;

            // ProjectOptions stays intentionally empty; do not mutate it here.

            // Date filter now uses SelectedDate/IsDateFilterEnabled rather than a dropdown.

            _suppressApply = false;

            // apply selected filters
            var filtered = items.AsEnumerable();

            var selectedProject = SelectedProjectOption ?? "All";
            if (selectedProject != "All")
                filtered = filtered.Where(i => (i.ProjectName ?? string.Empty) == selectedProject);

            var selectedState = SelectedStateOption ?? "All";
            if (selectedState != "All")
                filtered = filtered.Where(i => i.State.ToString() == selectedState);

            if (IsDateFilterEnabled && SelectedDate.HasValue)
            {
                var dto = DateOnly.FromDateTime(SelectedDate.Value);
                filtered = filtered.Where(i => i.Date == dto);
            }

            // default ordering (by date then start time)
            var ordered = filtered.OrderBy(i => i.Date).ThenBy(i => i.StartTime).ToList();

            // Update ObservableCollection on UI thread
            if (MainThread.IsMainThread)
            {
                DeclaredHoursList.Clear();
                foreach (var item in ordered)
                    DeclaredHoursList.Add(item);
                OnPropertyChanged(nameof(TotalWorkedHours));
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    DeclaredHoursList.Clear();
                    foreach (var item in ordered)
                        DeclaredHoursList.Add(item);
                    OnPropertyChanged(nameof(TotalWorkedHours));
                });
            }
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