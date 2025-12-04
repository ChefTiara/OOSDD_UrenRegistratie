using Hourregistration.Core.Interfaces.Repositories;
using Hourregistration.Core.Models;

namespace Hourregistration.Core.Data.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly List<Client> clients;
        public ClientRepository()
        {
            clients = [
                new Client(0, "A", "Aaa", "aaa@gmail.com") { Function = Role.Werknemer },
                new Client(1, "AB", "Aba", "aba@gmail.com") { Function = Role.Werknemer },
                new Client(2, "B", "Bbb", "bbb@gmail.com") { Function = Role.Opdrachtgever },
                new Client(3, "C", "Ccc", "ccc@gmail.com") { Function = Role.AdministratieMedewerker },
                new Client(4, "D", "Ddd", "ddd@gmail.com") { Function = Role.Beheer },
            ];
        }

        public Client? Get(int id)
        {
            return clients.FirstOrDefault(dh => dh.Id == id);
        }
        public List<Client> GetAll()
        {
            return clients;
        }
        public Client Add(Client client)
        {
            clients.Add(client);
            return client;
        }
        public Client Update(Client client)
        {
            Client? existingClient = Get(client.Id) ?? throw new ArgumentException("Declared hour not found");
            clients.Remove(existingClient);
            clients.Add(existingClient);
            return existingClient;
        }
        public Client Delete(int id)
        {
            Client? existingClient = Get(id) ?? throw new ArgumentException("Declared hour not found");
            clients.Remove(existingClient);
            return existingClient;
        }

        public Task<Client?> GetAsync(int id)
        {
            return Task.FromResult(Get(id));
        }
        public Task<List<Client>> GetAllAsync()
        {
            return Task.FromResult(GetAll());
        }
        public Task<Client> AddAsync(Client client)
        {
            return Task.FromResult(Add(client));
        }
        public Task<Client> UpdateAsync(Client client)
        {
            return Task.FromResult(Update(client));
        }
        public Task<Client> DeleteAsync(int id)
        {
            return Task.FromResult(Delete(id));
        }
    }
}