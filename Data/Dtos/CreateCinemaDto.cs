using System.ComponentModel.DataAnnotations;

namespace FilmesApi.Data.Dtos
{
    public class CreateCinemaDto
    {

        [Required(ErrorMessage = "O campo de erro é obrigatório!")]
        public string Nome { get; set; }
    }
}
