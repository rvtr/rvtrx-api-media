using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RVTR.Media.Domain.Models
{
  /// <summary>
  /// Represents the _Media_ model
  /// </summary>
  /// <param name="Id"></param>
  /// <param name="Uri"></param>
  /// <param name="AltText"></param>
  public class MediaModel : IValidatableObject
  {
    public int Id { get; set; } 
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
