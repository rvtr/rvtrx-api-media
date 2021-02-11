using RVTR.Media.Context;
using RVTR.Media.Context.Repositories;
using Xunit;

namespace RVTR.Media.Testing.Tests
{
  public class MediaRepositoryTest : DataTest
  {
    [Fact]
    public async void Test_Repository_SelectAsync()
    {
      using (var ctx = new MediaContext(Options))
      {
        var medias = new MediaRepository(ctx);
        var actual = await medias.SelectAsync();

        Assert.NotEmpty(actual);
      }
    }
  }
}
