using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using UntisNotifier.Abstractions.Models;

namespace UntisNotifier.Data.Configuration
{
    public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
    {
        public void Configure(EntityTypeBuilder<Lesson> builder)
        {
            builder
                .ToTable("Lessons")
                .HasKey(k => k.ID);

            /*builder
                .Property(p => p.ID)
                    .ValueGeneratedOnAdd();*/
        }
    }
}
