using CityInfo.API.Entities;
using CityInfo2.API.Dbcontexts;
using Microsoft.EntityFrameworkCore;

namespace CityInfo2.API.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly CityInfoContext _context;

        public CityInfoRepository(CityInfoContext context)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
        }
        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await _context.Cities.OrderBy(c=>c.Name).ToListAsync();
        }
        public async Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(string? name,string? serachQuery, int pageNumber, int pageSize)
        {
            //if (string.IsNullOrEmpty(name) && string.IsNullOrWhiteSpace(serachQuery))
            //{
            //    return await GetCitiesAsync();
            //}
            //Collection to start
            var collection = _context.Cities as IQueryable<City>;
            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.Trim();
                collection=collection.Where(c => c.Name == name);
            }
            if (!string.IsNullOrWhiteSpace(serachQuery))
            {
                serachQuery = serachQuery.Trim();
                collection = collection.Where(a => a.Name.Contains(serachQuery)
                  || ( a.Description !=null && a.Description.Contains(serachQuery))
                ) ;
            }
            var totalItemCount = await collection.CountAsync();
            var paginationmetadeta = new PaginationMetadata(totalItemCount, pageSize, pageNumber);
            var colectionToreturn= await collection.OrderBy(c => c.Name)
                .Skip(pageSize*(pageNumber-1))
                .Take(pageSize)
                .ToListAsync();

            return (colectionToreturn, paginationmetadeta);
           
        }

        public async Task<City?> GetCityAsync(int cityId, bool includePointofInterest)
        {
            if (includePointofInterest)
            {
                return await _context.Cities.Include(c=>c.PointsOfInterest).Where(c=>c.Id == cityId).SingleOrDefaultAsync();
            }
            return await _context.Cities.Where(c=>c.Id == cityId).SingleOrDefaultAsync();
        }

        public Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId)
        {
            return _context.PointsOfInterest.Where(p=>p.CityId==cityId && p.Id==pointOfInterestId).SingleOrDefaultAsync();
        }

        public async Task<bool> CityExistAsync(int cityId)
        {
            return await _context.Cities.AnyAsync(c => c.Id == cityId);
        }
        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId)
        {
            return await _context.PointsOfInterest.Where(p=>p.CityId==cityId).ToListAsync();
        }

        public async Task AddPointOfInterestForCityAsync(int CityId, PointOfInterest pointOfInterest)
        {
           var city = await GetCityAsync(CityId, false);
            if(city != null)
            {
                city.PointsOfInterest.Add(pointOfInterest);
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
          return   ( await _context.SaveChangesAsync() >=0 );
        }

        public async Task UpdatePointofInterest(int cityId, int pointOfInterestId)
        {
           
        }

        public void DeletePointofInterest(PointOfInterest pointOfInterest)
        {
            _context.PointsOfInterest.Remove(pointOfInterest);
        }
    }
}
