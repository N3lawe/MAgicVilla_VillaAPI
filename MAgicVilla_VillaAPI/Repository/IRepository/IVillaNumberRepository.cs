using MAgicVilla_VillaAPI.Models;

namespace MAgicVilla_VillaAPI.Repository.IRepository;
public interface IVillaNumberRepository : IRepository<VillaNumber>
{
    Task<VillaNumber> UpdateAsync(VillaNumber entity);
}