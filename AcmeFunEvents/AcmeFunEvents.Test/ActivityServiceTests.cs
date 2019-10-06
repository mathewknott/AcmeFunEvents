using System.Collections.Generic;
using System.Threading.Tasks;
using AcmeFunEvents.Web.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AcmeFunEvents.Test
{
    public class ActivityServiceTests : ServicesBase
    {
        private IActivityService Service => ServiceProvider.GetRequiredService<IActivityService>();

        [Theory]
        [MemberData(nameof(ActivitySearchTestData))]
        public async Task GetActivity_CanGetSearchResponse(string search, string sort, string order, int limit = 200, int offset = 0)
        {
            var response = await Service.GetActivitiesAsync(sort, out _, order, limit, offset, search);
            Assert.NotNull(response);
        }
        
        [Theory]
        [MemberData(nameof(ActivityTestData))]
        public async Task GetActivity_CanGetResponse(int limit, int offset = 0)
        {
            var response = await Service.GetActivitiesAsync(out _, limit, offset);
            Assert.NotNull(response);
        }
        
        [Fact]
        public async Task GetActivity_CanGetResponse_Alt()
        {
            var limit = 1;
            var offset = 0;
            
            var response = await Service.GetActivitiesAsync(out _, limit, offset);
            Assert.NotNull(response);
        }
        
        [Theory]
        [MemberData(nameof(ActivitySearchTestData))]
        public async Task GetActivity_CanGetResponse_Search(string search, string sort, string order, int limit = 200, int offset = 0)
        {
            var response = await Service.GetActivitiesAsync(sort, out _, order, limit, offset, search);
            Assert.NotNull(response);
        }
        
        /// <summary>
        /// int limit, int offset = 0
        /// </summary>
        public static IEnumerable<object[]> ActivityTestData =>
            new List<object[]>
            {
                new object[] { 1, 0 },
                new object[] { 1, 10 }
            };

        /// <summary>
        /// string search, string sort, string order, int limit = 200, int offset = 0
        /// </summary>
        public static IEnumerable<object[]> ActivitySearchTestData =>
            new List<object[]>
            {
                new object[] { "", "", "asc", 1, 0 },
                new object[] { "", "", "desc", 1, 0 },
                new object[] { "", "", "xxx", 1, 0 },
                new object[] { "", "", "asc", 1, 0 }
            };
    }
}