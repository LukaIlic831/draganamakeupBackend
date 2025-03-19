using DraganaMakeup.Models;
using Microsoft.EntityFrameworkCore;

namespace DraganaMakeup.Context
{
    public class ApplicationDbContext : DbContext
    {
        public virtual  DbSet<User> Users { get; set; } = null!;
        public virtual  DbSet<Appointment> Appointments { get; set; } = null!;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }
    }
}