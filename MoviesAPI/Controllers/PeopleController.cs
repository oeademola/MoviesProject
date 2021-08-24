using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Data;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Services;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PeopleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;
        private readonly string containerName = "people";
        public PeopleController(ApplicationDbContext context, IMapper mapper, IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<PersonDTO>>> Get()
        {
            var people = await _context.People.AsNoTracking().ToListAsync();
            return _mapper.Map<List<PersonDTO>>(people);
        }

        [HttpGet("{id}", Name = "get-person")]
        public async Task<ActionResult<PersonDTO>> Get(int id)
        {
            var person = await _context.People.FirstOrDefaultAsync(x => x.Id == id);

            if (person == null)
                return NotFound();

            return _mapper.Map<PersonDTO>(person);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] PersonToCreateDTO personToCreateDTO)
        {
            var person = _mapper.Map<Person>(personToCreateDTO);

            if (personToCreateDTO.Picture != null)
            using (var memoryStream = new MemoryStream())
            {
                await personToCreateDTO.Picture.CopyToAsync(memoryStream);
                var content = memoryStream.ToArray();
                var extension = Path.GetExtension(personToCreateDTO.Picture.FileName);
                var contentType = personToCreateDTO.Picture.ContentType;
                person.Picture = await _fileStorageService.SaveFile(content, extension, containerName, contentType);
            }
            _context.People.Add(person);
            await _context.SaveChangesAsync();

            var personDTO = _mapper.Map<PersonDTO>(person);
            return new CreatedAtRouteResult("get-person", new { id = personDTO.Id }, personDTO);
        }

    }
}