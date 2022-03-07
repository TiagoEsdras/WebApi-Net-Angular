using System.Collections.Generic;
using System.Threading.Tasks;
using ProEventos.Domain.Identity;
using ProEventos.Persistence.Contextos;
using ProEventos.Persistence.Contratos;

namespace ProEventos.Persistence
{
    public class UserPersist : GeralPersist, IUserPersist
    {
        private readonly ProEventosContext _context;

        public UserPersist(ProEventosContext context) : base(context)
        {
            _context = context;
        }
        public Task<IEnumerable<User>> GetUsersAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<User> GetUserByIdAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<User> GetUserByUsernameAsync(string username)
        {
            throw new System.NotImplementedException();
        }
    }
}