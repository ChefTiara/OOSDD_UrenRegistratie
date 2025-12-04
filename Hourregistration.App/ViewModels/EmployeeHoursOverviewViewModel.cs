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

            EmployeeHours.Clear();
            foreach (var item in all)
                EmployeeHours.Add(item);
        }
    }
}