using Microsoft.EntityFrameworkCore;

namespace ECOMMERCE.SUBMISSION.DATA;

public class EcommerceDataContext : DbContext
{
    private readonly string dbPath;

    public EcommerceDataContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Path.Combine(Environment.GetFolderPath(folder), "db");
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        dbPath = System.IO.Path.Join(path, "ecommerce.db");
    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={dbPath}");

    public DbSet<Account> Accounts { get; set; }
    public DbSet<Specification> Specifications { get; set; }
    public DbSet<Order> Orders { get; set; }
}
