using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRSystem.Models
{
    public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
    {
        public void Configure(EntityTypeBuilder<Attendance> builder)
        {
            builder.ToTable("Attendance");
            builder.HasKey(a => a.ID);
            builder.Property(a => a.ID).IsRequired().ValueGeneratedOnAdd();
            builder.Property(a => a.EmployeeID).IsRequired();
            builder.Property(a => a.AttendanceDate).IsRequired();
            
        }
    }
}
