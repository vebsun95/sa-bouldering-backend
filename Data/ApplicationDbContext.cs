using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using backend.Models;
using Microsoft.AspNetCore.Identity;

namespace backend.Data;

public class ApplicationDbContext : IdentityDbContext<User, IdentityRole, string> 
{
    #nullable disable
    public virtual DbSet<Boulder> Boulders {get; set;}
    public virtual DbSet<Ascent> Ascents {get; set;}
    public virtual DbSet<Wall> Walls {get; set;}
    public virtual DbSet<ClimbingHold> ClimbingHolds {get; set;}
    public virtual DbSet<ClimbingHoldBoulder> ClimbingHoldBoulders {get; set;}
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

        // Primary Keys
        builder.Entity<Wall>().HasKey(w => w.Id);
        builder.Entity<Ascent>().HasKey(a => a.Id);
        builder.Entity<Boulder>().HasKey(b => b.Id);
        builder.Entity<ClimbingHold>().HasKey(ch => ch.Id);
        builder.Entity<ClimbingHoldBoulder>().HasKey(chb => new {chb.BoulderId, chb.ClimbingHoldId});

        // Relationships
    }
}
