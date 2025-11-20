using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hourregistration.Core.Interfaces.Services;
using Hourregistration.Core.Models;
using System.Collections.ObjectModel;

namespace Hourregistration.App.ViewModels
{
    public partial class EmployeeOverviewViewModel : BaseViewModel
    {
        private readonly IDeclaredHoursService _declaredHoursService;
        public ObservableCollection<DeclaredHours> DeclaredHoursList { get; set; } = [];

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
            Load();
        }

        public override void Load()
        {
            DeclaredHoursList.Clear();
            foreach (var item in _declaredHoursService.GetAll() ?? Enumerable.Empty<DeclaredHours>())
            {
                DeclaredHoursList.Add(item);
            }

            // notify footer text update
            OnPropertyChanged(nameof(TotalWorkedHours));
        }

        public override void OnAppearing()
        {
            Load();
        }

        public override void OnDisappearing()
        {
            DeclaredHoursList.Clear();
        }

        // Command exposed to the view to reload the list manually
        [RelayCommand]
        private void Refresh()
        {
            Load();
        }
    }
}