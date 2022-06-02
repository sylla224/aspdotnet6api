using AutoMapper;
using CityInfo.API.Entities;
using CityInfo2.API.Models;

namespace CityInfo2.API.Profiles
{
    public class PointOfInterestProfile:Profile
    {
        public PointOfInterestProfile()
        {
            CreateMap<PointOfInterest, PointOfInterestDto>().ReverseMap();
            CreateMap<PointOfInterestForCreationDto, PointOfInterest>();
            CreateMap<PointOfInterestForUpdateDto, PointOfInterest>().ReverseMap();
            
        }
    }
}
