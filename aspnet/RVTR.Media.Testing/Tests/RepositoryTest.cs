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
    public async void Test_Repository_InsertAsync()
    {
      using (var ctx = new MediaContext(Options))
      {
        var medias = new Repository<MediaModel>(ctx);
        await medias.InsertAsync(_media);
        Assert.Equal(EntityState.Added, ctx.Entry(_media).State);
      }
    }
  }
}
