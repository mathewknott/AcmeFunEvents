using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AcmeFunEvents.Web.Interfaces;

namespace AcmeFunEvents.Web.Services
{
    public class NumberService : INumberService
    {
        /// <summary>
        /// Search including paging
        /// </summary>
        /// <returns>List</returns>
        public Task<List<int[]>> GetResultAsync(int[] pattern, int input)
        {
            var stringList = new List<string>();
            
            foreach (var t1 in pattern)
            {
                for (var j = 0; j < pattern.Where(x => x != t1).ToArray().Length; j++)
                {
                    if (!(t1 + pattern[j]).Equals(input)) continue;

                    stringList.Add(t1 > pattern[j]
                        ? string.Join(",", new[] {t1, pattern[j]})
                        : string.Join(",", new[] { pattern[j], t1}));

                    break;
                }
            }

            return Task.FromResult(stringList.Select(d => Array.ConvertAll(d.Split(','), int.Parse)).ToList());
        }
    }
}