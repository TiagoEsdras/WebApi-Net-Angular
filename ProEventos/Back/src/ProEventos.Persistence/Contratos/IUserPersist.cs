using System.Collections.Generic;
using System.Threading.Tasks;
using ProEventos.Domain;
using ProEventos.Domain.Identity;

namespace ProEventos.Persistence.Contratos
{
    public interface IUserPersist : IGeralPersist
    {
       Task<IEnumerable<User>> GetUsersAsync();
       Task<User> GetUserByIdAsync(int id);
       Task<User> GetUserByUsernameAsync(string username);
    }
}