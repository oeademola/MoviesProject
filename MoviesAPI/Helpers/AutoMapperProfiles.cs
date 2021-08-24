using AutoMapper;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;

namespace MoviesAPI.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Genre, GenreDTO>();
            CreateMap<GenreToCreateDTO, Genre>();
            CreateMap<Person, PersonDTO>();
            CreateMap<PersonToCreateDTO, Person>()
                .ForMember(x => x.Picture, options => options.Ignore());
        }
    }
}