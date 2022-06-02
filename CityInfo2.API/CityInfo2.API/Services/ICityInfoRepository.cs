using CityInfo.API.Entities;

namespace CityInfo2.API.Services
{
    public interface ICityInfoRepository
    {
        Task<IEnumerable<City>> GetCitiesAsync();
        Task<(IEnumerable<City>,PaginationMetadata)> GetCitiesAsync(string? name, string? serachQuery, int pageNumber, int pageSize);
        Task<City?> GetCityAsync(int cityId, bool includePointofInterest);
        Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId);
        Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId,
            int pointOfInterestId);
        Task<bool> CityExistAsync(int cityId);
        Task AddPointOfInterestForCityAsync(int CityId, PointOfInterest pointOfInterest);
        Task UpdatePointofInterest(int cityId, int pointOfInterestId);
        void DeletePointofInterest(PointOfInterest pointOfInterest);
        Task<bool> SaveChangesAsync();


    }
}
