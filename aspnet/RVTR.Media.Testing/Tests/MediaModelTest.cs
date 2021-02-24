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
            EntityId = 0,
            Group = "profiles",
            GroupIdentifier = "your.name@email.com",
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
      MediaModel media = new MediaModel(){Group = "profiles", GroupIdentifier = "your.name@email.com", Uri = "https://notblobstorage/%22", AltText="hat"};

      var validationContext = new ValidationContext(media);
      var actual = Validator.TryValidateObject(media, validationContext, null, true);

      Assert.True(actual);
    }

    [Fact]
    public void Test_Validate_MediaModel_EmptyGroup()
    {
      MediaModel media = new MediaModel(){EntityId = 12345, GroupIdentifier = "your.name@email.com", Uri = "https://notblobstorage/%22", AltText="hat"};

      var validationContext = new ValidationContext(media);

      Assert.NotEmpty(media.Validate(validationContext));
    }

    [Fact]
    public void Test_Validate_MediaModel_EmptyGroupIdentifier()
    {
      MediaModel media = new MediaModel(){EntityId = 12345, Group = "profiles", Uri = "https://notblobstorage/%22", AltText="hat"};

      var validationContext = new ValidationContext(media);

      Assert.NotEmpty(media.Validate(validationContext));
    }


    [Fact]
    public void Test_Validate_MediaModel_EmptyUri()
    {
      MediaModel media = new MediaModel(){EntityId = 12345, Group = "profiles", GroupIdentifier = "your.name@email.com",  AltText="hat"};

      var validationContext = new ValidationContext(media);

      Assert.NotEmpty(media.Validate(validationContext));
    }

    [Fact]
    public void Test_Validate_MediaModel_EmptyAltText()
    {
      MediaModel media = new MediaModel(){EntityId = 12345, Group = "profiles", GroupIdentifier = "your.name@email.com", Uri = "https://notblobstorage/%22"};

      var validationContext = new ValidationContext(media);

      Assert.NotEmpty(media.Validate(validationContext));
    }

    [Fact]
    public void Test_Validate_MediaModel_ValidGroups()
    {
      MediaModel mediaProfile = new MediaModel(){EntityId = 12345, Group = "profiles", GroupIdentifier = "your.name@email.com", Uri = "https://notblobstorage/%22",  AltText="hat"};
      MediaModel mediaCampground = new MediaModel(){EntityId = 12345, Group = "campgrounds", GroupIdentifier = "your.name@email.com", Uri = "https://notblobstorage/%22",  AltText="hat"};
      MediaModel mediaCampsite = new MediaModel(){EntityId = 12345, Group = "campsites", GroupIdentifier = "your.name@email.com", Uri = "https://notblobstorage/%22",  AltText="hat"};

      var validationContext = new ValidationContext(mediaProfile);
      var actual = Validator.TryValidateObject(mediaProfile, validationContext, null, true);

      Assert.True(actual);

      validationContext = new ValidationContext(mediaCampground);
      actual = Validator.TryValidateObject(mediaCampground, validationContext, null, true);

      Assert.True(actual);

      validationContext = new ValidationContext(mediaCampsite);
      actual = Validator.TryValidateObject(mediaCampsite, validationContext, null, true);

      Assert.True(actual);
    }

    [Fact]
    public void Test_Validate_MediaModel_BadGroup()
    {
      MediaModel media = new MediaModel(){EntityId = 12345, Group = "profile1", GroupIdentifier = "your.name@email.com", Uri = "https://notblobstorage/%22",  AltText="hat"};

      var validationContext = new ValidationContext(media);
      var actual = Validator.TryValidateObject(media, validationContext, null, true);

      Assert.False(actual);
    }

    [Fact]
    public void Test_Validate_MediaModel_BadGroupIdentifierEmail()
    {
      MediaModel media = new MediaModel(){EntityId = 12345, Group = "profiles", GroupIdentifier = "your.name@email.com1", Uri = "https://notblobstorage/%22",  AltText="hat"};

      var validationContext = new ValidationContext(media);
      var actual = Validator.TryValidateObject(media, validationContext, null, true);

      Assert.False(actual);
    }

    [Fact]
    public void Test_Validate_MediaModel_BadGroupIdentifierCampground()
    {
      MediaModel media = new MediaModel()
      {
        EntityId = 12345, Group = "profiles", GroupIdentifier = "campground111", Uri = "https://notblobstorage/%22",  AltText="hat"
      };

      var validationContext = new ValidationContext(media);
      var actual = Validator.TryValidateObject(media, validationContext, null, true);

      Assert.False(actual);
    }

    [Fact]
    public void Test_Validate_MediaModel_ValidGroupIdentifierCampsite()
    {
      MediaModel media = new MediaModel(){EntityId = 12345, Group = "profiles", GroupIdentifier = "campground-111", Uri = "https://notblobstorage/%22",  AltText="hat"};

      var validationContext = new ValidationContext(media);
      var actual = Validator.TryValidateObject(media, validationContext, null, true);

      Assert.True(actual);
    }

    [Fact]
    public void Test_Validate_MediaModel_BadGroupIdentifierCampsite()
    {
      MediaModel media = new MediaModel(){EntityId = 12345, Group = "profiles", GroupIdentifier = "campground.111", Uri = "https://notblobstorage/%22",  AltText="hat"};

      var validationContext = new ValidationContext(media);
      var actual = Validator.TryValidateObject(media, validationContext, null, true);

      Assert.False(actual);
    }
  }
}
