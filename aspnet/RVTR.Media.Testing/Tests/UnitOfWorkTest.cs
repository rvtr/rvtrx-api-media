using RVTR.Media.Context;
using RVTR.Media.Context.Repositories;
using Xunit;

namespace RVTR.Media.Testing.Tests
{
  public class UnitOfWorkTest : DataTest
  {
    [Fact]
    public async void Test_UnitOfWork_CommitAsync()
    {
      using var ctx = new MediaContext(Options);
      var unitOfWork = new UnitOfWork(ctx);
      var actual = await unitOfWork.CommitAsync();

      Assert.NotNull(unitOfWork.Media);
      Assert.Equal(0, actual);
    }
  }
}
