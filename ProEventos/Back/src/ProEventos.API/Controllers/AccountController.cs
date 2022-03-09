using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProEventos.Application.Contratos;
using ProEventos.Application.Dtos;

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

        
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserDto user)
        {
            try
            {
                if(await _accountService.UserExist(user.UserName)) {
                    return BadRequest("Usuário já existe!");
                }

                var userCreated = await _accountService.CrateAccountAsync(user); 
                    
                return userCreated != null ? Ok(userCreated)
                    : BadRequest("Usuário não criado.Tente novamente mais tarde");
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar recuperar usuário. Erro: {ex.Message}");
            }
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginDto userLogin)
        {
            try
            {
                var user = await _accountService.GetUserByUserNameAsync(userLogin.Username);

                if(user == null) 
                    return BadRequest("Usuário inválido");
                
                var result = await _accountService.CheckUserPasswordAsync(user, userLogin.Password);

                return !result.Succeeded ? Unauthorized("Senha incorreta")
                    : Ok(new {
                        userName = user.Username,
                        primeiroNome = user.PrimeiroNome,
                        token = _tokenService.CreateToken(user).Result
                    });
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar recuperar usuário. Erro: {ex.Message}");
            }
        }
    }
}