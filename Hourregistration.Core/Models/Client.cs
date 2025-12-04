namespace Hourregistration.Core.Models
{
    public partial class Client : Model
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Role FunctionName { get; set; } = Role.Werknemer;
        public Client(int id, string name, string email) : base(id)
        {
            Name = name;
            Email = email;
            FunctionName = Role.Werknemer;
        }
    }
}
