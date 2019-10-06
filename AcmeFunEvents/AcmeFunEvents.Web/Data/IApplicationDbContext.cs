using AcmeFunEvents.Web.DTO;
using Microsoft.EntityFrameworkCore;

namespace AcmeFunEvents.Web.Data
{
    public interface IApplicationDbContext
    {
        DbSet<Activity> Activity { get; set; }
        DbSet<Registration> Registration { get; set; }
        DbSet<User> User { get; set; }

        int SaveChanges();
    }
}