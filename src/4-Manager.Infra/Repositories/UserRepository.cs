using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Manager.Domain.Entities;
using Manager.Infra.Context;
using Manager.Infra.Interface;
using Manager.Infra.Repository;
using Microsoft.EntityFrameworkCore;

namespace Manager.Infra.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository{
        private readonly ManagerContext _context;

        public UserRepository(ManagerContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User> GetByEmail(string email)
        {
            var user = await _context.Users
                                     .Where
                                     (
                                         x => x.Email.ToLower() == email.ToLower()
                                     )
                                     .AsNoTracking()
                                     .ToListAsync();
            return user.FirstOrDefault();
        }

        public async Task<List<User>> SearchByEmail(string email)
        {
            var users = await _context.Users
                                     .Where
                                     (
                                         x => x.Email.ToLower() == email.ToLower()
                                     )
                                     .AsNoTracking()
                                     .ToListAsync();
            return users;
        }

        public async Task<List<User>> SearchByName(string name)
        {
            var users = await _context.Users
                                     .Where
                                     (
                                         x => x.Name.ToLower() == name.ToLower()
                                     )
                                     .AsNoTracking()
                                     .ToListAsync();
            return users;
        }
    }
}