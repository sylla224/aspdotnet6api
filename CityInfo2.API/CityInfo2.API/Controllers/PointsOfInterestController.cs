using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Services;
using CityInfo2.API.Dbcontexts;
using CityInfo2.API.Models;
using CityInfo2.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo2.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    public class PointsOfInterestController : Controller
    {
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly LocalMailService _localmailservice;
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, LocalMailService localMailService, ICityInfoRepository cityInfoRepository, IMapper mapper )
        {
            _logger = logger;
            _localmailservice = localMailService?? throw new ArgumentNullException(nameof(localMailService));
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentException(nameof(mapper));
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointOfInterest(int cityId)
        {
            if (! await _cityInfoRepository.CityExistAsync(cityId))
            {
                _logger.LogInformation($"city with id {cityId} wasnt found when accessing point of interest");
                return NotFound();
            }
            var pointsofInterestforCity = await _cityInfoRepository.GetPointsOfInterestForCityAsync(cityId);
            return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsofInterestforCity));
        }

        [HttpGet("{pointofinterestid}", Name = "GetPointOfInterest")]
        public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(
            int cityId, int pointofinterestid)
        {
            if (!await _cityInfoRepository.CityExistAsync(cityId))
            {
                _logger.LogInformation($"city with id {cityId} wasnt found when accessing point of interest");
                return NotFound();
            }
            var pointofinterest = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointofinterestid);
            if (pointofinterest == null)
            {
                return NotFound();
            }
            PointOfInterestDto pointOf_map = _mapper.Map<PointOfInterestDto>(pointofinterest);
            return Ok(pointOf_map);
        }
        [HttpPost]
        public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(int cityId, [FromBody] PointOfInterestForCreationDto pointOfInterestForCreationDto)
        {
            if (!await _cityInfoRepository.CityExistAsync(cityId))
            {
                _logger.LogInformation($"city with id {cityId} wasnt found when accessing point of interest");
                return NotFound();
            }

            var finalpointofinterest = _mapper.Map<PointOfInterest>(pointOfInterestForCreationDto);
            await _cityInfoRepository.AddPointOfInterestForCityAsync(cityId, finalpointofinterest);
            await _cityInfoRepository.SaveChangesAsync();

            var CreatedPointOfInterestToReturn = _mapper.Map<PointOfInterestDto>(finalpointofinterest);
        
            return CreatedAtRoute("GetPointOfInterest",
                new
                {
                    cityId = cityId,
                    pointofinterestid = CreatedPointOfInterestToReturn.Id,
                },

                CreatedPointOfInterestToReturn
                );

        }
        [HttpPut("{pointofinterestId}")]
        public async Task<ActionResult> UpdatePointofInterest(int cityId, int pointofinterestId, PointOfInterestForUpdateDto pointOfInterest)
        {
            if (!await _cityInfoRepository.CityExistAsync(cityId))
            {
                _logger.LogInformation($"city with id {cityId} wasnt found when accessing point of interest");
                return NotFound();
            }
            var pointofInterestFromDb = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId ,pointofinterestId);
            if (pointofInterestFromDb == null)
            {
                return NotFound();
            }

            _mapper.Map(pointOfInterest, pointofInterestFromDb);
          await  _cityInfoRepository.SaveChangesAsync();
            return NoContent();

        }
        [HttpPatch("{pointofinterestId}")]
        public async Task<ActionResult> PartiallyUpdatePointOfInterest(int cityId, int pointofinterestId, JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        {
            if (!await _cityInfoRepository.CityExistAsync(cityId))
            {
                _logger.LogInformation($"city with id {cityId} wasnt found when accessing point of interest");
                return NotFound();
            }
            var pointofinterestentity =  await _cityInfoRepository.GetPointOfInterestForCityAsync (cityId ,pointofinterestId);
            if(pointofinterestentity == null)
            {
                return NotFound();
            }
            var pointOfinterestTopach = _mapper.Map<PointOfInterestForUpdateDto>(pointofinterestentity);
            patchDocument.ApplyTo(pointOfinterestTopach, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (!TryValidateModel(patchDocument))
            {
                return BadRequest();
            }
            _mapper.Map(pointOfinterestTopach, pointofinterestentity);
            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();
        }
       [HttpDelete("{pointofinterestId}")]
        public async Task<ActionResult> DeletePointofInterest(int cityId,int  pointofinterestId)
        {
            if (!await _cityInfoRepository.CityExistAsync(cityId))
            {
                _logger.LogInformation($"city with id {cityId} wasnt found when accessing point of interest");
                return NotFound();
            }
            var pointofinterestentity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointofinterestId);
            if (pointofinterestentity == null)
            {
                return NotFound();
            }
            
            _cityInfoRepository.DeletePointofInterest(pointofinterestentity);
           await _cityInfoRepository.SaveChangesAsync();
            return NoContent();

        }
       
    }
}
