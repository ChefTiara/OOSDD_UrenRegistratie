using Hourregistration.App.ViewModels;
using Hourregistration.Core.Interfaces.Services;
using Hourregistration.Core.Models;
using System.Collections.ObjectModel;

namespace Hourregistration.App.ViewModels
{
    public partial class EmployeeOverview : BaseViewModel
    {
        private readonly IDeclaredHoursService _declaredHoursService;
        public ObservableCollection<DeclaredHours> DeclaredHoursList { get; set; } = [];
        public EmployeeOverview(IDeclaredHoursService declaredHoursService)
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