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
    public partial class AdministratiemedewerkerUrenoverzichtViewModel : BaseViewModel
    {
        private readonly IDeclaredHoursService _declaredHoursService;

        // Items die in de UI getoond worden
        public ObservableCollection<DeclaredHours> DeclaredHoursList { get; set; } = new();

        // Ongefilterde items van de huidige week
        private List<DeclaredHours> _allWeekItems = new();

        // huidige weekstart (maandag)
        private DateTime _currentWeekStart = GetStartOfWeek(DateTime.Today);
        public string WeekLabel => FormatWeekLabel(_currentWeekStart);

        // ===================================
        // ZOEKEN
        // ===================================

        // totaal gewerkte uren
        public string TotalWorkedHours
        {
            get
            {
                var total = DeclaredHoursList?.Sum(x => x.WorkedHours) ?? 0;
                return $"Totaal aantal uren: {total}u";
            }
        }

        // ===================================
        // FILTERS
        // ===================================

        private bool isFilterVisible;
        public bool IsFilterVisible
        {
            get => isFilterVisible;
            set => SetProperty(ref isFilterVisible, value);
        }

        public ObservableCollection<string> FunctieOptions { get; } =
            new() { "Medewerker", "Teamleider" };

        private string selectedFunctie = string.Empty;
        public string SelectedFunctie
        {
            get => selectedFunctie;
            set => SetProperty(ref selectedFunctie, value);
        }

        // belangrijke fix → nullable
        private DateOnly? selectedDate = null;
        public DateOnly? SelectedDate
        {
            get => selectedDate;
            set => SetProperty(ref selectedDate, value);
        }

        // commands
        public ICommand FilterCommand { get; }
        public ICommand ApplyFiltersCommand { get; }
        public ICommand ResetFiltersCommand { get; }

        public AdministratiemedewerkerUrenoverzichtViewModel(IDeclaredHoursService declaredHoursService)
        {
            _declaredHoursService = declaredHoursService;

            ApplyWeek();

            FilterCommand = new Command(ToggleFilter);
            ApplyFiltersCommand = new Command(ApplyFilters);
        }

        public override void Load() => ApplyWeek();
        public override void OnAppearing() => Load();
        public override void OnDisappearing() => DeclaredHoursList.Clear();

        // refresh
        [RelayCommand]
        private void Refresh() => ApplyWeek();

        // vorige week
        [RelayCommand]
        private void PreviousWeek()
        {
            _currentWeekStart = _currentWeekStart.AddDays(-7);
            ApplyWeek();
        }

        // volgende week
        [RelayCommand]
        private void NextWeek()
        {
            _currentWeekStart = _currentWeekStart.AddDays(7);
            ApplyWeek();
        }

        // ===================================
        // WEEK LADEN
        // ===================================
        private void ApplyWeek()
        {
            var all = _declaredHoursService.GetAll() ?? Enumerable.Empty<DeclaredHours>();

            var weekStart = _currentWeekStart.Date;
            var weekEnd = weekStart.AddDays(6).Date;

            var start = DateOnly.FromDateTime(weekStart);
            var end = DateOnly.FromDateTime(weekEnd);

            // bronitems van de week
            _allWeekItems = all
                .Where(i => i.Date >= start && i.Date <= end)
                .OrderBy(i => i.Date)
                .ThenBy(i => i.StartTime)
                .ToList();

            // eerst alles opnieuw tonen
            DeclaredHoursList.Clear();
            foreach (var item in _allWeekItems)
                DeclaredHoursList.Add(item);

            OnPropertyChanged(nameof(WeekLabel));
            OnPropertyChanged(nameof(TotalWorkedHours));
        }

        // ===================================
        // FILTER-LOGICA
        // ===================================
        private void ApplyFilters()
        {
            IEnumerable<DeclaredHours> filtered = _allWeekItems;

            // functiefilter
            if (!string.IsNullOrEmpty(SelectedFunctie))
                filtered = filtered.Where(x => x.FunctieName == SelectedFunctie);

            // datumfilter → alleen als gebruiker datum kiest
            if (SelectedDate.HasValue)
                filtered = filtered.Where(x => x.Date == SelectedDate.Value);


            DeclaredHoursList.Clear();
            foreach (var item in filtered)
                DeclaredHoursList.Add(item);

            OnPropertyChanged(nameof(TotalWorkedHours));
        }

        private void ToggleFilter() => IsFilterVisible = !IsFilterVisible;

        // ===================================
        // HELPERS
        // ===================================
        private static DateTime GetStartOfWeek(DateTime date)
        {
            var diff = date.DayOfWeek == DayOfWeek.Sunday ? -6 : DayOfWeek.Monday - date.DayOfWeek;
            return date.Date.AddDays(diff);
        }

        private static string FormatWeekLabel(DateTime weekStart)
        {
            var week = ISOWeek.GetWeekOfYear(weekStart);
            var end = weekStart.AddDays(6);

            return $"Week {week} ({weekStart:dd-MM-yyyy} / {end:dd-MM-yyyy})";
        }
    }
}
