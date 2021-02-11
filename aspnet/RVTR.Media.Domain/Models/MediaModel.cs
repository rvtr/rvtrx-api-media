using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RVTR.Media.Domain.Models
{
  /// <summary>
  /// Represents the _Media_ model
  /// </summary>
  public class MediaModel : IValidatableObject
  {
    public int Id { get; set; } // id that it belongs to
    public string Group { get; set; } // user affiliation (profile, campground, campsite)
    public int FileSize { get; set; } //5mb max
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
    /// <returns></returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
      return null;
    }
  }
}
