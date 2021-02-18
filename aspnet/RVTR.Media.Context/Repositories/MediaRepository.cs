using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RVTR.Media.Domain.Interfaces;
using RVTR.Media.Domain.Models;

namespace RVTR.Media.Context.Repositories
{
  /// <summary>
  /// Represents the _Repository_ generic
  /// </summary>
  /// <typeparam name="TEntity"></typeparam>
  public class MediaRepository : Repository<MediaModel>
  {
    public MediaRepository(MediaContext context) : base(context) { }

    public override async Task<MediaModel> SelectAsync(int id) => await Db
      .Where(x => x.MediaId == id)
      .FirstOrDefaultAsync();

    public override async Task<IEnumerable<MediaModel>> SelectAsync() =>
      await Db.ToListAsync();
  }
}
