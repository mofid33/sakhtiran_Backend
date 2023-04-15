using System.Threading.Tasks;
using MarketPlace.API.Data.Models;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface IForceUpdateRepository
    {
        Task<TForceUpdate> GetForceUpdateAsync();
    }
}