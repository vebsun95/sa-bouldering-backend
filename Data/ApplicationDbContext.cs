using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using backend.Models;
using Microsoft.AspNetCore.Identity;

namespace backend.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string> 
{
    #nullable disable
    public virtual DbSet<Boulder> Boulders {get; set;}
    public virtual DbSet<Ascent> Ascents {get; set;}
    public virtual DbSet<Wall> Walls {get; set;}
    public virtual DbSet<EllipseHold> EllipseHolds {get; set;}
    public virtual DbSet<PolygonHold> PolygonHolds {get; set;}
    public virtual DbSet<Point> Points {get; set;}
    public virtual DbSet<Neighbour> Neighbours {get; set;}
    public virtual DbSet<HoldIndex> HoldIndices {get; set;}
    #nullable restore
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
       
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
        builder.Entity<Boulder>().HasKey(a => a.Id);
        builder.Entity<Ascent>().HasKey(a => a.Id);
        builder.Entity<Wall>().HasKey(a => a.Id);
        builder.Entity<EllipseHold>().HasKey(a => a.Id);
        builder.Entity<PolygonHold>().HasKey(a => a.Id);
        builder.Entity<Point>().HasKey(a => a.Id);
        builder.Entity<Neighbour>().HasKey(a => a.Id);
        builder.Entity<HoldIndex>().HasKey(a => a.Id);


    }
}
