using AutoMapper;
using Bogus;
using Bogus.DataSets;
using EscNet.Cryptography.Interfaces;
using FluentAssertions;
using Manager.Domain.Entities;
using Manager.Services.DTO;
using Manager.Services.Interfaces;
using Manager.Services.Services;
using Manager.Tests.Configurations.AutoMapper;
using Manager.Tests.Fixtures;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using System.Linq.Expressions;
using Manager.Infra.Interface;

namespace Manager.Tests.Projects.Services
{
    public class UserServiceTests
    {
        // Subject Under Test (Quem será testado!)
        private readonly IUserService _sut;

        //Mocks
        private readonly IMapper _mapper;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IRijndaelCryptography> _rijndaelCryptographyMock;

        public UserServiceTests()
        {
            _mapper = AutoMapperConfiguration.GetConfiguration();
            _userRepositoryMock = new Mock<IUserRepository>();
            _rijndaelCryptographyMock = new Mock<IRijndaelCryptography>();

            _sut = new UserService
            (
                mapper: _mapper,
                userRepository: _userRepositoryMock.Object,
                rijndaelCryptography: _rijndaelCryptographyMock.Object
            );
        }

        #region Create

        [Fact(DisplayName = "Create Valid User")]
        [Trait("Category", "Services")]
        public async Task Create_WhenUserIsValid_ReturnsUserDto()
        {
            // Arrange
            var userToCreate = UserFixture.CreateValidUserDTO();

            var encryptedPassword = new Lorem().Sentence();
            var userCreated = _mapper.Map<User>(userToCreate);
            userCreated.SetPassword(encryptedPassword);

            _userRepositoryMock.Setup(x => 
                x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(() => null);

            _rijndaelCryptographyMock.Setup(x => x.Encrypt(It.IsAny<string>()))
                .Returns(encryptedPassword);

            _userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync(() => userCreated);

            // Act
            var result = await _sut.CreateAsync(userToCreate);

            // Assert
            result.Should()
                .BeEquivalentTo(_mapper.Map<UserDto>(userCreated));
        }

        #endregion

        #region Update

        [Fact(DisplayName = "Update Valid User")]
        [Trait("Category", "Services")]
        public async Task Update_WhenUserIsValid_ReturnsUserDto()
        {
            // Arrange
            var oldUser = UserFixture.CreateValidUser();
            var userToUpdate = UserFixture.CreateValidUserDTO();
            var userUpdated = _mapper.Map<User>(userToUpdate);

            var encryptedPassword = new Lorem().Sentence();

            _userRepositoryMock.Setup(x => x.GetAsync(oldUser.Id))
            .ReturnsAsync(() => oldUser);

            _rijndaelCryptographyMock.Setup(x => x.Encrypt(It.IsAny<string>()))
                .Returns(encryptedPassword);

            _userRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(() => userUpdated);

            // Act
            var result = await _sut.UpdateAsync(userToUpdate);

            // Assert
            result.Should()
                .BeEquivalentTo(_mapper.Map<UserDto>(userUpdated));
        }

        #endregion

        #region Remove

        [Fact(DisplayName = "Remove User")]
        [Trait("Category", "Services")]
        public async Task Remove_WhenUserExists_RemoveUser()
        {
            // Arrange
            var userId = new Randomizer().Int(0, 1000);

            _userRepositoryMock.Setup(x => x.RemoveAsync(It.IsAny<int>()))
                .Verifiable();

            // Act
            await _sut.RemoveAsync(userId);

            // Assert
            _userRepositoryMock.Verify(x => x.RemoveAsync(userId), Times.Once);
        }

        #endregion

        #region Get

        [Fact(DisplayName = "Get By Id")]
        [Trait("Category", "Services")]
        public async Task GetById_WhenUserExists_ReturnsUserDto()
        {
            // Arrange
            var userId = new Randomizer().Long(0, 1000);
            var userFound = UserFixture.CreateValidUser();

            _userRepositoryMock.Setup(x => x.GetAsync(userId))
            .ReturnsAsync(() => userFound);

            // Act
            var result = await _sut.GetAsync(userId);

            // Assert
            result.Should()
                .BeEquivalentTo(_mapper.Map<UserDto>(userFound));
        }

        [Fact(DisplayName = "Get By Id When User Not Exists")]
        [Trait("Category", "Services")]
        public async Task GetById_WhenUserNotExists_ReturnsEmptyOptional()
        {
            // Arrange
            var userId = new Randomizer().Int(0, 1000);

            _userRepositoryMock.Setup(x => x.GetAsync(userId))
                .ReturnsAsync(() => null);

            // Act
            var result = await _sut.GetAsync(userId);

            // Assert
            result.Should()
                .BeNull();
        }

        [Fact(DisplayName = "Get By Email")]
        [Trait("Category", "Services")]
        public async Task GetByEmail_WhenUserExists_ReturnsUserDto()
        {
            // Arrange
            var userEmail = new Internet().Email();
            var userFound = UserFixture.CreateValidUser();

            _userRepositoryMock.Setup(x => 
                x.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(() => userFound);

            // Act
            var result = await _sut.GetByEmailAsync(userEmail);

            // Assert
            result.Should()
                .BeEquivalentTo(_mapper.Map<UserDto>(userFound));
        }

        [Fact(DisplayName = "Get By Email When User Not Exists")]
        [Trait("Category", "Services")]
        public async Task GetByEmail_WhenUserNotExists_ReturnsEmptyOptional()
        {
            // Arrange
            var userEmail = new Internet().Email();

            _userRepositoryMock.Setup(x => 
                x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(() => null);

            // Act
            var result = await _sut.GetByEmailAsync(userEmail);

            // Assert
            result.Should()
                .BeNull();
        }

        [Fact(DisplayName = "Get All Users")]
        [Trait("Category", "Services")]
        public async Task GetAllUsers_WhenUsersExists_ReturnsAListOfUserDto()
        {
            // Arrange
            var usersFound = UserFixture.CreateListValidUser();

            _userRepositoryMock.Setup(x => x.GetAsync())
                .ReturnsAsync(() => usersFound);

            // Act
            var result = await _sut.GetAllAsync();

            // Assert
            result.Should()
                .BeEquivalentTo(_mapper.Map<List<UserDto>>(usersFound));
        }

        [Fact(DisplayName = "Get All Users When None User Found")]
        [Trait("Category", "Services")]
        public async Task GetAllUsers_WhenNoneUserFound_ReturnsEmptyList()
        {
            // Arrange

            _userRepositoryMock.Setup(x => x.GetAsync())
                .ReturnsAsync(() => null);

            // Act
            var result = await _sut.GetAllAsync();

            // Assert
            result.Should()
                .BeEmpty();
        }

        #endregion

        #region Search

        [Fact(DisplayName = "Search By Name")]
        [Trait("Category", "Services")]
        public async Task SearchByName_WhenAnyUserFound_ReturnsAListOfUserDto()
        {
            // Arrange
            var nameToSearch = new Name().FirstName();
            var usersFound = UserFixture.CreateListValidUser();

            _userRepositoryMock.Setup(x => 
                x.SearchByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(() => usersFound);

            // Act
            var result = await _sut.SearchByNameAsync(nameToSearch);

            // Assert
            result.Should()
                .BeEquivalentTo(_mapper.Map<List<UserDto>>(usersFound));
        }

        [Fact(DisplayName = "Search By Name When None User Found")]
        [Trait("Category", "Services")]
        public async Task SearchByName_WhenNoneUserFound_ReturnsEmptyList()
        {
            // Arrange
            var nameToSearch = new Name().FirstName();

            _userRepositoryMock.Setup(x => 
                x.SearchByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(() => null);

            // Act
            var result = await _sut.SearchByNameAsync(nameToSearch);

            // Assert
            result.Should()
                .BeEmpty();
        }

        [Fact(DisplayName = "Search By Email")]
        [Trait("Category", "Services")]
        public async Task SearchByEmail_WhenAnyUserFound_ReturnsAListOfUserDto()
        {
            // Arrange
            var emailSoSearch = new Internet().Email();
            var usersFound = UserFixture.CreateListValidUser();

            _userRepositoryMock.Setup(x => 
                x.SearchByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(() => usersFound);

            // Act
            var result = await _sut.SearchByEmailAsync(emailSoSearch);

            // Assert
            result.Should()
                .BeEquivalentTo(_mapper.Map<List<UserDto>>(usersFound));
        }

        [Fact(DisplayName = "Search By Email When None User Found")]
        [Trait("Category", "Services")]
        public async Task SearchByEmail_WhenNoneUserFound_ReturnsEmptyList()
        {
            // Arrange
            var emailSoSearch = new Internet().Email();

            _userRepositoryMock.Setup(x => 
                x.SearchByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(() => null);

            // Act
            var result = await _sut.SearchByEmailAsync(emailSoSearch);

            // Assert
            result.Should()
                .BeEmpty();
        }

        #endregion
    }
}