using System.Collections.Generic;
using AcmeFunEvents.Web.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AcmeFunEvents.Web.Services
{
    public interface ICountryService
    {
        IEnumerable<SelectListItem> CountrySelectList(CountryValueType type);
    }
}