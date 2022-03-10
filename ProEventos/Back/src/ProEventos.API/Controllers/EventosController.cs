using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProEventos.Application.Contratos;
using ProEventos.Domain;
using Microsoft.AspNetCore.Http;
using ProEventos.Application.Dtos;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using ProEventos.API.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace ProEventos.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EventosController : ControllerBase
    {
        private readonly IEventoService _eventoService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IAccountService _accountService;

        public EventosController(
            IEventoService eventoService,
            IWebHostEnvironment webHostEnvironment,
            IAccountService accountService
        )
        {
            _eventoService = eventoService;
            _webHostEnvironment = webHostEnvironment;
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var userId = User.GetUserId();
                var eventos = await _eventoService.GetAllEventosAsync(userId, true);
                if(eventos == null) return NoContent();

                return Ok(eventos);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar recuperar os eventos. Erro: {ex.Message}");
            }
        }

        [HttpGet("{id}")]        
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var userId = User.GetUserId();
                var evento = await _eventoService.GetEventoByIdAsync(userId, id, true);
                if(evento == null) return NoContent();
                
                return Ok(evento);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar recuperar o evento do Id: {id}. Erro: {ex.Message}");
            }
        }

        [HttpGet("{tema}/tema")]
        public async Task<IActionResult> GetByTema(string tema)
        {
            try
            {
                var userId = User.GetUserId();
                var eventos = await _eventoService.GetAllEventosByTemaAsync(userId, tema, true);
                if(eventos == null) return NoContent();

                return Ok(eventos);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar recuperar o eventos com Tema: {tema}. Erro: {ex.Message}");
            }
        }

         [HttpPost("upload-image/{eventoId}")]
        public async Task<IActionResult> UploadImage(int eventoId)
        {
            try
            {
                var userId = User.GetUserId();
                var evento = await _eventoService.GetEventoByIdAsync(userId, eventoId, true);
                if(evento == null) return NoContent();

                var file = Request.Form.Files[0];

                if(file.Length > 0) {
                    DeleteImage(evento.ImageURL);
                    evento.ImageURL = await SaveImage(file);
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
                if(evento == null) return NoContent();

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
                if(evento == null) return NoContent();

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
                if(evento == null) return NoContent();

                if(await _eventoService.DeleteEvento(userId, id)) {
                    DeleteImage(evento.ImageURL);
                    return  Ok(true);
                }
                else {
                    throw new Exception("Ocorreu um erro ao tentar deletar o evento!");
                }
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar deletar o evento. Erro: {ex.Message}");
            }
        }

        [NonAction]
        public void DeleteImage(string imageName) {
            var imagePath = Path.Combine(_webHostEnvironment.ContentRootPath, @"Resources/images", imageName);
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);
        }

        [NonAction]
        public async Task<string> SaveImage(IFormFile imageFile) {

            string imageName = new String(Path.GetFileNameWithoutExtension(imageFile.FileName)
                .Take(10)
                .ToArray()
            ).Replace(' ', '-');

            imageName = $"{imageName}{DateTime.UtcNow.ToString("yymmssfff")}{Path.GetExtension(imageFile.FileName)}";

            var imagePath = Path.Combine(_webHostEnvironment.ContentRootPath, @"Resources/images", imageName);

            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return imageName;
        }
    }
}
