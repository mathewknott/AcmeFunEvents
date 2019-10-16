using System.Collections.Generic;
using System.Threading.Tasks;

namespace AcmeFunEvents.Web.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface INumberService
    {
        Task<List<int[]>> GetResultAsync(int[] pattern, int input);
    }
}