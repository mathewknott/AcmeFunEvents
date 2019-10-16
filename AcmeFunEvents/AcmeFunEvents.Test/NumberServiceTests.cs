using System.Collections.Generic;
using System.Threading.Tasks;
using AcmeFunEvents.Web.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AcmeFunEvents.Test
{
    public class NumberServiceTests : ServicesBase
    {
        private INumberService Service => ServiceProvider.GetRequiredService<INumberService>();

        [Theory]
        [MemberData(nameof(NumberTestData))]
        public async Task GetResult_CanGetResponse(int[] pattern, int input)
        {
            var response = await Service.GetResultAsync(pattern, input);
            Assert.NotNull(response);
        }

        [Theory]
        [InlineData(new[] { 1, 3, 9, 6, 4 }, 10)]
        public async Task GetResult_TestCount(int[] pattern, int input)
        {
            var response = await Service.GetResultAsync(pattern, input);
            Assert.Equal(2, response.Count);
        }

        /// <summary>
        /// int[] pattern, int totalValue, int input
        /// Return an array of integers where any 2 numbers in the array add up to 12
        /// </summary>
        
        public static IEnumerable<object[]> NumberTestData =>
            new List<object[]>
            {
                new object[] { new [] { 1, 3, 9, 6, 4 }, 10},
                new object[] { new [] { 1, 3, 8, 6, 4 }, 12}
            };
    }
}