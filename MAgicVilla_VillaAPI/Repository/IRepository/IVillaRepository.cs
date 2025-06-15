using MAgicVilla_VillaAPI.Models;
namespace MAgicVilla_VillaAPI.Repository.IRepository;
public interface IVillaRepository : IRepository<Villa>
{
    Task<Villa> UpdateAsync(Villa entity);
}
