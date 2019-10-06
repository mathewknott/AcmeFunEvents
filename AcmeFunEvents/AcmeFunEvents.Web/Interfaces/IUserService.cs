using System.Collections.Generic;
using System.Threading.Tasks;
using AcmeFunEvents.Web.DTO;

namespace AcmeFunEvents.Web.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <param name="sort"></param>
        /// <param name="order"></param>
        /// <param name="total"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        Task<IEnumerable<User>> GetUsersAsync(string sort, out int total, string order, int limit = 200, int offset = 0, string search = "");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="total"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        Task<IEnumerable<User>> GetUsersAsync(out int total, int limit = 200, int offset = 0);

    }
}