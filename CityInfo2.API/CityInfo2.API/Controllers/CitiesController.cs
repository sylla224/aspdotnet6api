using AutoMapper;
using CityInfo.API.Entities;
using CityInfo2.API.Models;
using CityInfo2.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CityInfo2.API.Controllers
{
    [Route("api/cities")]
    [ApiController]
    [Authorize]
    public class CitiesController : ControllerBase
    {
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;
        const int maxCitiespagesize = 20;

        public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper )
        {
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper?? throw new ArgumentException(nameof(mapper));
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities([FromQuery] string? name, string? serachQuery,
                                                                                               int pageNumber=1, int pageSize=10)
        {
            if (pageSize > maxCitiespagesize)
            {
                pageSize = maxCitiespagesize;
            }
            var (cities,paginationmetadeta) = await _cityInfoRepository.GetCitiesAsync(name, serachQuery,pageNumber, pageSize);

            Response.Headers.Add("X-pagination", JsonSerializer.Serialize(paginationmetadeta));
           
            return Ok(_mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cities));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> getCity(int id, bool includepointofinterest=false)
        {
          var city= await  _cityInfoRepository.GetCityAsync(id, includepointofinterest);
            if (city == null)
            {
                return NotFound();
            }
            if (includepointofinterest)
            {
                return Ok(_mapper.Map<CityDto>(city));
            }
            return Ok(_mapper.Map<CityWithoutPointsOfInterestDto>(city));
            
       }
    }
}
