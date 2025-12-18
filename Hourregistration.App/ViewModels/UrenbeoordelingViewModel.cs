using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hourregistration.Core.Interfaces.Services;
using Hourregistration.Core.Models;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Linq;

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
            var pending = _declaredHoursService.GetByState(DeclaredState.Verzonden);
            foreach (var d in pending)
                PendingDeclarations.Add(d);

            var approved = _declaredHoursService.GetByState(DeclaredState.Akkoord);
            var denied = _declaredHoursService.GetByState(DeclaredState.Geweigerd);

            foreach (var d in approved.Concat(denied).OrderByDescending(ReviewedSortKey))
                ReviewedDeclarations.Add(d);
        }

        [RelayCommand]
        private async Task Approve(DeclaredHours item)
        {
            if (item is null) return;

            // Confirm approval with the user
            var confirmed = await (Application.Current?.MainPage?.DisplayAlert(
                "Uren bevestigen",
                $"Weet u zeker dat u deze uren wilt goedkeuren voor {item.User.Username} ({item.Reason})?",
                "Bevestigen",
                "Annuleren") ?? Task.FromResult(false));

            if (!confirmed) return;

            item.State = DeclaredState.Akkoord;
            item.ReviewedOn = DateOnly.FromDateTime(DateTime.Now);
            _declaredHoursService.Update(item);

            PendingDeclarations.Remove(item);
            AddReviewedOrdered(item);
        }

        [RelayCommand]
        private async Task Reject(DeclaredHours item)
        {
            if (item is null) return;

            // Confirm rejection with the user
            var confirmed = await (Application.Current?.MainPage?.DisplayAlert(
                "Uren afwijzen",
                $"Weet u zeker dat u deze uren wilt afwijzen voor {item.User.Username} ({item.Reason})?",
                "Afwijzen",
                "Annuleren") ?? Task.FromResult(false));

            if (!confirmed) return;

            item.State = DeclaredState.Geweigerd;
            item.ReviewedOn = DateOnly.FromDateTime(DateTime.Now);
            _declaredHoursService.Update(item);

            PendingDeclarations.Remove(item);
            AddReviewedOrdered(item);
        }

        private void AddReviewedOrdered(DeclaredHours item)
        {
            if (item is null) return;

            // If already present, leave it in place to avoid extra UI churn
            if (ReviewedDeclarations.Contains(item))
                return;

            // Insert at top so existing items visually slide down
            ReviewedDeclarations.Insert(0, item);
        }

        private static DateOnly ReviewedSortKey(DeclaredHours d)
        {
            if (d.ReviewedOn is DateOnly reviewed)
                return reviewed;
            return d.SubmittedOn;
        }
    }
}
