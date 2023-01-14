using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backenend.Models;

public class BackendContext : DbContext
{
    public BackendContext(DbContextOptions<BackendContext> options)
        : base(options)
    {
    }

    public DbSet<Car> Cars { get; set; } = null!;
}