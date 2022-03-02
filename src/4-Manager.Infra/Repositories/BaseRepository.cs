using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Manager.Domain.Entities;
using Manager.Infra.Interface;
using Manager.Infra.Context;

namespace Manager.Infra.Repository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : Base
    {
        private readonly ManagerContext _context;

        public BaseRepository(ManagerContext context)
        {
            _context = context;
        }

        public virtual async Task<T> CreateAsync (T obj)
        {
            _context.Add(obj);
            await _context.SaveChangesAsync();

            return obj;
        }

        public virtual async Task<T> UpdateAsync (T obj)
        {
            _context.Entry(obj).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return obj;
        }

        public virtual async Task<T> RemoveAsync (long id)
        {
            var obj = await GetAsync(id);

            if(obj != null)
            {
                _context.Remove(obj);
                await _context.SaveChangesAsync();
            }

            return obj;
        }

        public virtual async Task<T> GetAsync (long id)
        {
            var obj = await _context.Set<T>()
                                    .AsNoTracking()
                                    .Where(x => x.Id == id)
                                    .ToListAsync();

            return obj.FirstOrDefault();
        }

        public virtual async Task<List<T>> GetAsync()
        {
            return await _context.Set<T>()
                                 . AsNoTracking()
                                 .ToListAsync();
        }

    }
}