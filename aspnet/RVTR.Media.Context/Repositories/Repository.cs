using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RVTR.Media.Domain.Abstracts;
using RVTR.Media.Domain.Interfaces;

namespace RVTR.Media.Context.Repositories
{
  /// <summary>
  /// Represents the _Repository_ generic
  /// </summary>
  /// <typeparam name="TEntity"></typeparam>
  public class Repository<TEntity> : IRepository<TEntity> where TEntity : AEntity
  {
    private readonly DbSet<TEntity> _dbSet;

    /// <summary>
    ///
    /// </summary>
    /// <param name="context"></param>
    public Repository(MediaContext context)
    {
      _dbSet = context.Set<TEntity>();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public virtual async Task DeleteAsync(string id) => _dbSet.Remove((await SelectAsync(e => e.id == id)).FirstOrDefault());

    /// <summary>
    ///
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    public virtual async Task InsertAsync(TEntity entry) => await _dbSet.AddAsync(entry).ConfigureAwait(true);

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public virtual async Task<IEnumerable<TEntity>> SelectAsync() => await _dbSet.ToListAsync();

    /// <summary>
    ///
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual async Task<IEnumerable<TEntity>> SelectAsync(Expression<Func<TEntity, bool>> predicate)
    {
      var entities = await _dbSet.Where(predicate).ToListAsync().ConfigureAwait(true);

      foreach (var entity in entities)
      {
        foreach (var navigation in _dbSet.Attach(entity).Navigations)
        {
          await navigation.LoadAsync();
        }
      }

      return entities;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="entry"></param>
    public virtual void Update(TEntity entry) => _dbSet.Update(entry);
  }
}
