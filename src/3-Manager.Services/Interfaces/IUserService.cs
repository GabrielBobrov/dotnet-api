using Manager.Services.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Manager.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> CreateAsync(UserDto userDto);
        Task<UserDto> UpdateAsync(UserDto userDto);
        Task RemoveAsync(long id);
        Task<UserDto> GetAsync(long id);
        Task<UserDto> GetAsync(UserDto userDto);
        Task<List<UserDto>> GetAllAsync();
        Task<List<UserDto>> SearchByNameAsync(string name);
        Task<List<UserDto>> SearchByEmailAsync(string email);
        Task<UserDto> GetByEmailAsync(string email);
        
    }
}