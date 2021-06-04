using Microsoft.EntityFrameworkCore;
using RVTR.Media.Context;
using RVTR.Media.Context.Repositories;
using RVTR.Media.Domain.Models;
using Xunit;

namespace RVTR.Media.Testing.Tests
{
  public class RepositoryTest : DataTest
  {
    private readonly MediaModel _media = new MediaModel()
    {
      EntityId = 3,
      Group = "string",
      GroupIdentifier = "string",
      Uri = "string",
      AltText = "string"
    };

    [Fact]
    public async void Test_Repository_InsertAsync()
    {
      using (var ctx = new MediaContext(Options))
      {
        var medias = new Repository<MediaModel>(ctx);
        await medias.InsertAsync(_media);
        Assert.Equal(EntityState.Added, ctx.Entry(_media).State);
      }
    }

    ///<summary>
    /// checks that DeleteAsync function removes entity from context
    ///</summary>
    [Fact]
    public async void Test_Repository_DeleteAsync()
    {
      using (var ctx = new MediaContext(Options))
      {
        var medias = new Repository<MediaModel>(ctx);
        ctx.Medias.Add(_media);
        ctx.SaveChanges();
        await medias.DeleteAsync(_media.id);
        Assert.Equal(EntityState.Deleted, ctx.Entry(_media).State);
      }
    }
  }
}
