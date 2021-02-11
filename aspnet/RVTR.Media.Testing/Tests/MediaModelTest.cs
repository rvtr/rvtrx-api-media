using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RVTR.Media.Domain.Models;
using Xunit;

namespace RVTR.Media.Testing.Tests
{
  public class MediaModelTest
  {
    public static readonly IEnumerable<object[]> Medias = new List<object[]>
    {
      new object[]
      {
        new MediaModel()
        {
          Id = 0
        }
      }
    };

    [Theory]
    [MemberData(nameof(Medias))]
    public void Test_Create_MediaModel(MediaModel media)
    {
      var validationContext = new ValidationContext(media);
      var actual = Validator.TryValidateObject(media, validationContext, null, true);

      Assert.True(actual);
    }
  }
}
