using System.ComponentModel.DataAnnotations;

namespace FilmesApi.Models
{
    public class Filme
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "O título é obrigatorio")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "O gênero é obrigatorio")]
        [MaxLength(50, ErrorMessage = "O tamanho gênero não pode exceder 50 caracteres")]
        public string Genero { get; set; }

        [Required]
        [Range(70, 600, ErrorMessage = "A duração deve ter entre 70 e 600min")]
        public int Duracao { get; set; }

        public virtual ICollection<Sessao> Sessoes{ get; set; }
    }
}