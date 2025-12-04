using Hourregistration.Core.Interfaces.Repositories;
using Hourregistration.Core.Interfaces.Services;
using Hourregistration.Core.Models;

namespace Hourregistration.Core.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;

        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public Client? Get(int clientId)
        {
            return _clientRepository.Get(clientId);
        }
        public List<Client> GetAll()
        {
            return _clientRepository.GetAll();
        }
        public Client Add(Client client)
        {
            return _clientRepository.Add(client);
        }
        public Client Update(Client client)
        {
            return _clientRepository.Update(client);
        }
        public Client Delete(int id)
        {
            return _clientRepository.Delete(id);
        }

        public Task<Client?> GetAsync(int clientId)
        {
            return _clientRepository.GetAsync(clientId);
        }
        public Task<List<Client>> GetAllAsync()
        {
            return _clientRepository.GetAllAsync();
        }
        public Task<Client> AddAsync(Client client)
        {
            return _clientRepository.AddAsync(client);
        }
        public Task<Client> UpdateAsync(Client client)
        {
            return _clientRepository.UpdateAsync(client);
        }
        public Task<Client> DeleteAsync(int id)
        {
            return _clientRepository.DeleteAsync(id);
        }
    }
}