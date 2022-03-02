using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using EscNet.Cryptography.Interfaces;
using Manager.Core.Communication.Mediator.Interfaces;
using Manager.Core.Communication.Messages.Notifications;
using Manager.Core.Enums;
using Manager.Core.Structs;
using Manager.Core.Validations.Message;
using Manager.Domain.Entities;
using Manager.Infra.Interfaces;
using Manager.Services.DTO;
using Manager.Services.Interfaces;

namespace Manager.Services.Services
{
    public class UserService : IUserService{
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IRijndaelCryptography _rijndaelCryptography;
        private readonly IMediatorHandler _mediator;

        public UserService(
            IMapper mapper,
            IUserRepository userRepository,
            IRijndaelCryptography rijndaelCryptography, 
            IMediatorHandler mediator)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _rijndaelCryptography = rijndaelCryptography;
            _mediator = mediator;
        }

        public async Task<Optional<UserDto>> CreateAsync(UserDto userDto)
        {
            Expression<Func<User, bool>> filter = user 
                => user.Email.ToLower() == userDto.Email.ToLower();

            var userExists = await _userRepository.GetAsync(filter);

            if (userExists != null)
            {
                await _mediator.PublishDomainNotificationAsync(new DomainNotification(
                    ErrorMessages.UserAlreadyExists,
                    DomainNotificationType.UserAlreadyExists));

                return new Optional<UserDto>();
            }   

            var user = _mapper.Map<User>(userDto);
            user.Validate();

            if (!user.IsValid)
            {
                await _mediator.PublishDomainNotificationAsync(new DomainNotification(
                   ErrorMessages.UserInvalid(user.ErrorsToString()),
                   DomainNotificationType.UserInvalid));

                return new Optional<UserDto>();
            }

            user.SetPassword(_rijndaelCryptography.Encrypt(user.Password));

            var userCreated = await _userRepository.CreateAsync(user);

            return _mapper.Map<UserDto>(userCreated);
        }

        public async Task<Optional<UserDto>> UpdateAsync(UserDto userDto){
            var userExists = await _userRepository.GetAsync(userDto.Id);

            if (userExists == null)
            {
                await _mediator.PublishDomainNotificationAsync(new DomainNotification(
                   ErrorMessages.UserNotFound,
                   DomainNotificationType.UserNotFound));

                return new Optional<UserDto>();
            }

            var user = _mapper.Map<User>(userDto);
            user.Validate();

            if (!user.IsValid)
            {
                await _mediator.PublishDomainNotificationAsync(new DomainNotification(
                   ErrorMessages.UserInvalid(user.ErrorsToString()),
                   DomainNotificationType.UserInvalid));

                return new Optional<UserDto>();
            }

            user.SetPassword(_rijndaelCryptography.Encrypt(user.Password));

            var userUpdated = await _userRepository.UpdateAsync(user);

            return _mapper.Map<UserDto>(userUpdated);
        }
        public async Task RemoveAsync(long id)
            => await _userRepository.RemoveAsync(id);

        public async Task<Optional<UserDto>> GetAsync(long id){
            var user = await _userRepository.GetAsync(id);

            return _mapper.Map<UserDto>(user);
        }

        public async Task<Optional<IList<UserDto>>> GetAllAsync(){
            var allUsers = await _userRepository.GetAllAsync();
            var allUsersDTO = _mapper.Map<IList<UserDto>>(allUsers);

            return new Optional<IList<UserDto>>(allUsersDTO);
        }

        public async Task<Optional<IList<UserDto>>> SearchByNameAsync(string name){
            Expression<Func<User, bool>> filter = u 
                => u.Name.ToLower().Contains(name.ToLower());

            var allUsers = await _userRepository.SearchAsync(filter);
            var allUsersDTO = _mapper.Map<IList<UserDto>>(allUsers);

            return new Optional<IList<UserDto>>(allUsersDTO);
        }

        public async Task<Optional<IList<UserDto>>> SearchByEmailAsync(string email){
            Expression<Func<User, bool>> filter = user
                => user.Email.ToLower().Contains(email.ToLower());

            var allUsers = await _userRepository.SearchAsync(filter);
            var allUsersDTO = _mapper.Map<IList<UserDto>>(allUsers);

            return new Optional<IList<UserDto>>(allUsersDTO);
        }

        public async Task<Optional<UserDto>> GetByEmailAsync(string email){
            Expression<Func<User, bool>> filter = user
                => user.Email.ToLower() == email.ToLower();

            var user = await _userRepository.GetAsync(filter);
            var userDto = _mapper.Map<UserDto>(user);

            return new Optional<UserDto>(userDto);
        }
    }
}