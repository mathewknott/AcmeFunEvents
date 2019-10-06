using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using AcmeFunEvents.Web.Data;
using AcmeFunEvents.Web.DTO;
using AcmeFunEvents.Web.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AcmeFunEvents.Web.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly ApplicationDbContext _db;

        private readonly ILogger _logger;

        /// <param name="dataContext"></param>
        /// <param name="logger"></param>
        public RegistrationService(
            ApplicationDbContext dataContext,
            ILogger<ActivityService> logger
        )
        {
            _db = dataContext;
            _logger = logger;
        }


        /// <summary>
        /// Search including paging
        /// </summary>
        /// <param name="search">gives the text to search into database</param>
        /// <param name="sort">gives you the column name of the table to sort</param>
        /// <param name="registration">specifies the registration i.e, ASC or DESC</param>
        /// <param name="total">Total number of records</param>
        /// <param name="limit">specifies number of rows</param>
        /// <param name="offset">starting position / offset in table</param>
        /// <returns>List</returns>
        public Task<IEnumerable<Registration>> GetRegistrationsAsync(string sort, out int total, string registration, int limit = 200, int offset = 0, string search ="")
        {
            var q = from p in _db.Registration.AsNoTracking()
                    .Include(s => s.Activity)
                    .Include(s => s.User)
                    select new
                {
                    p.Id,
                    p.RegistrationNumber,
                    p.User,
                    p.Comments,
                    p.Activity,
                    p.ActivityId,
                    p.UserId
                    };

            if (!string.IsNullOrEmpty(sort))
            {
                q = q.AsQueryable().OrderBy(sort + " " + (string.IsNullOrEmpty(registration) ? "asc" : registration));
            }

            total = q.Count();

            return Task.FromResult(q.Skip(offset).Take(limit <= 0 ? 200 : limit).ToList()
                .Select(x => new Registration { Id = x.Id, RegistrationNumber = x.RegistrationNumber, User = x.User, Comments = x.Comments, Activity = x.Activity }));
        }

        /// <summary>
        /// Search including paging
        /// </summary>
        /// <param name="total">Total number of records</param>
        /// <param name="limit">specifies number of rows</param>
        /// <param name="offset">starting position / offset in table</param>
        /// <returns>List</returns>
        public Task<IEnumerable<Registration>> GetRegistrationsAsync(out int total, int limit = 200, int offset = 0)
        {
            var q = from p in _db.Registration.AsNoTracking()
                    .Include(s => s.Activity)
                    .Include(s => s.User)
                    select new
                {
                    p.Id,
                    p.RegistrationNumber,
                    p.User,
                    p.Comments,
                    p.Activity,
                    p.ActivityId,
                    p.UserId
                    };

            total = q.Count();
            return Task.FromResult(q.Skip(offset).Take(limit <= 0 ? 200 : limit).ToList()
                .Select(x => new Registration { Id = x.Id, RegistrationNumber = x.RegistrationNumber, User = x.User, Comments = x.Comments, Activity = x.Activity}));
        }

        /// <summary>
        /// Search including paging
        /// </summary>
        /// <param name="total">Total number of records</param>
        /// <param name="activityId"></param>
        /// <param name="limit">specifies number of rows</param>
        /// <param name="offset">starting position / offset in table</param>
        /// <returns>List</returns>
        public Task<IEnumerable<Registration>> GetRegistrationsByActivityIdAsync(out int total, Guid activityId, int limit = 200, int offset = 0)
        {
            var q = from p in _db.Registration.AsNoTracking()
                    .Include(s => s.Activity)
                    .Include(s => s.User)
                    .Where(x=>x.ActivityId.Equals(activityId))
                select new
                {
                    p.Id,
                    p.RegistrationNumber,
                    p.User,
                    p.Comments,
                    p.Activity,
                    p.ActivityId,
                    p.UserId
                };

            total = q.Count();
            return Task.FromResult(q.Skip(offset).Take(limit <= 0 ? 200 : limit).ToList()
                .Select(x => new Registration { Id = x.Id, RegistrationNumber = x.RegistrationNumber, User = x.User, Comments = x.Comments, Activity = x.Activity }));
        }

        /// <returns></returns>
        public void AddRegistration(Registration registration, out int result)
        {
            try
            {
                _db.Registration.Add(registration);
                _db.Entry(registration.User).State = EntityState.Unchanged;
                _db.Entry(registration.Activity).State = EntityState.Unchanged;
                result = _db.SaveChanges();
                _logger.Log(LogLevel.Information, new EventId(2), "", null, (s, exception) => "Registration Created");
            }
            catch (Exception dbEx)
            {
                result = 0;
                _logger.LogError(new EventId(2), dbEx, "An error occured saving activity");
            }
        }

        /// <returns></returns>
        public void EditRegistration(Registration registration, out int result)
        {
            try
            {
                _db.Registration.Update(registration);
                result = _db.SaveChanges();
                _logger.Log(LogLevel.Information, new EventId(2), "", null, (s, exception) => "Registration Created");
            }
            catch (Exception dbEx)
            {
                result = 0;
                _logger.LogError(new EventId(2), dbEx, "An error occured saving activity");
            }
        }

    }
}