using AutoMapper;
using FilmesApi.Data;
using FilmesApi.Data.Dtos;
using FilmesApi.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace FilmesApi.Controllers;

[ApiController]
[Route("[controller]")]
public class FilmeController : ControllerBase
{

    private FilmeContext _context;
    private IMapper _mapper;

    public FilmeController(FilmeContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Adiciona um filme ao banco de dados
    /// </summary>
    /// <param name="filmeDto">Objeto com os campos necessários para criação de um filme</param>
    /// <returns>IActionResult</returns>
    /// <response code="201">Caso inserção seja feita com sucesso</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult AdicionaFilme([FromBody] UpdateFilmeDto filmeDto)
    {
        Filme filme = _mapper.Map<Filme>(filmeDto);
        _context.Filmes.Add(filme);
        _context.SaveChanges();
        return CreatedAtAction(nameof(RecuperarFilmePorId), new { id = filme.Id }, filme);
    }

    /// <summary>
    /// Recupera uma lista de filmes do banco de dados.
    /// </summary>
    /// <param name="skip">Número de filmes a serem ignorados no início da lista. Valor padrão é 0.</param>
    /// <param name="take">Número máximo de filmes a serem retornados. Valor padrão é 50.</param>
    /// <returns>Uma coleção de objetos ReadFilmeDto representando os filmes recuperados.</returns>
    /// <response code="200">Caso a recuperação dos filmes seja bem-sucedida.</response>
    [HttpGet]
    public IEnumerable<ReadFilmeDto> RecuperaFilmes([FromQuery] int skip = 0,
          [FromQuery] int take = 50,
          [FromQuery] string? nomeCinema = null)
    {
        if (nomeCinema == null)
        {
            return _mapper.Map<List<ReadFilmeDto>>(_context.Filmes.Skip(skip).Take(take).ToList());
        }
        return _mapper.Map<List<ReadFilmeDto>>(_context.Filmes.Skip(skip).Take(take).Where(filme => filme.Sessoes
                .Any(sessao => sessao.Cinema.Nome == nomeCinema)).ToList());
    }

    /// <summary>
    /// Recupera um filme específico pelo seu ID.
    /// </summary>
    /// <param name="id">ID do filme a ser recuperado.</param>
    /// <returns>Um objeto ReadFilmeDto representando o filme recuperado ou NotFound se o filme não existir.</returns>
    /// <response code="200">Caso o filme seja encontrado e retornado com sucesso.</response>
    /// <response code="404">Caso o filme não seja encontrado.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult? RecuperarFilmePorId(int id)
    {
        Filme? filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
        if (filme == null) return NotFound();
        var filmeDto = _mapper.Map<ReadFilmeDto>(filme);
        return Ok(filmeDto);
    }

    /// <summary>
    /// Atualiza um filme existente pelo seu ID.
    /// </summary>
    /// <param name="id">ID do filme a ser atualizado.</param>
    /// <param name="filmeDto">Objeto com os campos atualizados do filme.</param>
    /// <returns>O filme atualizado ou NotFound se o filme não existir.</returns>
    /// <response code="200">Caso o filme seja atualizado com sucesso.</response>
    /// <response code="404">Caso o filme não seja encontrado.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult AtualizarFilme(int id, [FromBody] UpdateFilmeDto filmeDto)
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
        if (filme == null) return NotFound();
        _mapper.Map(filmeDto, filme);
        _context.SaveChanges();
        return Ok(filme);
    }

    /// <summary>
    /// Atualiza parcialmente um filme existente pelo seu ID.
    /// </summary>
    /// <param name="id">ID do filme a ser atualizado.</param>
    /// <param name="patch">Documento JSON Patch contendo as alterações a serem aplicadas.</param>
    /// <returns>O filme atualizado ou NotFound se o filme não existir.</returns>
    /// <response code="200">Caso o filme seja atualizado com sucesso.</response>
    /// <response code="400">Caso a validação do modelo falhe.</response>
    /// <response code="404">Caso o filme não seja encontrado.</response>
    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult AtualizarFilmeParcial(int id, [FromBody] JsonPatchDocument<UpdateFilmeDto> patch)
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
        if (filme == null) return NotFound();

        var filemParaAtualizar = _mapper.Map<UpdateFilmeDto>(filme);

        patch.ApplyTo(filemParaAtualizar, ModelState);
        if (!TryValidateModel(filemParaAtualizar))
        {
            return ValidationProblem(ModelState);
        }

        _mapper.Map(filemParaAtualizar, filme);
        _context.SaveChanges();
        return Ok(filme);
    }

    /// <summary>
    /// Deleta um filme existente pelo seu ID.
    /// </summary>
    /// <param name="id">ID do filme a ser deletado.</param>
    /// <returns>Uma resposta sem conteúdo (NoContent) se o filme for deletado com sucesso.</returns>
    /// <response code="204">Caso o filme seja deletado com sucesso.</response>
    /// <response code="404">Caso o filme não seja encontrado.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeletaFilme(int id)
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
        if (filme == null) return NotFound();
        _context.Remove(filme);
        _context.SaveChanges();
        return NoContent();
    }
}