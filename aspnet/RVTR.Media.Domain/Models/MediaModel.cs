using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RVTR.Media.Domain.Models
{
  /// <summary>
  /// Represents the _Media_ model
  /// </summary>
  /// <param name="MediaId"></param>
  /// <param name="Group">profile or campground or campsite</param>
  /// <param name="GroupdIdentifier">For profile use email. For campground use campgroundName. For campsite use campgroundName-lotNumber</param>
  /// <param name="Uri"></param>
  /// <param name="AltText"></param>
  public class MediaModel : IValidatableObject
  {
    public int MediaId { get; set; }

    [Required(ErrorMessage = "Group is required")]
    [RegularExpression(@"(profile|campground|campsite)", ErrorMessage = "Group affiliation not recognized.")]
    public string Group { get; set; }

    [Required(ErrorMessage = "GroupIdentifier is required")]
    [RegularExpression(@"((([a-zA-Z]+)([a-zA-Z0-9_\-\.]*)@([a-zA-Z0-9_\-\.]*)([a-zA-Z]+)\.([a-zA-Z]{2,5}))|(([a-zA-Z]+)([a-zA-Z_]*))|(([a-zA-Z]+)\-([0-9]+)))", ErrorMessage = "Group Identifier not recognized.")]
    public string GroupIdentifier { get; set; }

    [Required(ErrorMessage = "URL is required")]
    public string Uri { get; set; }

    [Required(ErrorMessage = "AltText is required")]
    public string AltText { get; set; }

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
      if (string.IsNullOrEmpty(Group))
      {
        result.Add(new ValidationResult("Group can not be null."));
      }
      if (string.IsNullOrEmpty(GroupIdentifier))
      {
        result.Add(new ValidationResult("GroupIdentifier can not be null."));
      }
      if (string.IsNullOrEmpty(Uri))
      {
        result.Add(new ValidationResult("URL can not be null."));
      }
      if (string.IsNullOrEmpty(AltText))
      {
        result.Add(new ValidationResult("AltText can not be null."));
      }

      return result;
    }
  }
}
