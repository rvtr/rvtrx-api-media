using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RVTR.Media.Domain.Models
{
  /// <summary>
  /// Represents the _Media_ model
  /// </summary>
  /// <param name="Id"></param>
  /// <param name="GroupId"></param>
  /// <param name="Group"></param>
  /// <param name="FileSize"></param>
  /// <param name="FileType"></param>
  /// <param name="Uri"></param>
  /// <param name="AltText"></param>
  public class MediaModel : IValidatableObject
  {

    public const long MaxFileSize = 5000000;
    public int Id { get; set; } //  Id of image from DB

    [Required(ErrorMessage = "GroupId is required")]
    public int GroupId { get; set; } // Id of group it belongs to

    [Required(ErrorMessage = "Group is required")]
    [RegularExpression(@"(profile|campground|campsite)", ErrorMessage = "Account affiliation not recognized.")]
    public string Group { get; set; } // user affiliation (profile, campground, campsite)

    [Required(ErrorMessage = "FileType is required")]
    [MaxLength(100, ErrorMessage = "FileName is to long")]
    [RegularExpression(@".*\.(jpg|JPG|png|PNG)", ErrorMessage = "File type must be correct.")]
    public string FileType { get; set; } //azure cosmos content type
    public string Uri { get; set; } // where the image is IMPORTANT
    public string AltText { get; set; } // text description

    /// <summary>
    /// Empty constructor
    /// </summary>
    public MediaModel() { }


    /// <summary>
    /// Represents the _Media_ `Validate` method
    /// </summary>
    /// <param name="validationContext"></param>
    /// <returns>List of Validation result</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
      List<ValidationResult> result = new List<ValidationResult>();

      if (string.Equals(GroupId.ToString(), "0"))
      {
        result.Add(new ValidationResult("GroupId can not be null."));
      }
      if (string.IsNullOrEmpty(Group))
      {
        result.Add(new ValidationResult("Group can not be null."));
      }
      if (string.IsNullOrEmpty(FileType))
      {
        result.Add(new ValidationResult("FileType can not be null."));
      }

      return result;
    }
  }
}
