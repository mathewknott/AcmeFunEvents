using System.Linq;
using Microsoft.Extensions.Caching.Memory;

namespace AcmeFunEvents.Web.Services
{
    public class LookupCacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ICountryService _countryService;

        public LookupCacheService(IMemoryCache cache, ICountryService countryService)
        {
            _cache = cache;
            _countryService = countryService;
        }

        public void Init()
        {
            var countries = _countryService.CountrySelectList(CountryValueType.TwoLetterCode).ToList();
            _cache.Set(CacheKeys.CountriesSelectList, countries);
        }
    }

    public static class CacheKeys
    {
        public static string CountriesSelectList => "CountriesSelectList";
    }
}