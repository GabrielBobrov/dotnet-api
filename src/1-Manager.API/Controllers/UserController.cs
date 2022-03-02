using Manager.API.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Manager.Core.Exceptions;
using System;
using Manager.Services.Interfaces;
using AutoMapper;
using Manager.Services.DTO;
using Manager.API.Utilites;
using Microsoft.AspNetCore.Authorization;

namespace Manager.API.Controllers
{
    [ApiController]
    [Route("/api/v1/users")]
    public class UserController : ControllerBase
    {
        public readonly IMapper _mapper;
        private readonly IUserService _userService;

        public UserController(IMapper mapper, IUserService userService)
        {
            _mapper = mapper;
            _userService = userService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateUserViewModel viewModel)
        {
            try
            {
                var userDto = _mapper.Map<UserDto>(viewModel);

                var userCreadted = await _userService.CreateAsync(userDto);

                return Ok(new ResultViewModel{
                    Message = "Usuario criado com sucesso",
                    Success = true,
                    Data = userCreadted
                });
            }
            catch (DomainException ex)
            {
                
                return BadRequest(Responses.DomainErrorMessage(ex.Message,ex.Errors));
            }
            catch (Exception e)
            {
                
                return StatusCode(500, e);
            }
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateUserViewModel userViewModel)
        {
            var userDto = _mapper.Map<UserDto>(userViewModel);
            var userUpdated = await _userService.UpdateAsync(userDto);

            return Ok(new ResultViewModel
            {
                Message = "Usuário atualizado com sucesso!",
                Success = true,
                Data = userUpdated
            });
        }

        [HttpDelete]
        [Authorize]
        [Route("{id}")]
        public async Task<IActionResult> RemoveAsync(long id)
        {
            await _userService.RemoveAsync(id);

            return Ok(new ResultViewModel
            {
                Message = "Usuário removido com sucesso!",
                Success = true,
                Data = null
            });
        }

        [HttpGet]
        [Authorize]
        [Route("{id}")]
        public async Task<IActionResult> GetAsync(long id)
        {
            var user = await _userService.GetAsync(id);

            if (user == null)
            {
                return Ok(new ResultViewModel
                {
                    Message = "Nenhum usuário foi encontrado com o ID informado.",
                    Success = true,
                    Data = user
                });
            }

            return Ok(new ResultViewModel
            {
                Message = "Usuário encontrado com sucesso!",
                Success = true,
                Data = user
            });
        }


        [HttpGet]
        [Authorize]
        [Route("get-all")]
        public async Task<IActionResult> GetAsync()
        {
            var allUsers = await _userService.GetAllAsync();


            return Ok(new ResultViewModel
            {
                Message = "Usuários encontrados com sucesso!",
                Success = true,
                Data = allUsers
            });
        }


        [HttpGet]
        [Authorize]
        [Route("get-by-email")]
        public async Task<IActionResult> GetByEmailAsync([FromQuery] string email)
        {
            var user = await _userService.GetByEmailAsync(email);

            

            if (user == null)
            {
                return Ok(new ResultViewModel
                {
                    Message = "Nenhum usuário foi encontrado com o email informado.",
                    Success = true,
                    Data = user
                });
            }

            return Ok(new ResultViewModel
            {
                Message = "Usuário encontrado com sucesso!",
                Success = true,
                Data = user
            });
        }

        [HttpGet]
        [Authorize]
        [Route("search-by-name")]
        public async Task<IActionResult> SearchByNameAsync([FromQuery] string name)
        {
            var allUsers = await _userService.SearchByNameAsync(name);

            if (allUsers == null)
            {
                return Ok(new ResultViewModel
                {
                    Message = "Nenhum usuário foi encontrado com o nome informado",
                    Success = true,
                    Data = null
                });
            }

            return Ok(new ResultViewModel
            {
                Message = "Usuário encontrado com sucesso!",
                Success = true,
                Data = allUsers
            });
        }


        [HttpGet]
        [Authorize]
        [Route("search-by-email")]
        public async Task<IActionResult> SearchByEmailAsync([FromQuery] string email)
        {
            var allUsers = await _userService.SearchByEmailAsync(email);

            if (allUsers == null)
            {
                return Ok(new ResultViewModel
                {
                    Message = "Nenhum usuário foi encontrado com o email informado",
                    Success = true,
                    Data = null
                });
            }

            return Ok(new ResultViewModel
            {
                Message = "Usuário encontrado com sucesso!",
                Success = true,
                Data = allUsers
            });
        }
    }
}