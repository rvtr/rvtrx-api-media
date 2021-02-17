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
            Id = 0,
            Uri = "https://",
            AltText = "alternate text"
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

    
    [Theory]
    [MemberData(nameof(Medias))]
    public void Test_Validate_MediaModel(MediaModel media)
    {
      var validationContext = new ValidationContext(media);

      Assert.Empty(media.Validate(validationContext));
    }

    [Fact]
    public void Test_Validate_MediaModel_EmptyID()
    {
      MediaModel media = new MediaModel(){Uri = "https://notblobstorage/%22", AltText="hat"};

      var validationContext = new ValidationContext(media);
      var actual = Validator.TryValidateObject(media, validationContext, null, true);

      Assert.True(actual);
    }


    [Fact]
    public void Test_Validate_MediaModel_EmptyUri()
    {
      MediaModel media = new MediaModel(){Id = 12345, AltText="hat"};

      var validationContext = new ValidationContext(media);
      
      Assert.NotEmpty(media.Validate(validationContext));
    }

    [Fact]
    public void Test_Validate_MediaModel_EmptyAltText()
    {
      MediaModel media = new MediaModel(){Id = 12345, Uri = "https://notblobstorage/%22"};

      var validationContext = new ValidationContext(media);
      
      Assert.NotEmpty(media.Validate(validationContext));
    }
  }
}
