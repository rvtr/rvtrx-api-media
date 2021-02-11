using Microsoft.EntityFrameworkCore;
using RVTR.Media.Context;
using RVTR.Media.Context.Repositories;
using RVTR.Media.Domain.Models;
using Xunit;

namespace RVTR.Media.Testing.Tests
{
  public class RepositoryTest : DataTest
  {
    private readonly MediaModel _media = new MediaModel() { Id = 3 };

    [Fact]
    public async void Test_Repository_DeleteAsync()
    {
      using (var ctx = new MediaContext(Options))
      {
        var medias = new Repository<MediaModel>(ctx);
        var media = await ctx.Medias.FirstAsync();
        await medias.DeleteAsync(media.Id);
        Assert.Equal(EntityState.Deleted, ctx.Entry(media).State);
      }
    }

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

    [Fact]
    public async void Test_Repository_SelectAsync()
    {
      using (var ctx = new MediaContext(Options))
      {
        var medias = new Repository<MediaModel>(ctx);
        var actual = await medias.SelectAsync();

        Assert.NotEmpty(actual);
      }
    }

    [Fact]
    public async void Test_Repository_Update()
    {
      using (var ctx = new MediaContext(Options))
      {
        var medias = new Repository<MediaModel>(ctx);
        var media = await ctx.Medias.FirstAsync();

        medias.Update(media);

        var result = ctx.Medias.Find(media.Id);
        Assert.Equal(EntityState.Modified, ctx.Entry(result).State);
      }
    }
  }
}
