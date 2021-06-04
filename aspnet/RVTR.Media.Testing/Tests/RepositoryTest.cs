using Microsoft.EntityFrameworkCore;
using RVTR.Media.Context;
using RVTR.Media.Context.Repositories;
using RVTR.Media.Domain.Models;
using Xunit;

namespace RVTR.Media.Testing.Tests
{
  public class RepositoryTest : DataTest
  {
    private readonly MediaModel _media = new MediaModel() { EntityId = 3 };

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
        var result = await medias.SelectAsync();
        Assert.NotNull(result);
      }
    }

    [Fact]
    public void Test_Repository_Update()
    {
      using (var ctx = new MediaContext(Options))
      {
        MediaModel temp = new MediaModel() { EntityId = 3 };
        var medias = new Repository<MediaModel>(ctx);
        medias.Update(temp);
        Assert.Equal(EntityState.Modified, ctx.Entry(temp).State);
      }
    }
  }
}
