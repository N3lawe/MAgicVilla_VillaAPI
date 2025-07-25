﻿using MAgicVilla_VillaAPI.Data;
using MAgicVilla_VillaAPI.Models;
using MAgicVilla_VillaAPI.Repository.IRepository;

namespace MAgicVilla_VillaAPI.Repository;
public class VillaNumberRepository : Repository<VillaNumber>, IVillaNumberRepository
{
    private readonly ApplicationDbContext _db;
    public VillaNumberRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }
    public async Task<VillaNumber> UpdateAsync(VillaNumber entity)
    {
        entity.UpdatedDate = DateTime.Now;
        _db.VillaNumbers.Update(entity);
        await _db.SaveChangesAsync();
        return entity;
    }
}