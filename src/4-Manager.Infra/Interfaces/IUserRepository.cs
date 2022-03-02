using System.Collections.Generic;
using System.Threading.Tasks;
using Manager.Domain.Entities;
namespace Manager.Infra.Interface{
    public interface IUserRepository : IBaseRepository<User>{
        Task<User> GetByEmailAsync(string email);
        Task<List<User>> SearchByEmailAsync(string email);
         Task<List<User>> SearchByNameAsync(string email);


    }
}