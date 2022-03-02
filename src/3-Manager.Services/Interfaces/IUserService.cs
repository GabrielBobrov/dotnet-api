using System.Threading.Tasks;
using System.Collections.Generic;
using Manager.Services.DTO;
using Manager.Core.Structs;

namespace Manager.Services.Interfaces{
    public interface IUserService{
        Task<Optional<UserDto>> CreateAsync(UserDto userDto);
        Task<Optional<UserDto>> UpdateAsync(UserDto userDto);
        Task RemoveAsync(long id);
        Task<Optional<UserDto>> GetAsync(long id);
        Task<Optional<IList<UserDto>>> GetAllAsync();
        Task<Optional<IList<UserDto>>> SearchByNameAsync(string name);
        Task<Optional<IList<UserDto>>> SearchByEmailAsync(string email);
        Task<Optional<UserDto>> GetByEmailAsync(string email);
    }
}