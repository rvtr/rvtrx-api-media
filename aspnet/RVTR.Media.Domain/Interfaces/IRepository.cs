using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using RVTR.Media.Domain.Abstracts;

namespace RVTR.Media.Domain.Interfaces
{
  /// <summary>
  ///
  /// </summary>
  /// <typeparam name="TEntity"></typeparam>
  public interface IRepository<TEntity> where TEntity : AEntity
  {
    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task DeleteAsync(string id);

    /// <summary>
    ///
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    Task InsertAsync(TEntity entry);

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<TEntity>> SelectAsync();

    /// <summary>
    ///
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    Task<IEnumerable<TEntity>> SelectAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    ///
    /// </summary>
    /// <param name="entry"></param>
    void Update(TEntity entry);
  }
}
