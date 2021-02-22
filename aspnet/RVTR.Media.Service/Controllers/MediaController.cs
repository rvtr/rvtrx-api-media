using System.Collections.Generic;
using System.Linq;
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

    /// <summary>
    ///
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="unitOfWork"></param>
    public MediaController(ILogger<MediaController> logger, IUnitOfWork unitOfWork, IConfiguration configuration)
    {
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
    public async Task<IActionResult> Delete(int mediaId)
    {
      try
      {
        _logger.LogDebug("deleting media");

        var mediaModel = (await _unitOfWork.Media.SelectAsync(e => e.EntityId == id)).FirstOrDefault();

        await _unitOfWork.Media.DeleteAsync(mediaModel.EntityId); //look at this
        BlobClient blob = new BlobClient(new System.Uri(mediaModel.Uri));
        await blob.DeleteAsync();

        await _unitOfWork.Media.DeleteAsync(mediaModel.MediaId);
        await _unitOfWork.CommitAsync();


        _logger.LogInformation($"deleted media");

        return Ok();
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

      return Ok(await _unitOfWork.Media.SelectAsync(groupidentifier));
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
      List<MediaModel> AcceptedModels = new List<MediaModel>();

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
        BlobServiceClient blobServiceClient = new BlobServiceClient(_configuration.GetConnectionString("storage"));

        string FileExtention = file.FileName.Substring(file.FileName.Length - 4);

        MediaModel model = new MediaModel();
        model.Group = group;
        model.GroupIdentifier = groupidentifier;

        switch (group)
        {
          case "profile":
          case "campground":
          case "campsite":
            {
              _logger.LogDebug("uploading media");

              BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(model.Group);
              BlobClient blobClient = containerClient.GetBlobClient(model.GroupIdentifier + System.Guid.NewGuid().ToString() + FileExtention);

              await blobClient.UploadAsync(file.OpenReadStream());

              _logger.LogDebug("uploaded media");

              model.Uri = blobClient.Uri.ToString();
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
