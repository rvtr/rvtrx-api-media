using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RVTR.Media.Domain.Abstracts;

namespace RVTR.Media.Domain.Models
{
  /// <summary>
  /// Represents the _MediaModel_ class
  /// </summary>
  public class MediaModel : AEntity, IValidatableObject
  {
<<<<<<< HEAD
=======
    public long MediaId { get; set; }

>>>>>>> fixed cosmos connection, changed mediaId to long
    [Required(ErrorMessage = "Group is required")]
    [RegularExpression(@"(profiles|campgrounds|campsites)", ErrorMessage = "Group affiliation not recognized.")]
    public string Group { get; set; }

    [Required(ErrorMessage = "GroupIdentifier is required")]
    [RegularExpression(@"((([a-zA-Z]+)(((_|\-|\.)([a-zA-Z0-9]+))*)@((([a-zA-Z0-9]+)(_|\-|\.))*)([a-zA-Z]+)\.([a-zA-Z]{2,5}))|(([a-zA-Z]+)((_[a-zA-Z]+)*))(\-([0-9]+))?)", ErrorMessage = "Group Identifier not recognized.")]
    public string GroupIdentifier { get; set; }

    [Required(ErrorMessage = "URL is required")]
    public string Uri { get; set; }

    [Required(ErrorMessage = "AltText is required")]
    public string AltText { get; set; }

    /// <summary>
<<<<<<< HEAD
    /// Represents the _MediaModel_ `Validate` method
=======
    /// Empty constructor
    /// </summary>
    public MediaModel() { MediaId = System.DateTime.Now.Ticks; }


    /// <summary>
    /// Represents the _Media_ `Validate` method
>>>>>>> fixed cosmos connection, changed mediaId to long
    /// </summary>
    /// <param name="validationContext"></param>
    /// <returns>List of Validation result</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
      if (string.IsNullOrEmpty(Group))
      {
        yield return new ValidationResult("Group can not be null.");
      }
      if (string.IsNullOrEmpty(GroupIdentifier))
      {
        yield return new ValidationResult("GroupIdentifier can not be null.");
      }
      if (string.IsNullOrEmpty(Uri))
      {
        yield return new ValidationResult("URL can not be null.");
      }
      if (string.IsNullOrEmpty(AltText))
      {
        yield return new ValidationResult("AltText can not be null.");
      }
    }
  }
}
