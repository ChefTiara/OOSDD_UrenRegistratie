    using CommunityToolkit.Mvvm.ComponentModel;
using Hourregistration.Core.Interfaces.Repositories;
using Hourregistration.Core.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace Hourregistration.App.ViewModels
{
    public partial class EmployeeHoursOverviewViewModel : BaseViewModel
    {
        private readonly IDeclaredHoursRepository _repository;

        public ObservableCollection<DeclaredHoursEmployee> EmployeeHours { get; set; } = new();
        private List<DeclaredHoursEmployee> _allEmployeeHours = new();

        // FILTER OPTIONS (function)
        public ObservableCollection<string> FunctieOptions { get; } =
            new() { "Alle", "Medewerker", "Teamleider" };

        private string selectedFunctie = "Alle";   // Default value
        public string SelectedFunctie
        {
            get => selectedFunctie;
            set
            {
                if (SetProperty(ref selectedFunctie, value))
                    ApplyFilters();  // instantly apply filters
            }
        }

        // SEARCH
        private string searchText;
        public string SearchText
        {
            get => searchText;
            set
            {
                if (SetProperty(ref searchText, value))
                    ApplyFilters();   // instantly apply filters
            }
        }

        public EmployeeHoursOverviewViewModel(IDeclaredHoursRepository repository)
        {
            _repository = repository;
            LoadHours();
        }

        public override void Load() => LoadHours();

        private void LoadHours()
        {
            var all = _repository.GetAllEmployeeHours()
                                 ?.OrderBy(x => x.FullName)
                                 .ToList() ?? new();

            _allEmployeeHours = all;

            EmployeeHours.Clear();
            foreach (var item in all)
                EmployeeHours.Add(item);
        }

        private void ApplyFilters()
        {
            if (_allEmployeeHours == null)
                return;

            IEnumerable<DeclaredHoursEmployee> filtered = _allEmployeeHours;

            // Function-filter (No filtering at "Alle")
            if (!string.IsNullOrWhiteSpace(SelectedFunctie) && SelectedFunctie != "Alle")
            {
                filtered = filtered.Where(x =>
                    x.Role.Equals(SelectedFunctie, StringComparison.OrdinalIgnoreCase));
            }

            // Search by name
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(x =>
                    x.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            EmployeeHours.Clear();
            foreach (var item in filtered)
                EmployeeHours.Add(item);
        }
        public string EmployeeName { get; set; }
        public string CurrentWeekText { get; set; } // bv "Week 45 (03-11-2025 / 09-11-2025)"
        public IEnumerable<HourEntry> FilteredHours { get; set; }
        public int TotalHours { get; set; }

        public class HourEntry
        {
            public string Day { get; set; }
            public DateTime Date { get; set; }
            public string PlannedTime { get; set; }
            public int Hours { get; set; }
            public string ProjectName { get; set; }
            public string Description { get; set; }
        }
    }
}