using CommunityToolkit.Mvvm.ComponentModel;
using Hourregistration.Core.Interfaces.Repositories;
using Hourregistration.Core.Interfaces.Services;
using Hourregistration.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Hourregistration.App.ViewModels
{
    // UI-friendly wrapper for a LocalUser that exposes display properties
    public sealed class UserListItem
    {
        public LocalUser User { get; }

        public UserListItem(LocalUser user) => User = user;

        public string Username => User.Username;
        public string Role => User.Role.ToString();

        // Display "Geen" when LatestDeclaration is not set (DateTime.MinValue)
        public string LatestDeclarationDisplay =>
            User.LatestDeclaration == DateTime.MinValue
                ? "Geen"
                : User.LatestDeclaration.ToString("dd-MM-yyyy HH:mm");

        // Grey color when no latest declaration, otherwise default text color
        public Color LatestDeclarationColor =>
            User.LatestDeclaration == DateTime.MinValue
                ? Colors.Gray
                : Colors.White;
    }

    public partial class EmployeeHoursOverviewViewModel : BaseViewModel
    {
        private readonly ILocalUserRepository _repository;
        private readonly IDeclaredHoursService _declaredHoursService;

        // expose wrapper items to the UI
        public ObservableCollection<UserListItem> Users { get; set; } = new();
        private List<UserListItem> _allUsers = new();

        // FILTER OPTIONS (function)
        public ObservableCollection<string> FunctieOptions { get; } =
            new() { "Alle", "Werknemer", "Opdrachtgever", "Administratiemedewerker", "Beheer" };

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

        // Inject both repositories/services so we can set LatestDeclaration per user
        public EmployeeHoursOverviewViewModel(ILocalUserRepository repository, IDeclaredHoursService declaredHoursService)
        {
            _repository = repository;
            _declaredHoursService = declaredHoursService;
            LoadUsers();
        }

        public override void Load() => LoadUsers();

        private void LoadUsers()
        {
            var allUsers = _repository.GetAll().Result
                                 ?.OrderBy(x => x.FullName)
                                 .ToList() ?? new List<LocalUser>();

            // Populate LatestDeclaration for each user using DeclaredHoursService
            foreach (var user in allUsers)
            {
                try
                {
                    // Use service to get latest declaration; service may return null
                    var latest = _declaredHoursService.GetLatestDeclarationFromUserId(user.Id);
                    user.LatestDeclaration = latest?.CreatedAt ?? DateTime.MinValue;
                }
                catch
                {
                    // if service throws or no declaration exists, keep DateTime.MinValue
                    user.LatestDeclaration = DateTime.MinValue;
                }
            }

            // Wrap users for UI
            var wrapped = allUsers.Select(u => new UserListItem(u)).ToList();
            _allUsers = wrapped;

            Users.Clear();
            foreach (var item in wrapped)
                Users.Add(item);
        }

        private void ApplyFilters()
        {
            if (_allUsers == null)
                return;

            IEnumerable<UserListItem> filtered = _allUsers;

            // Function-filter (No filtering at "Alle")
            if (!string.IsNullOrWhiteSpace(SelectedFunctie) && SelectedFunctie != "Alle")
            {
                filtered = filtered.Where(x =>
                    x.User.Role.ToString().Equals(SelectedFunctie, System.StringComparison.OrdinalIgnoreCase));
            }

            // Search by name or username
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(x =>
                    (!string.IsNullOrWhiteSpace(x.User.FullName) && x.User.FullName.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase))
                    || (!string.IsNullOrWhiteSpace(x.User.Username) && x.User.Username.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase)));
            }

            Users.Clear();
            foreach (var item in filtered)
                Users.Add(item);
        }
    }
}