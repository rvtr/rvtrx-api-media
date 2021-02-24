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

    /// <summary>
    ///
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public MediaContext(DbContextOptions<MediaContext> options) : base(options) { }

    /// <summary>
    ///
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<MediaModel>().HasKey(e => e.EntityId);
    }
  }
}
