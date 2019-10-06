using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AcmeFunEvents.Web.DTO;

namespace AcmeFunEvents.Web.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRegistrationService
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
        Task<IEnumerable<Registration>> GetRegistrationsAsync(string sort, out int total, string order, int limit = 200, int offset = 0, string search = "");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="total"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        Task<IEnumerable<Registration>> GetRegistrationsAsync(out int total, int limit = 200, int offset = 0);

        Task<IEnumerable<Registration>> GetRegistrationsByActivityIdAsync(out int total, Guid activityId, int limit = 200, int offset = 0);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registration"></param>
        /// <param name="result"></param>
        void AddRegistration(Registration registration, out int result);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registration"></param>
        /// <param name="result"></param>
        void EditRegistration(Registration registration, out int result);

    }
}