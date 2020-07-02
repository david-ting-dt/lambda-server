using System.Collections.Generic;
using System.Threading.Tasks;
using HelloWorld.DbItem;

namespace HelloWorld.Interfaces
{
    public interface IDbHandler
    {
        Task<List<Person>> GetPeopleAsync();
        Task AddPersonAsync(string name);
        Task DeletePersonAsync(string id);
        Task UpdatePersonAsync(string id, string newName);
    }
}