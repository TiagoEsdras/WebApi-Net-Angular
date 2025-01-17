using ProEventos.Application.Dtos;
using ProEventos.Persistence.Models;
using System.Threading.Tasks;

namespace ProEventos.Application.Contratos
{
    public interface IPalestranteService
    {
        Task<PalestranteDto> AddPalestrantes(int userId, PalestranteAddDto palestranteAddDto);

        Task<PalestranteDto> UpdatePalestrante(int userId, PalestranteUpdateDto palestranteUpdateDto);

        Task<PageList<PalestranteDto>> GetAllPalestrantesAsync(PageParams pageParams, bool includeEventos = false);

        Task<PalestranteDto> GetPalestranteByUserIdAsync(int userId, bool includeEventos = false);
    }
}