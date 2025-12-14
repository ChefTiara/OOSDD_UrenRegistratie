using System;
using System.Collections.Generic;
using Hourregistration.App.Views;
using Hourregistration.Core.Models;

namespace Hourregistration.App.Services
{
    public static class SessionManager
    {
        
        public static Role? CurrentRole { get; set; }

        // Map pages to their page numbers
        private static readonly Dictionary<Type, int> PageNumbers = new()
        {
            { typeof(DeclarationPage), 4 }
        };
        

        // Overload: check access by page type (e.g., typeof(DeclarationPage))
        public static bool CanAccessPage(Type pageType)
        {
            if (CurrentRole == null)
                return false;

            if (!PageNumbers.TryGetValue(pageType, out var pageNumber))
                return false;

            return CanAccessPage(pageNumber);
        }

        // Existing logic by page number
        public static bool CanAccessPage(int pageNumber)
        {
            if (CurrentRole == null)
                return false;

            switch (CurrentRole)
            {
                case Role.Werknemer:
                    return pageNumber == 1;

                case Role.Opdrachtgever:
                    return pageNumber == 2;

                case Role.Administratiemedewerker:
                    
                    return pageNumber >= 1 && pageNumber <= 3;

                case Role.Beheer:
                   
                    return pageNumber <= 4;

                default:
                    return false;
            }
   
        }
    }
}
