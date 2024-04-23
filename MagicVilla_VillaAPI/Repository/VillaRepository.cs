using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MagicVilla_VillaAPI.Repository
{
    public class VillaRepository : Repository<Villa>, IVillaRepository
    {
        public VillaRepository(ApplicationDbContext db) : base(db)
        {
        }

        public async Task UpdateAsync(Villa entity)
        {
            entity.UpdatedDate = DateTime.Now;
            _dbSet.Update(entity);
            await SaveAsync();
        }
    }
}
