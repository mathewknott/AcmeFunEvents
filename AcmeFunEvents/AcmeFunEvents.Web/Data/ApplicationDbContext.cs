using AcmeFunEvents.Web.DTO;
using AcmeFunEvents.Web.Extensions;
using Microsoft.EntityFrameworkCore;

namespace AcmeFunEvents.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Activity>()
                .HasIndex(u => u.Code)
                .IsUnique();

            builder.Entity<Registration>()
                .HasIndex(u => u.RegistrationNumber)
                .IsUnique();


            builder.Entity<User>()
                .HasIndex(u => u.EmailAddress)
                .IsUnique();

            foreach (var entity in builder.Model.GetEntityTypes())
            {
                // Replace table names
                entity.Relational().TableName = entity.Relational().TableName.ToSnakeCase();

                // Replace column names            
                foreach (var property in entity.GetProperties())
                {
                    property.Relational().ColumnName = property.Name.ToSnakeCase();
                }

                foreach (var key in entity.GetKeys())
                {
                    key.Relational().Name = key.Relational().Name.ToSnakeCase();
                }

                foreach (var key in entity.GetForeignKeys())
                {
                    key.Relational().Name = key.Relational().Name.ToSnakeCase();
                }

                foreach (var index in entity.GetIndexes())
                {
                    index.Relational().Name = index.Relational().Name.ToSnakeCase();
                }
            }
        }

        public DbSet<Activity> Activity { get; set; }
        public DbSet<Registration> Registration { get; set; }
        public DbSet<User> User { get; set; }

    }
}
