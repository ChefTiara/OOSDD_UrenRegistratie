using CommunityToolkit.Mvvm.ComponentModel;
using Hourregistration.Core.Interfaces.Repositories;
using Hourregistration.Core.Models;
using System.Collections.ObjectModel;

namespace Hourregistration.App.ViewModels
{
    public partial class EmployeeHoursOverviewViewModel : BaseViewModel
    {
        private readonly IDeclaredHoursRepository _repository;

        public ObservableCollection<DeclaredHoursEmployee> EmployeeHours { get; set; } = new();
        private List<DeclaredHoursEmployee> _allEmployeeHours = new();

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

        private string searchText;
        public string SearchText
        {
            get => searchText;
            set
            {
                if (SetProperty(ref searchText, value))
                    ApplySearch();
            }
        }
        private void ApplySearch()
        {
            if (_allEmployeeHours == null) return;

            IEnumerable<DeclaredHoursEmployee> filtered = _allEmployeeHours;

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(x =>
                    x.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            EmployeeHours.Clear();
            foreach (var item in filtered)
                EmployeeHours.Add(item);
        }
    }
}