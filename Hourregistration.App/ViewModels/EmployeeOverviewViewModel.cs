using CommunityToolkit.Mvvm.ComponentModel;
using Hourregistration.App.ViewModels;
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
                // Example: return the total as a formatted string, e.g. "Totaal: 40u"
                var total = DeclaredHoursList?.Sum(x => x.WorkedHours) ?? 0;
                return $"Totaal aantal gewerkte uren: {total}u";
            }
        }

        public EmployeeOverviewViewModel(IDeclaredHoursService declaredHoursService)
        {
            _declaredHoursService = declaredHoursService;
            DeclaredHoursList = [];
            Load();
        }


        public override void Load()
        {
            DeclaredHoursList.Clear();
            foreach (var item in _declaredHoursService.GetAll())
            {
                DeclaredHoursList.Add(item);
            }
        }

        public override void OnAppearing()
        {
            Load();
        }

        public override void OnDisappearing()
        {
            DeclaredHoursList.Clear();
        }
    }
}