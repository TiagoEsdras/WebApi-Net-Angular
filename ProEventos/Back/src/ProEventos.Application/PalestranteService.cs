using AutoMapper;
using ProEventos.Application.Contratos;
using ProEventos.Application.Dtos;
using ProEventos.Domain;
using ProEventos.Persistence.Contratos;
using ProEventos.Persistence.Models;
using System.Threading.Tasks;

namespace ProEventos.Application
{
    public class PalestranteService : IPalestranteService
    {
        private readonly IPalestrantePersist _palestrantePersist;
        private readonly IMapper _mapper;

        public PalestranteService(IPalestrantePersist palestrantePersist,
                                  IMapper mapper)
        {
            _palestrantePersist = palestrantePersist;
            _mapper = mapper;
        }

        public async Task<PalestranteDto> AddPalestrantes(int userId, PalestranteAddDto palestranteAddDto)
        {
            var palestrante = _mapper.Map<Palestrante>(palestranteAddDto);
            palestrante.UserId = userId;

            _palestrantePersist.Add<Palestrante>(palestrante);

            if (await _palestrantePersist.SaveChangesAsync())
            {
                var palestranteRetorno = await _palestrantePersist.GetPalestranteByUserIdAsync(userId, false);

                return _mapper.Map<PalestranteDto>(palestranteRetorno);
            }
            return null;
        }

        public async Task<PalestranteDto> UpdatePalestrante(int userId, PalestranteUpdateDto palestranteUpdateDto)
        {
            var palestrante = await _palestrantePersist.GetPalestranteByUserIdAsync(userId, false);
            if (palestrante == null) return null;

            palestranteUpdateDto.Id = palestrante.Id;
            palestranteUpdateDto.UserId = userId;

            _mapper.Map(palestranteUpdateDto, palestrante);

            _palestrantePersist.Update<Palestrante>(palestrante);

            if (await _palestrantePersist.SaveChangesAsync())
            {
                var palestranteRetorno = await _palestrantePersist.GetPalestranteByUserIdAsync(userId, false);

                return _mapper.Map<PalestranteDto>(palestranteRetorno);
            }
            return null;
        }

        public async Task<PageList<PalestranteDto>> GetAllPalestrantesAsync(PageParams pageParams, bool includeEventos = false)
        {
            var palestrantes = await _palestrantePersist.GetAllPalestrantesAsync(pageParams, includeEventos);
            if (palestrantes == null) return null;

            var resultado = _mapper.Map<PageList<PalestranteDto>>(palestrantes);

            resultado.CurrentPage = palestrantes.CurrentPage;
            resultado.TotalPages = palestrantes.TotalPages;
            resultado.PageSize = palestrantes.PageSize;
            resultado.TotalCount = palestrantes.TotalCount;

            return resultado;
        }

        public async Task<PalestranteDto> GetPalestranteByUserIdAsync(int userId, bool includeEventos = false)
        {
            var palestrante = await _palestrantePersist.GetPalestranteByUserIdAsync(userId, includeEventos);
            if (palestrante == null) return null;

            var resultado = _mapper.Map<PalestranteDto>(palestrante);

            return resultado;
        }
    }
}