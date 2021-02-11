using System;
using Microsoft.EntityFrameworkCore;
using RVTR.Media.Domain.Models;

namespace RVTR.Media.Context
{
  /// <summary>
  /// Represents the _Media_ context
  /// </summary>
  public class MediaContext : DbContext
  {
    public DbSet<MediaModel> Medias { get; set; }

    public MediaContext(DbContextOptions<MediaContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<MediaModel>().HasKey(e => e.Id);
    }
  }
}
