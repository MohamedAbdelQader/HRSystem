using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;



namespace HRSystem.Models
{
    public class HRContext : IdentityDbContext<User>
    {
        public HRContext(DbContextOptions options):base(options) { }
        
        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<OfficialHolidays> OfficialHolidays { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Payment> Payments { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            new UserConfiguration().Configure(modelBuilder.Entity<User>());
            new EmployeeConfiguration().Configure(modelBuilder.Entity<Employee>());
            new AttendanceConfiguration().Configure(modelBuilder.Entity<Attendance>());
            new OfficialHolidaysConfiguration().Configure(modelBuilder.Entity<OfficialHolidays>());
            new GroupConfiguration().Configure(modelBuilder.Entity<Group>());
            new PaymentConfiguration().Configure(modelBuilder.Entity<Payment>());
            base.OnModelCreating(modelBuilder);
        }


       protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }














    }
}
