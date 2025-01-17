using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ProEventos.Api.Helpers
{
    public interface IUtil
    {
        Task<string> SaveImage(IFormFile imageFile, string destino);

        void DeleteImage(string imageName, string detino);
    }
}