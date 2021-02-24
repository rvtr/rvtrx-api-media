using System.Threading.Tasks;
using RVTR.Media.Domain.Interfaces;
using RVTR.Media.Domain.Models;

namespace RVTR.Media.Context.Repositories
{
  /// <summary>
  /// Represents the _UnitOfWork_ repository
  /// </summary>
  public class UnitOfWork : IUnitOfWork
  {
    private readonly MediaContext _context;

    public IRepository<MediaModel> Media { get; }

    /// <summary>
    ///
    /// </summary>
    /// <param name="context"></param>
    public UnitOfWork(MediaContext context)
    {
      _context = context;
      Media = new Repository<MediaModel>(context);
    }

    /// <summary>
    /// Represents the _UnitOfWork_ `Commit` method
    /// </summary>
    /// <returns></returns>
    public async Task<int> CommitAsync() => await _context.SaveChangesAsync();
  }
}
