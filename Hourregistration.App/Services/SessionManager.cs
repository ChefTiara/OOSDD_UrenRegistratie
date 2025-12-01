using Hourregistration.Core.Models;

namespace Hourregistration.App.Services
{
    public static class SessionManager
    {
        public static Role? CurrentRole { get; set; }

        // PageNumber = 1, 2, 3 of 4
        public static bool CanAccessPage(int pageNumber)
        {
            if (CurrentRole == null)
                return false;

            switch (CurrentRole)
            {
                case Role.Werknemer:
                    // Werknemer: alleen pagina 1
                    return pageNumber == 1;

                case Role.Opdrachtgever:
                    // Opdrachtgever: alleen pagina 2
                    return pageNumber == 2;

                case Role.AdministratieMedewerker:
                    // Administratie medewerker: pagina 1, 2 en 3
                    return pageNumber == 3;

                case Role.Beheer:
                    // Beheer: pagina 1 t/m 4
                    return pageNumber <= 4;

                default:
                    return false;
            }
        }
    }
}