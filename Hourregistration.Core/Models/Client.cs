namespace Hourregistration.Core.Models
{
    public partial class Client : Model
    {
        public string Voornaam { get; set; } = string.Empty;
        public string Achternaam { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Role Function { get; set; } = Role.Werknemer;
        public Client(int id, string voornaam, string achternaam, string email) : base(id)
        {
            Voornaam = voornaam;
            Achternaam = achternaam;
            Email = email;
            Function = Role.Werknemer;
        }
    }
}
