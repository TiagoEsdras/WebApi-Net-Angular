using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProEventos.Api.Helpers;
using ProEventos.API.Extensions;
using ProEventos.Application.Contratos;
using ProEventos.Application.Dtos;
using ProEventos.Persistence.Models;
using System;
using System.Threading.Tasks;

namespace ProEventos.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EventosController : ControllerBase
    {
        private readonly IEventoService _eventoService;
        private readonly IUtil _util;

        private readonly string _destino = "Images";

        public EventosController(
            IEventoService eventoService,
            IUtil util)
        {
            _eventoService = eventoService;
            _util = util;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PageParams pageParams)
        {
            try
            {
                var eventos = await _eventoService.GetAllEventosAsync(User.GetUserId(), pageParams, true);

                Response.AddPagination(eventos.CurrentPage, eventos.PageSize, eventos.TotalCount, eventos.TotalPages);

                return Ok(eventos);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar recuperar eventos. Erro: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var userId = User.GetUserId();
                var evento = await _eventoService.GetEventoByIdAsync(userId, id, true);
                if (evento == null) return NoContent();

                return Ok(evento);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar recuperar o evento do Id: {id}. Erro: {ex.Message}");
            }
        }

        [HttpPost("upload-image/{eventoId}")]
        public async Task<IActionResult> UploadImage(int eventoId)
        {
            try
            {
                var userId = User.GetUserId();
                var evento = await _eventoService.GetEventoByIdAsync(userId, eventoId, true);
                if (evento == null) return NoContent();

                var file = Request.Form.Files[0];

                if (file.Length > 0)
                {
                    _util.DeleteImage(evento.ImageURL, _destino);
                    evento.ImageURL = await _util.SaveImage(file, _destino);
                }

                var eventoRetorno = await _eventoService.UpdateEvento(userId, eventoId, evento);

                return Ok(eventoRetorno);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar fazer upload da imagem. Erro: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(EventoDto model)
        {
            try
            {
                var userId = User.GetUserId();
                var evento = await _eventoService.AddEventos(userId, model);
                if (evento == null) return NoContent();

                return Ok(evento);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar adicionar o evento. Erro: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, EventoDto model)
        {
            try
            {
                var userId = User.GetUserId();
                var evento = await _eventoService.UpdateEvento(userId, id, model);
                if (evento == null) return NoContent();

                return Ok(evento);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar atualizar o evento. Erro: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userId = User.GetUserId();
                var evento = await _eventoService.GetEventoByIdAsync(userId, id, true);
                if (evento == null) return NoContent();

                if (await _eventoService.DeleteEvento(userId, id))
                {
                    _util.DeleteImage(evento.ImageURL, _destino);
                    return Ok(true);
                }
                else
                {
                    throw new Exception("Ocorreu um erro ao tentar deletar o evento!");
                }
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar deletar o evento. Erro: {ex.Message}");
            }
        }
    }
}