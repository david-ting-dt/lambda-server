using System.Collections.Generic;
using System.Threading.Tasks;
using HelloWorld.DbItem;

namespace HelloWorld.Interfaces
{
    public interface IDbHandler
    {
        Task<List<Person>> GetPeopleAsync();
        Task AddPersonAsync(int id, string name);
        Task DeletePersonAsync(int id);
        Task UpdatePersonAsync(int id, string newName);
    }
}