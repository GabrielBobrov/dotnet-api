using AutoMapper;
using EscNet.Cryptography.Interfaces;
using Manager.Core.Exceptions;
using Manager.Domain.Entities;
using Manager.Infra.Interface;
using Manager.Services.DTO;
using Manager.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Manager.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IRijndaelCryptography _rijndaelCryptography;

        private readonly IUserRepository _userRepository;

        public UserService(IMapper mapper, IUserRepository userRepository, IRijndaelCryptography rijndaelCryptography )
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _rijndaelCryptography = rijndaelCryptography;
        }

        public async Task<UserDto> CreateAsync(UserDto userDto)
        {
            var userExists = await _userRepository.GetByEmailAsync(userDto.Email);

            if(userExists != null)
            {
                throw new DomainException("Ja existe um usuario cadastrado com esse email");
            }
        
            var user = _mapper.Map<User>(userDto);
            user.Validate();
            user.ChangePassword(_rijndaelCryptography.Encrypt(user.Password));

            var userCreated = await _userRepository.CreateAsync(user);
            
            return _mapper.Map<UserDto>(userCreated);
        }
        public async Task<UserDto> UpdateAsync(UserDto userDto)
        {
            var userExists = await _userRepository.GetAsync(userDto.Id);

            if(userExists == null)
            {
                throw new DomainException("Nao existe nenhum usuario cadastrado com o id informado");
            }

            var user = _mapper.Map<User>(userDto);
            user.Validate();
            user.ChangePassword(_rijndaelCryptography.Encrypt(user.Password));


            var userUpdated = await _userRepository.UpdateAsync(user);

            return _mapper.Map<UserDto>(userUpdated);

        }
        public async Task RemoveAsync(long id)
        {
            await _userRepository.RemoveAsync(id);
        }
        public async Task<UserDto> GetAsync(UserDto userDto)
        {
            var user = await _userRepository.GetAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> GetAsync(long id)
        {
            var user = await _userRepository.GetAsync(id);

            return _mapper.Map<UserDto>(user);
        }
        public async Task<List<UserDto>> GetAllAsync()
        {
            var allUsers = await _userRepository.GetAsync();

            return _mapper.Map<List<UserDto>>(allUsers);
        }
        public async Task<List<UserDto>> SearchByNameAsync(string name)
        {
            var allusers = await _userRepository.SearchByNameAsync(name);

            return _mapper.Map<List<UserDto>>(allusers);
        }
        public async Task<List<UserDto>> SearchByEmailAsync(string email)
        {
            var allusers = await _userRepository.SearchByEmailAsync(email);

            return _mapper.Map<List<UserDto>>(allusers);
        }
        public async Task<UserDto> GetByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);

            return _mapper.Map<UserDto>(user);
        }
    }
}