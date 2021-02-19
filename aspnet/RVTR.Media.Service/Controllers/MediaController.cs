using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

    /// <summary>
    ///
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="unitOfWork"></param>
    public MediaController(ILogger<MediaController> logger, IUnitOfWork unitOfWork)
    {
      _logger = logger;
      _unitOfWork = unitOfWork;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="group"></param>
    /// <param name="groupidentifier"></param>
    /// <returns></returns>
    [HttpDelete("{group}/{groupidentifier}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string group, string groupidentifier)
    {
      try
      {
        _logger.LogDebug("deleting media");

        var mediaModel = (await _unitOfWork.Media.SelectAsync(e => e.EntityId == id)).FirstOrDefault();

        await _unitOfWork.Media.DeleteAsync(mediaModel.EntityId);
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
    /// <param name="group"></param>
    /// <param name="groupidentifier"></param>
    /// <returns></returns>
    [HttpGet("{group}/{groupidentifier}")]
    [ProducesResponseType(typeof(IEnumerable<MediaModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(string group, string groupidentifier)
    {
      _logger.LogInformation($"retrieve media");

      return Ok(await _unitOfWork.Media.SelectAsync());

    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MediaModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int id)
    {
      _logger.LogDebug("retrieving media");

      var mediaModel = (await _unitOfWork.Media.SelectAsync(e => e.EntityId == id)).FirstOrDefault();

      if (mediaModel is MediaModel thatMedia)
      {
        _logger.LogInformation($"retrieved media");

        return Ok(thatMedia);
      }

      _logger.LogWarning($"missing media");

      return NotFound();
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
        BlobServiceClient blobServiceClient = new BlobServiceClient(System.Environment.GetEnvironmentVariable("ConnectionStrings__storage"));

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

    /// <summary>
    ///
    /// </summary>
    /// <param name="media"></param>
    /// <param name="group"></param>
    /// <param name="groupidentifier"></param>
    /// <returns></returns>
    [HttpPut("{group}/{groupidentifier}")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put([FromBody] IFormFile media, string group, string groupidentifier)
    {
      try
      {
        _logger.LogDebug("updating media");

        _unitOfWork.Media.Update(media);
        await _unitOfWork.CommitAsync();

        _logger.LogInformation($"updated media");

        return Accepted(media);
      }

      catch
      {
        _logger.LogWarning($"missing media");

        return NotFound();
      }
    }
  }
}
