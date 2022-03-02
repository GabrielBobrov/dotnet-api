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

        public async Task<UserDto> Create(UserDto userDto)
        {
            var userExists = await _userRepository.GetByEmail(userDto.Email);

            if(userExists != null)
            {
                throw new DomainException("Ja existe um usuario cadastrado com esse email");
            }
        
            var user = _mapper.Map<User>(userDto);
            user.Validate();
            user.ChangePassword(_rijndaelCryptography.Encrypt(user.Password));

            var userCreated = await _userRepository.Create(user);
            
            return _mapper.Map<UserDto>(userCreated);
        }
        public async Task<UserDto> Update(UserDto userDto)
        {
            var userExists = await _userRepository.Get(userDto.Id);

            if(userExists == null)
            {
                throw new DomainException("Nao existe nenhum usuario cadastrado com o id informado");
            }

            var user = _mapper.Map<User>(userDto);
            user.Validate();
            user.ChangePassword(_rijndaelCryptography.Encrypt(user.Password));


            var userUpdated = await _userRepository.Update(user);

            return _mapper.Map<UserDto>(userUpdated);

        }
        public async Task Remove(long id)
        {
            await _userRepository.Remove(id);
        }
        public async Task<UserDto> Get(UserDto userDto)
        {
            var user = await _userRepository.Get();

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> Get(long id)
        {
            var user = await _userRepository.Get(id);

            return _mapper.Map<UserDto>(user);
        }
        public async Task<List<UserDto>> GetAll()
        {
            var allUsers = await _userRepository.Get();

            return _mapper.Map<List<UserDto>>(allUsers);
        }
        public async Task<List<UserDto>> SearchByName(string name)
        {
            var allusers = await _userRepository.SearchByName(name);

            return _mapper.Map<List<UserDto>>(allusers);
        }
        public async Task<List<UserDto>> SearchByEmail(string email)
        {
            var allusers = await _userRepository.GetByEmail(email);

            return _mapper.Map<List<UserDto>>(allusers);
        }
        public async Task<UserDto> GetByEmail(string email)
        {
            var user = await _userRepository.GetByEmail(email);

            return _mapper.Map<UserDto>(user);
        }
    }
}