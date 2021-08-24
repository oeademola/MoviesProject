using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoviesAPI.Data;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenresController : ControllerBase
    {
        private readonly ILogger<GenresController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public GenresController(ILogger<GenresController> logger, ApplicationDbContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
            _logger = logger;

        }

        [HttpGet]
        public async Task<ActionResult<List<GenreDTO>>> Get()
        {
            var genres = await _context.Genres.AsNoTracking().ToListAsync();
            var genresDTO = _mapper.Map<List<GenreDTO>>(genres);
            return genresDTO;
        }

        [HttpGet("{id}", Name = "get-genre")]
        public async Task<ActionResult<GenreDTO>> GetGenre(int id)
        {
            var genre = await _context.Genres.FirstOrDefaultAsync(x => x.Id == id);

            if (genre == null)
                return NotFound();
            var genreDTO = _mapper.Map<GenreDTO>(genre);
            return genreDTO;
        }

        [HttpPost]
        public async Task<ActionResult> Post(GenreToCreateDTO genreToCreateDTO)
        {
            var genre = _mapper.Map<Genre>(genreToCreateDTO);
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            var genreDTO = _mapper.Map<GenreDTO>(genre);

            return new CreatedAtRouteResult("get-genre", new { id = genreDTO.Id }, genreDTO);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, GenreToCreateDTO genreToCreateDTO)
        {
            var genre = _mapper.Map<Genre>(genreToCreateDTO);
            genre.Id = id;
            _context.Entry(genre).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete ("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var isExist = await _context.Genres.AnyAsync(x => x.Id == id);

            if (!isExist)
                return NotFound();
            _context.Genres.Remove(new Genre() {Id = id});
            await _context.SaveChangesAsync(); 
            return NoContent();
        }

    }
}