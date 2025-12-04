using Hourregistration.Core.Models;

namespace Hourregistration.Core.Interfaces.Services
{
    public interface IClientService
    {
        public Client? Get(int clientId);
        public List<Client> GetAll();
        public Client Add(Client client);
        public Client Update(Client client);
        public Client Delete(int id);

        public Task<Client?> GetAsync(int clientId);
        public Task<List<Client>> GetAllAsync();
        public Task<Client> AddAsync(Client client);
        public Task<Client> UpdateAsync(Client client);
        public Task<Client> DeleteAsync(int id);
    }
}