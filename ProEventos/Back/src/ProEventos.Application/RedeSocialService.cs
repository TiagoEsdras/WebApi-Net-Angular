using AutoMapper;
using ProEventos.Application.Contratos;
using ProEventos.Application.Dtos;
using ProEventos.Domain;
using ProEventos.Persistence.Contratos;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProEventos.Application
{
    public class RedeSocialService : IRedeSocialService
    {
        private readonly IRedeSocialPersist _redeSocialPersist;
        private readonly IMapper _mapper;

        public RedeSocialService(IRedeSocialPersist redeSocialPersist,
                           IMapper mapper)
        {
            _redeSocialPersist = redeSocialPersist;
            _mapper = mapper;
        }

        public async Task<RedeSocialDto[]> SaveByEvento(int eventoId, RedeSocialDto[] redeSocialDtos)
        {
            var redeSocials = await _redeSocialPersist.GetAllByEventoIdAsync(eventoId);
            if (redeSocials == null) return null;

            foreach (var redeSocialDto in redeSocialDtos)
            {
                if (redeSocialDto.Id == 0)
                {
                    await AddRedeSocial(eventoId, redeSocialDto, true);
                }
                else
                {
                    var redeSocial = redeSocials.FirstOrDefault(redeSocial => redeSocial.Id == redeSocialDto.Id);
                    redeSocialDto.EventoId = eventoId;

                    _mapper.Map(redeSocialDto, redeSocial);

                    _redeSocialPersist.Update<RedeSocial>(redeSocial);

                    await _redeSocialPersist.SaveChangesAsync();
                }
            }

            var RedeSocialRetorno = await _redeSocialPersist.GetAllByEventoIdAsync(eventoId);

            return _mapper.Map<RedeSocialDto[]>(RedeSocialRetorno);
        }

        public async Task<RedeSocialDto[]> SaveByPalestrante(int palestranteId, RedeSocialDto[] redeSocialDtos)
        {
            var redeSocials = await _redeSocialPersist.GetAllByPalestranteIdAsync(palestranteId);
            if (redeSocials == null) return null;

            foreach (var redeSocialDto in redeSocialDtos)
            {
                if (redeSocialDto.Id == 0)
                {
                    await AddRedeSocial(palestranteId, redeSocialDto, false);
                }
                else
                {
                    var redeSocial = redeSocials.FirstOrDefault(redeSocial => redeSocial.Id == redeSocialDto.Id);
                    redeSocialDto.PalestranteId = palestranteId;

                    _mapper.Map(redeSocialDto, redeSocial);

                    _redeSocialPersist.Update<RedeSocial>(redeSocial);

                    await _redeSocialPersist.SaveChangesAsync();
                }
            }

            var redeSocialRetorno = await _redeSocialPersist.GetAllByPalestranteIdAsync(palestranteId);

            return _mapper.Map<RedeSocialDto[]>(redeSocialRetorno);
        }

        public async Task<bool> DeleteByEvento(int eventoId, int redeSocialId)
        {
            var redeSocial = await _redeSocialPersist.GetRedeSocialEventoByIdsAsync(eventoId, redeSocialId);
            if (redeSocial == null)
                throw new Exception("Rede Social por Evento para delete não encontrado.");

            _redeSocialPersist.Delete<RedeSocial>(redeSocial);
            return await _redeSocialPersist.SaveChangesAsync();
        }

        public async Task<bool> DeleteByPalestrante(int palestranteId, int redeSocialId)
        {
            var redeSocial = await _redeSocialPersist.GetRedeSocialPalestranteByIdsAsync(palestranteId, redeSocialId);
            if (redeSocial == null)
                throw new Exception("Rede Social por Palestrante para delete não encontrado.");

            _redeSocialPersist.Delete<RedeSocial>(redeSocial);
            return await _redeSocialPersist.SaveChangesAsync();
        }

        public async Task<RedeSocialDto[]> GetAllByEventoIdAsync(int eventoId)
        {
            var redeSocials = await _redeSocialPersist.GetAllByEventoIdAsync(eventoId);
            if (redeSocials == null) return null;

            var resultado = _mapper.Map<RedeSocialDto[]>(redeSocials);

            return resultado;
        }

        public async Task<RedeSocialDto[]> GetAllByPalestranteIdAsync(int palestranteId)
        {
            var redeSocials = await _redeSocialPersist.GetAllByPalestranteIdAsync(palestranteId);
            if (redeSocials == null) return null;

            var resultado = _mapper.Map<RedeSocialDto[]>(redeSocials);

            return resultado;
        }

        public async Task<RedeSocialDto> GetRedeSocialEventoByIdsAsync(int eventoId, int redeSocialId)
        {
            var redeSocial = await _redeSocialPersist.GetRedeSocialEventoByIdsAsync(eventoId, redeSocialId);
            if (redeSocial == null) return null;

            var resultado = _mapper.Map<RedeSocialDto>(redeSocial);

            return resultado;
        }

        public async Task<RedeSocialDto> GetRedeSocialPalestranteByIdsAsync(int palestranteId, int redeSocialId)
        {
            var redeSocial = await _redeSocialPersist.GetRedeSocialPalestranteByIdsAsync(palestranteId, redeSocialId);
            if (redeSocial == null) return null;

            var resultado = _mapper.Map<RedeSocialDto>(redeSocial);

            return resultado;
        }

        private async Task AddRedeSocial(int Id, RedeSocialDto redeSocialDto, bool isEvento)
        {
            var redeSocial = _mapper.Map<RedeSocial>(redeSocialDto);
            if (isEvento)
            {
                redeSocial.EventoId = Id;
                redeSocial.PalestranteId = null;
            }
            else
            {
                redeSocial.EventoId = null;
                redeSocial.PalestranteId = Id;
            }

            _redeSocialPersist.Add<RedeSocial>(redeSocial);

            await _redeSocialPersist.SaveChangesAsync();
        }
    }
}