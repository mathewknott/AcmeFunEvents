using System.Threading.Tasks;
using AcmeFunEvents.Web.Models;

namespace AcmeFunEvents.Web.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IHeaderService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<HeaderModel> GetGtmAsync();
    }
}