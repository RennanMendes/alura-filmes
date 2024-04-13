using System.ComponentModel.DataAnnotations;

namespace FilmesApi.Data.Dtos;

public class CreateFilmeDto
{
    [Required(ErrorMessage = "O título é obrigatorio")]
    public string Titulo { get; set; }

    [Required(ErrorMessage = "O gênero é obrigatorio")]
    [StringLength(50, ErrorMessage = "O tamanho gênero não pode exceder 50 caracteres")]
    public string Genero { get; set; }

    [Required]
    [Range(70, 600, ErrorMessage = "A duração deve ter entre 70 e 600min")]
    public int Duracao { get; set; }
}
