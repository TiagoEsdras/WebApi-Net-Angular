using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProEventos.API.Extensions;
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

        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser()
        {
            try
            {
                var username = User.GetUserName();
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
                if(await _accountService.UserExist(user.UserName))
                    return BadRequest("Usuário já existe!");

                var userCreated = await _accountService.CrateAccountAsync(user); 
                    
                return userCreated != null ? Ok(userCreated)
                    : BadRequest("Usuário não criado.Tente novamente mais tarde");
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar registrar novo usuário. Erro: {ex.Message}");
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
                        userName = user.UserName,
                        primeiroNome = user.PrimeiroNome,
                        ultimoNome = user.UltimoNome,
                        token = _tokenService.CreateToken(user).Result
                    });
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar fazer login. Erro: {ex.Message}");
            }
        }

        [HttpPut("UpdateUser")]        
        public async Task<IActionResult> Update(UserUpdateDto userUpdate)
        {
            try
            {
                if (userUpdate.UserName != User.GetUserName())
                    return Unauthorized("Usuário inválido");

                var user = await _accountService.GetUserByUserNameAsync(User.GetUserName());

                if(user == null)
                    return BadRequest("Usuário inválido");

                userUpdate.Id = user.Id;
                var userUpdated = await _accountService.UpdateAccount(userUpdate);

                return userUpdated == null ? NoContent() : Ok(new
                {
                    userName = userUpdated.UserName,
                    primeiroNome = userUpdated.PrimeiroNome,
                    ultimoNome = userUpdated.UltimoNome,
                    token = _tokenService.CreateToken(userUpdated).Result
                });
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar atualizar usuário. Erro: {ex.Message}");
            }
        }
    }
}