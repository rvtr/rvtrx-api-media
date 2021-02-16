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
            GroupId = 1,
            Group = "profile",
            FileType = ".jpg",
            Uri = "",
            AltText = ""
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
      MediaModel media = new MediaModel(){GroupId = 12345, Group ="profile", FileType = ".png",Uri = "https://notblobstorage/%22", AltText="hat"};

      var validationContext = new ValidationContext(media);
      var actual = Validator.TryValidateObject(media, validationContext, null, true);

      Assert.True(actual);
    }

    [Fact]
    public void Test_Validate_MediaModel_EmptyGroupID()
    {
      MediaModel media = new MediaModel(){Id = 12345, Group ="profile", FileType = ".png",Uri = "https://notblobstorage/%22", AltText="hat"};

      var validationContext = new ValidationContext(media);
      
      Assert.NotEmpty(media.Validate(validationContext));
    }

    [Fact]
    public void Test_Validate_MediaModel_EmptyGroup()
    {
      MediaModel media = new MediaModel(){Id = 12345,GroupId = 54321, FileType = ".png",Uri = "https://notblobstorage/%22", AltText="hat"};

      var validationContext = new ValidationContext(media);
      
      Assert.NotEmpty(media.Validate(validationContext));
    }

    [Fact]
    public void Test_Validate_MediaModel_EmptyFileType()
    {
      MediaModel media = new MediaModel(){Id = 12345,GroupId = 54321,Group = "profile", Uri = "https://notblobstorage/%22", AltText="hat"};

      var validationContext = new ValidationContext(media);
      
      Assert.NotEmpty(media.Validate(validationContext));
    }

    [Fact]
    public void Test_Validate_MediaModel_BadGroup()
    {
      MediaModel media = new MediaModel(){Id =12345, GroupId = 12345, Group ="profile12345", FileType = ".png",Uri = "https://notblobstorage/%22", AltText="hat"};

      var validationContext = new ValidationContext(media);
      var actual = Validator.TryValidateObject(media, validationContext, null, true);

      Assert.False(actual);
    }

    [Fact]
    public void Test_Validate_MediaModel_BadFileType()
    {
      MediaModel media = new MediaModel(){Id =12345, GroupId = 12345,Group ="profile", FileType = ".pnga",Uri = "https://notblobstorage/%22", AltText="hat"};

      var validationContext = new ValidationContext(media);
      var actual = Validator.TryValidateObject(media, validationContext, null, true);

      Assert.False(actual);
    }
  }
}
