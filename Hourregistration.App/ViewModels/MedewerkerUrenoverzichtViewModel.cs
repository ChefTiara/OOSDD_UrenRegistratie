using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hourregistration.Core.Interfaces.Services;
using Hourregistration.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Microsoft.Maui.Controls;

namespace Hourregistration.App.ViewModels
{
    public partial class MedewerkerUrenoverzichtViewModel : BaseViewModel
    {
        private readonly IDeclaredHoursService _declaredHoursService;

        // Items shown in the UI
        public ObservableCollection<DeclaredHours> DeclaredHoursList { get; set; } = new();

        // Unfiltered source for the current week
        private List<DeclaredHours> _allWeekItems = new();

        // current week's Monday
        private DateTime _currentWeekStart = GetStartOfWeek(DateTime.Today);
        public string WeekLabel => FormatWeekLabel(_currentWeekStart);

        // SEARCH (kept, not a filter pane)
        private string textSearch = string.Empty;
        public string TextSearch
        {
            get => textSearch;
            set
            {
                if (SetProperty(ref textSearch, value))
                    ApplySearch();
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

        public MedewerkerUrenoverzichtViewModel(IDeclaredHoursService declaredHoursService)
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

            var start = DateOnly.FromDateTime(weekStart);
            var end = DateOnly.FromDateTime(weekEnd);

            _allWeekItems = all
                .Where(i => i.Date >= start && i.Date <= end)
                .OrderBy(i => i.Date)
                .ThenBy(i => i.StartTime)
                .ToList();

            DeclaredHoursList.Clear();
            foreach (var item in _allWeekItems)
                DeclaredHoursList.Add(item);

            OnPropertyChanged(nameof(WeekLabel));
            OnPropertyChanged(nameof(TotalWorkedHours));
        }

        // Apply only the simple Search (no filter pane)
        private void ApplySearch()
        {
            var items = string.IsNullOrWhiteSpace(TextSearch) || TextSearch.Length < 2
                ? _allWeekItems
                : _allWeekItems.Where(x =>
                    x.Voornaam.Contains(TextSearch, StringComparison.OrdinalIgnoreCase) ||
                    x.Achternaam.Contains(TextSearch, StringComparison.OrdinalIgnoreCase) ||
                    x.FunctieName.Contains(TextSearch, StringComparison.OrdinalIgnoreCase))
                  .ToList();

            DeclaredHoursList.Clear();
            foreach (var item in items)
                DeclaredHoursList.Add(item);

            OnPropertyChanged(nameof(TotalWorkedHours));
        }

        // Helpers
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
