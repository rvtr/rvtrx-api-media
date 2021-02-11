using System.Threading.Tasks;
using RVTR.Media.Domain.Models;

namespace RVTR.Media.Domain.Interfaces
{
  public interface IUnitOfWork
  {
    IRepository<MediaModel> Media { get; }

    Task<int> CommitAsync();
  }
}
