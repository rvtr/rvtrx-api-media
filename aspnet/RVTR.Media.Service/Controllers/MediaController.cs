using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RVTR.Media.Domain.Interfaces;
using RVTR.Media.Domain.Models;

namespace RVTR.Media.Service.Controllers
{
  /// <summary>
  ///
  /// </summary>
  [ApiController]
  [ApiVersion("0.0")]
  [EnableCors("Public")]
  [Route("rest/media/{version:apiVersion}/[controller]")]
  public class MediaController : ControllerBase
  {
    private readonly ILogger<MediaController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    private readonly IConfiguration _configuration;
     private readonly BlobServiceClient _blobServiceClient;



    /// <summary>
    ///
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="unitOfWork"></param>
    /// <param name="configuration"></param>
    /// <param name="blobServiceClient"></param>
    public MediaController(ILogger<MediaController> logger, IUnitOfWork unitOfWork, IConfiguration configuration,
      BlobServiceClient blobServiceClient)
    {
      _blobServiceClient = blobServiceClient;
      _logger = logger;
      _unitOfWork = unitOfWork;
      _configuration = configuration;
    }

  /// <summary>
  ///
  /// </summary>
  /// <param name="mediaId"></param>
  /// <returns></returns>
  [HttpDelete("{mediaId}")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> Delete(string mediaId)
  {
    try
    {
      _logger.LogDebug("deleting media");

      var mediaModel = (await _unitOfWork.Media.SelectAsync(e => e.id == mediaId)).First();

      string blobName = mediaModel.Uri.Substring(mediaModel.Uri.LastIndexOf('/') + 1);

     
      BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(mediaModel.Group);
      await containerClient.DeleteBlobIfExistsAsync(blobName);

      await _unitOfWork.Media.DeleteAsync(mediaModel.id);
      await _unitOfWork.CommitAsync();


      _logger.LogInformation($"deleted media");

      return Ok(mediaModel);
    }
    catch
    {
      _logger.LogWarning($"missing media");

      return NotFound();
    }
  }

  /// <summary>
  ///
  /// </summary>
  /// <param name="groupidentifier"></param>
  /// <returns></returns>
  [HttpGet("{groupidentifier}")]
  [ProducesResponseType(typeof(IEnumerable<MediaModel>), StatusCodes.Status200OK)]
  public async Task<IActionResult> Get(string groupidentifier)
  {
    _logger.LogInformation($"retrieve media");

    return Ok(await _unitOfWork.Media.SelectAsync(x => x.GroupIdentifier == groupidentifier));
  }


  /// <summary>
  ///
  /// </summary>
  /// <param name="files"></param>
  /// <param name="group"></param>
  /// <param name="groupidentifier"></param>
  /// <returns></returns>
  [HttpPost("{group}/{groupidentifier}")]
  [ProducesResponseType(StatusCodes.Status202Accepted)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> Post([FromForm] IFormFileCollection files, string group, string groupidentifier)
  {
    Regex FileExtensionRegex = new Regex(@"([a-zA-Z0-9\s_\.-:])+\.(png|jpg)$");

    if (!files.Any())
    {
      return BadRequest("No files given");
    }

    foreach (var file in files)
    {
      if (file.Length > (5 * 1024 * 1024))
      {
        return BadRequest("File too large");
      }
      if (!FileExtensionRegex.IsMatch(file.FileName))
      {
        return BadRequest("Invalid file extention");
      }
    }

    foreach (var file in files)
    {

      MediaModel model = new MediaModel();
      model.Group = group;
      model.GroupIdentifier = groupidentifier;

      switch (group)
      {
        case "profiles":
        case "campgrounds":
        case "campsites":
          {
            _logger.LogDebug("uploading media");

            await _unitOfWork.Media.UploadAsync(file, model, _blobServiceClient);

            _logger.LogDebug("uploaded media");

            model.AltText = "Picture of " + model.GroupIdentifier;

            _logger.LogDebug("adding media model");

            await _unitOfWork.Media.InsertAsync(model);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation($"added media model");

            break;
          }

        default:
          {
            return BadRequest("Invalid group entered");
          }
      }
    }

    return Accepted();
  }
}
}
