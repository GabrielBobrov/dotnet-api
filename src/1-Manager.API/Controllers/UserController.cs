using Manager.API.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Manager.Core.Exceptions;
using System;
namespace Manager.API.Controllers
{
    [ApiController]
    [Route("/api/v1/user")]
    public class UserController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserViewModel viewModel)
        {
            try
            {
                return Ok();
            }
            catch (DomainException ex)
            {
                
                return BadRequest(ex);
            }
            catch (Exception)
            {
                
                return StatusCode(500,"Erro");
            }
        }
    }
}