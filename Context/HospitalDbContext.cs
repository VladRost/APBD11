using APBD10.Models;
using Microsoft.EntityFrameworkCore;

namespace APBD10.Context;

public class HospitalDbContext:DbContext
{
    public HospitalDbContext()
    {
        
    }
    public HospitalDbContext(DbContextOptions options):base(options)
    {
        
    }
    public DbSet<Doctor> Doctors { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlServer("Server=localhost;Database=master;User Id=sa;Password=SDFis2394Sfns;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Doctor>(opt =>
        {
            opt.HasKey(e => e.IdDoctor);
            opt.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            opt.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            opt.Property(e => e.Email).HasMaxLength(100).IsRequired();
        });
        
        modelBuilder.Entity<Patient>(opt =>
        {
            opt.HasKey(e => e.IdPatient);
            opt.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            opt.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            opt.Property(e => e.Birthdate).HasMaxLength(100).IsRequired();
        });
    }
}