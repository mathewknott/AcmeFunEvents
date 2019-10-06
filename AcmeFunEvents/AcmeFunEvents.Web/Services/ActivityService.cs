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
    public class ActivityService : IActivityService
    {
        private readonly ApplicationDbContext _db;

        private readonly ILogger _logger;

        public ActivityService(
            ApplicationDbContext dataContext,
            ILogger<ActivityService> logger
        )
        {
            _db = dataContext;
            _logger = logger;
        }

        /// <summary>
        /// Activity Search including paging
        /// </summary>
        /// <param name="search">gives the text to search into database</param>
        /// <param name="sort">gives you the column name of the table to sort</param>
        /// <param name="order">specifies the order i.e, ASC or DESC</param>
        /// <param name="total">Total number of records</param>
        /// <param name="limit">specifies number of rows</param>
        /// <param name="offset">starting position / offset in table</param>
        /// <returns>List of Activitys</returns>
        public Task<IEnumerable<Activity>> GetActivitiesAsync(string sort, out int total, string order, int limit = 200, int offset = 0, string search ="")
        {
            var q = from p in _db.Activity.AsNoTracking()
                    select new
                {
                    p.Id,
                    p.Code,
                    p.Name,
                    p.Date
                    };
            
            if (!string.IsNullOrEmpty(search) && search.Length > 2)
            {
                q = q.Where(x => x.Name.Contains(search, StringComparison.CurrentCultureIgnoreCase));
            }
            
            if (!string.IsNullOrEmpty(sort))
            {
                q = q.AsQueryable().OrderBy(sort + " " + (string.IsNullOrEmpty(order) ? "asc" : order));
            }

            total = q.Count();

            return Task.FromResult(q.Skip(offset).Take(limit <= 0 ? 200 : limit).ToList()
                .Select(x => new Activity { Id = x.Id, Name = x.Name, Code = x.Code, Date = x.Date}));
        }

        /// <summary>
        /// Activity Search including paging
        /// </summary>
        /// <param name="total">Total number of records</param>
        /// <param name="limit">specifies number of rows</param>
        /// <param name="offset">starting position / offset in table</param>
        /// <returns>List of Activitys</returns>
        public Task<IEnumerable<Activity>> GetActivitiesAsync(out int total, int limit = 200, int offset = 0)
        {
            var q = from p in _db.Activity.AsNoTracking()
                    select new
                {
                    p.Id,
                    p.Name,
                    p.Code,
                    p.Date
                };

            total = q.Count();
            return Task.FromResult(q.Skip(offset).Take(limit <= 0 ? 200 : limit).ToList()
                .Select(x => new Activity { Id = x.Id, Name = x.Name, Date = x.Date, Code = x.Code }));
        }

        /// <returns></returns>
        public void AddActivity(Activity activity, out int result)
        {
            try
            {
                _db.Activity.Add(activity);
                result = _db.SaveChanges();
                _logger.Log(LogLevel.Information, new EventId(2), "", null, (s, exception) => "Activity Created");
            }
            catch (Exception dbEx)
            {
                result = 0;
                _logger.LogError(new EventId(2), dbEx, "An error occured saving activity");
            }
        }

        /// <returns></returns>
        public void EditActivity(Activity activity, out int result)
        {
            try
            {
                _db.Activity.Update(activity);
                result = _db.SaveChanges();
                _logger.Log(LogLevel.Information, new EventId(2), "", null, (s, exception) => "Activity Created");
            }
            catch (Exception dbEx)
            {
                result = 0;
                _logger.LogError(new EventId(2), dbEx, "An error occured saving activity");
            }
        }
    }
}