using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using UntisNotifier.Abstractions.Models;

namespace UntisNotifier.Data
{
    internal class DatabaseContext : DbContext
    {
        private static bool _isInitialized;
        public DatabaseContext() : base()
        {
            if (!_isInitialized)
            {
                _isInitialized = Database.EnsureCreated();
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=database.sqlite");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new Configuration.LessonConfiguration());
        }

        public DbSet<Lesson> Lessons { get; set; }
    }
}
