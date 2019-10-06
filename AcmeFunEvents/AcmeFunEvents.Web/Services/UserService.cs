using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using AcmeFunEvents.Web.Data;
using AcmeFunEvents.Web.DTO;
using AcmeFunEvents.Web.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AcmeFunEvents.Web.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _db;

        public UserService(ApplicationDbContext dataContext)
        {
            _db = dataContext;
        }
        
        /// <summary>
        /// User Search including paging
        /// </summary>
        /// <param name="search">gives the text to search into database</param>
        /// <param name="sort">gives you the column name of the table to sort</param>
        /// <param name="order">specifies the order i.e, ASC or DESC</param>
        /// <param name="total">Total number of records</param>
        /// <param name="limit">specifies number of rows</param>
        /// <param name="offset">starting position / offset in table</param>
        /// <returns>List of Users</returns>
        public Task<IEnumerable<User>> GetUsersAsync(string sort, out int total, string order, int limit = 200, int offset = 0, string search ="")
        {
            var q = from p in _db.User.AsNoTracking()
                    select new
                {
                    p.Id,
                    p.FirstName,
                    p.LastName,
                    p.PhoneNumber,
                    p.EmailAddress
                    };
            
            if (!string.IsNullOrEmpty(search) && search.Length > 2)
            {
                q = q.Where(x => x.FirstName.Contains(search, StringComparison.CurrentCultureIgnoreCase) || x.LastName.Contains(search, StringComparison.CurrentCultureIgnoreCase));
            }
            
            if (!string.IsNullOrEmpty(sort))
            {
                q = q.AsQueryable().OrderBy(sort + " " + (string.IsNullOrEmpty(order) ? "asc" : order));
            }

            total = q.Count();

            return Task.FromResult(q.Skip(offset).Take(limit <= 0 ? 200 : limit).ToList()
                .Select(x => new User { Id = x.Id, FirstName = x.FirstName, LastName = x.LastName, PhoneNumber = x.PhoneNumber, EmailAddress = x.EmailAddress }));
        }

        /// <summary>
        /// User Search including paging
        /// </summary>
        /// <param name="total">Total number of records</param>
        /// <param name="limit">specifies number of rows</param>
        /// <param name="offset">starting position / offset in table</param>
        /// <returns>List of Users</returns>
        public Task<IEnumerable<User>> GetUsersAsync(out int total, int limit = 200, int offset = 0)
        {
            var q = from p in _db.User.AsNoTracking()
                    select new
                {
                    p.Id,
                    p.FirstName,
                    p.LastName,
                    p.PhoneNumber,
                    p.EmailAddress
                    };

            total = q.Count();
            return Task.FromResult(q.Skip(offset).Take(limit <= 0 ? 200 : limit).ToList()
                .Select(x => new User { Id = x.Id, FirstName = x.FirstName, LastName = x.LastName, PhoneNumber = x.PhoneNumber, EmailAddress = x.EmailAddress }));
        }

     
    }
}