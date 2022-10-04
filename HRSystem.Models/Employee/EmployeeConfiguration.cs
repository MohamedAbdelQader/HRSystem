using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRSystem.Models
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employee");
            builder.HasKey(e => e.UserId);
            builder.Property(e => e.Additions).HasDefaultValue(0);
            builder.Property(e => e.Discounts).HasDefaultValue(0);
            builder.Property(e => e.ReceivingMoneyTime).HasDefaultValue(DateTime.Now.AddDays(30));
            builder.Property(e => e.GroupID).HasDefaultValue(0);
        }
    }
}
