using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hourregistration.Core.Interfaces.Services;
using Hourregistration.Core.Models;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;

namespace Hourregistration.App.ViewModels
{
    public partial class UrenbeoordelingViewModel : BaseViewModel
    {
        private readonly IDeclaredHoursService _declaredHoursService;

        public ObservableCollection<DeclaredHours> PendingDeclarations { get; } = [];
        public ObservableCollection<DeclaredHours> ReviewedDeclarations { get; } = [];

        public UrenbeoordelingViewModel(IDeclaredHoursService declaredHoursService)
        {
            _declaredHoursService = declaredHoursService;
            Title = "Urenbeoordeling";
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            Load();
        }

        public override void Load()
        {
            PendingDeclarations.Clear();
            ReviewedDeclarations.Clear();

            // Get items based on State
            var pending = _declaredHoursService.GetByState(DeclaredState.Pending);
            foreach (var d in pending)
                PendingDeclarations.Add(d);

            var approved = _declaredHoursService.GetByState(DeclaredState.Approved);
            var denied = _declaredHoursService.GetByState(DeclaredState.Denied);

            foreach (var d in approved.Concat(denied))
                ReviewedDeclarations.Add(d);
        }

        [RelayCommand]
        private async Task Approve(DeclaredHours item)
        {
            if (item is null) return;

            // Confirm approval with the user
            var confirmed = await (Application.Current?.MainPage?.DisplayAlert(
                "Uren bevestigen",
                $"Weet u zeker dat u deze uren wilt goedkeuren voor {item.EmployeeName} ({item.ProjectName})?",
                "Bevestigen",
                "Annuleren") ?? Task.FromResult(false));

            if (!confirmed) return;

            item.State = DeclaredState.Approved;
            item.ReviewedOn = DateOnly.FromDateTime(DateTime.Now);
            _declaredHoursService.Update(item);

            PendingDeclarations.Remove(item);
            if (!ReviewedDeclarations.Contains(item))
                ReviewedDeclarations.Add(item);
        }

        [RelayCommand]
        private async Task Reject(DeclaredHours item)
        {
            if (item is null) return;

            // Confirm rejection with the user
            var confirmed = await (Application.Current?.MainPage?.DisplayAlert(
                "Uren afwijzen",
                $"Weet u zeker dat u deze uren wilt afwijzen voor {item.EmployeeName} ({item.ProjectName})?",
                "Afwijzen",
                "Annuleren") ?? Task.FromResult(false));

            if (!confirmed) return;

            item.State = DeclaredState.Denied;
            item.ReviewedOn = DateOnly.FromDateTime(DateTime.Now);
            _declaredHoursService.Update(item);

            PendingDeclarations.Remove(item);
            if (!ReviewedDeclarations.Contains(item))
                ReviewedDeclarations.Add(item);
        }
    }
}
