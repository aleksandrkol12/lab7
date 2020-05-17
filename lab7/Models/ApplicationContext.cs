using Microsoft.EntityFrameworkCore;

namespace lab7
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> context)
            : base(context)
        {
            Database.EnsureCreated();
        }

        public DbSet<Group> Groups { get; set; }

        public DbSet<Student> Students { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer("Server=DESKTOP-DRLUE30\\SQLEXPRESS;Database=laba7SITDB;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Student>()
                .Property(p => p.FirstName)
                .HasMaxLength(30);

            builder.Entity<Student>()
                .Property(p => p.LastName)
                .HasMaxLength(30);

            builder.Entity<Student>()
                .Property(p => p.CreatedAt);

            builder.Entity<Student>()
                .Property(p => p.UpdatedAt);

            builder.Entity<Group>()
                .Property(p => p.Name)
                .HasMaxLength(10);

            builder.Entity<Student>()
                .Property(p => p.UpdatedAt)
                .HasDefaultValue(null);

            builder.Entity<Group>()
                .HasMany(p => p.Students)
                .WithOne(p => p.Group)
                .HasForeignKey(p => p.GroupId)
                .HasConstraintName("FK_Group_Students")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
