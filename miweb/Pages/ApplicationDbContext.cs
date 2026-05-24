using Microsoft.EntityFrameworkCore;

namespace miweb.Pages
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Alumno> Alumnos { get; set; } = null!;
    }
}