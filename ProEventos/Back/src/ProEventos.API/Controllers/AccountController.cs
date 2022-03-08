using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProEventos.Application.Contratos;

namespace ProEventos.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]

    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ITokenService _tokenService;

        public AccountController(
            IAccountService accountService,
            ITokenService tokenService
        )
        {
            _accountService = accountService;
            _tokenService = tokenService;
        }

        [HttpGet("GetUser/{username}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUser(string username)
        {
            try
            {
                var user = await _accountService.GetUserByUserNameAsync(username);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar recuperar usuário. Erro: {ex.Message}");
            }
        }
    }
}