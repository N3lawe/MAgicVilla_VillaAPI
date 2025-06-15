using MAgicVilla_VillaAPI.Data;
using MAgicVilla_VillaAPI.Models;
using MAgicVilla_VillaAPI.Repository.IRepository;
namespace MAgicVilla_VillaAPI.Repository;
public class VillaRepository(ApplicationDbContext _db) : Repository<Villa>(_db), IVillaRepository
{
    public async Task<Villa> UpdateAsync(Villa entity)
    {
        entity.UpdatedDate = DateTime.Now;
        _db.Villas.Update(entity);
        await _db.SaveChangesAsync();
        return entity;
    }
}