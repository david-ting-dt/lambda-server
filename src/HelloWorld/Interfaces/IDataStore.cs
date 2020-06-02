using System.IO;
using System.Threading.Tasks;

namespace HelloWorld.Interfaces
{
    public interface IDataStore
    {
        public Task<Stream> Get();
    }
}