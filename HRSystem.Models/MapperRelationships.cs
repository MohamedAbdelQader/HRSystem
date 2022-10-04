using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HRSystem.Models
{
    public class MapperRelationships
    {
        public static void MapeRelationships(ModelBuilder builder)
        {
            

            
            builder.Entity<Employee>()
                .HasOne(v=>v.User)
                .WithOne(v => v.Employee)
                .HasForeignKey<Employee>(v=>v.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Employee>()
                .HasMany(e => e.Attendances)
                .WithOne(e => e.Employee)
                .HasForeignKey(e => e.EmployeeID)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Employee>()
                .HasOne(g => g.Group)
                .WithMany(g => g.Employees)
                .HasForeignKey(g => g.GroupID)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Employee>()
                .HasMany(p => p.Payments)
                .WithOne(p => p.Employee)
                .HasForeignKey(p => p.EmployeeID)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
