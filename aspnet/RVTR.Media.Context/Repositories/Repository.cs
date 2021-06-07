using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RVTR.Media.Domain.Abstracts;
using RVTR.Media.Domain.Interfaces;
using RVTR.Media.Domain.Models;

namespace RVTR.Media.Context.Repositories
{
  /// <summary>
  /// Represents the _Repository_ generic
  /// </summary>
  /// <typeparam name="TEntity"></typeparam>
  public class Repository<TEntity> : IRepository<TEntity> where TEntity : AEntity
  {
    private readonly DbSet<TEntity> _dbSet;
    

    private readonly MediaContext _context;

    /// <summary>
    ///
    /// </summary>
    /// <param name="context"></param>
    public Repository(MediaContext context)
    {
      _dbSet = context.Set<TEntity>();
      _context = context;
     
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
    public virtual async Task InsertAsync(TEntity entry){

      await _dbSet.AddAsync(entry).ConfigureAwait(true);
    } 

    public virtual async Task UploadAsync(IFormFile file,MediaModel entry, BlobServiceClient blobServiceClient )
    {
      var containerName = entry.Group;
      var fileName = file.FileName;
      string FileExtention = fileName.Substring(fileName.Length - 4);
      var blopName = entry.GroupIdentifier + System.Guid.NewGuid().ToString() + FileExtention;
      var blobClient = _GetBlobClient(containerName, blopName, blobServiceClient);
      await blobClient.UploadAsync(file.OpenReadStream());
      entry.Uri = blobClient.Uri.ToString();
      await _context.Medias.AddAsync(entry);

    }

    private static BlobClient _GetBlobClient(string containerName, string blobName, BlobServiceClient blobServiceClient)
    {
      var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
      return containerClient.GetBlobClient(blobName);
    }

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
