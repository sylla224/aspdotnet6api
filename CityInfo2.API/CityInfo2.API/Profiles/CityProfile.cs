using AutoMapper;
using CityInfo.API.Entities;
using CityInfo2.API.Models;

namespace CityInfo2.API.Profiles
{
    public class CityProfile:Profile
    {
        public CityProfile()
        {
            CreateMap<City, CityWithoutPointsOfInterestDto>();
            CreateMap<City, CityDto>();
           
        }
    }
}
