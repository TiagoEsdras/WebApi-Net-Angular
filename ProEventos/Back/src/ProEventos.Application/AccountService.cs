using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProEventos.Application.Contratos;
using ProEventos.Application.Dtos;
using ProEventos.Domain.Identity;
using ProEventos.Persistence.Contratos;
using System;
using System.Threading.Tasks;

namespace ProEventos.Application
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;
        private readonly IUserPersist _userPersist;

        public AccountService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IMapper mapper,
            IUserPersist userPersist
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userPersist = userPersist;
            _mapper = mapper;
        }

        public async Task<SignInResult> CheckUserPasswordAsync(UserUpdateDto userUpdadeDto, string password)
        {
            try
            {
                var user = await _userManager.Users.SingleOrDefaultAsync(
                    user => user.UserName == userUpdadeDto.UserName.ToLower()
                );

                return await _signInManager.CheckPasswordSignInAsync(user, password, false);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao tentar verificar password. Erro: {ex.Message}");
            }
        }

        public async Task<UserDto> CrateAccountAsync(UserDto userDto)
        {
            try
            {
                var user = _mapper.Map<User>(userDto);
                var result = await _userManager.CreateAsync(user, userDto.Password);

                if (result.Succeeded)
                {
                    return _mapper.Map<UserDto>(user);
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao tentar criar usuário. Erro: {ex.Message}");
            }
        }

        public async Task<UserUpdateDto> GetUserByUserNameAsync(string username)
        {
            try
            {
                var user = await _userPersist.GetUserByUserNameAsync(username);
                if (user == null)
                    return null;

                return _mapper.Map<UserUpdateDto>(user);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao tentar obter usuário por username. Erro: {ex.Message}");
            }
        }

        public async Task<UserUpdateDto> UpdateAccount(UserUpdateDto userUpdateDto)
        {
            try
            {
                var user = await _userPersist.GetUserByIdAsync(userUpdateDto.Id);
                if (user == null)
                    return null;

                userUpdateDto.Id = user.Id;
                _mapper.Map(userUpdateDto, user);

                if (userUpdateDto.Password is not null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    await _userManager.ResetPasswordAsync(user, token, userUpdateDto.Password);
                }

                _userPersist.Update<User>(user);

                if (await _userPersist.SaveChangesAsync())
                {
                    var userRetorno = await _userPersist.GetUserByIdAsync(user.Id);

                    return _mapper.Map<UserUpdateDto>(user);
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao tentar atualizar usuário. Erro: {ex.Message}");
            }
        }

        public async Task<bool> UserExist(string username)
        {
            try
            {
                return await _userManager.Users.AnyAsync(
                    user => user.UserName == username.ToLower()
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao tentar verificar se usuário existe. Erro: {ex.Message}");
            }
        }
    }
}