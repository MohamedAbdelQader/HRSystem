using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRSystem.Models
{
    public class OfficialHolidaysConfiguration : IEntityTypeConfiguration<OfficialHolidays>
    {
        public void Configure(EntityTypeBuilder<OfficialHolidays> builder)
        {
            builder.ToTable("OffitialHolidays");
            builder.HasKey(f => f.ID);
            builder.Property(o => o.ID).ValueGeneratedOnAdd().IsRequired();
        }
    }
}
