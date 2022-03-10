using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProEventos.Application.Dtos {
    public class EventoDto {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Local { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string DataEvento { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [MinLength(4, ErrorMessage = "O campo {0} deve ter no mínimo 4 caracteres")]
        [MaxLength(50, ErrorMessage = "O campo {0} deve ter no máximo 50 caracteres")]
        public string Tema { get; set; }

        [Display(Name = "Qtd Pessoas")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [Range(1, 120000, ErrorMessage = "O campo {0} não pode der menor que 1 e maior que 120.000")]
        public int QtdPessoas { get; set; }

        [Display(Name = "Imagem")]
        [RegularExpression(@".*\.(gif|jpe?g|bmp|png)$", ErrorMessage = "Não é uma imagem válida. (gif, jpg, jpeg, bmp ou png)")]
        public string ImageURL { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [Phone(ErrorMessage = "O campo {0} está com número inválido")]
        public string Telefone { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [EmailAddress(ErrorMessage = "O campo {0} precisa ser um email válido")]
        public string Email { get; set; }
        public int UserId { get; set; }
        public UserDto UserDto { get; set; }
        public IEnumerable<LoteDto> Lotes { get; set; }
        public IEnumerable<RedeSocialDto> RedesSociais { get; set; }
        public IEnumerable<PalestranteDto> PalestrantesEventos { get; set; }
    }
}