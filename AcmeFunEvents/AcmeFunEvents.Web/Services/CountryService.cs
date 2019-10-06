using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AcmeFunEvents.Web.Services
{
    public class CountryService : ICountryService
    {
        public IEnumerable<SelectListItem> CountrySelectList(CountryValueType type)
        {
            var getCultureInfo = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            var items =  getCultureInfo.Select(c => new RegionInfo(c.LCID)).Select(getRegionInfo => new SelectListItem
            {
                Value = type == CountryValueType.FullName ? getRegionInfo.EnglishName : getRegionInfo.TwoLetterISORegionName,
                Text = getRegionInfo.EnglishName,
                Selected = type == CountryValueType.FullName ? getRegionInfo.EnglishName.Equals("Canada", StringComparison.CurrentCultureIgnoreCase) : getRegionInfo.TwoLetterISORegionName.Equals("CA", StringComparison.CurrentCultureIgnoreCase)
            });

            return items.Distinct(new SelectListItemComparer()).OrderByDescending(x => x.Selected).ThenBy(x => x.Text == "Canada").ThenByDescending(x => x.Text == "United States").ThenBy(x => x.Text).ToList();
        }

        private class SelectListItemComparer : IEqualityComparer<SelectListItem>
        {
            public bool Equals(SelectListItem x, SelectListItem y)
            {
                return x.Text == y.Text && x.Value == y.Value;
            }

            public int GetHashCode(SelectListItem item)
            {
                var hashText = item.Text?.GetHashCode() ?? 0;
                var hashValue = item.Value?.GetHashCode() ?? 0;
                return hashText ^ hashValue;
            }
        }
    }

    public enum CountryValueType
    {
        TwoLetterCode,
        FullName
    }
}