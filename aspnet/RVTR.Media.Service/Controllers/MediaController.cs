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

    /// <summary>
    ///
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="unitOfWork"></param>
    /// <param name="configuration"></param>
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
    public async Task<IActionResult> Delete(string mediaId)
    {
      try
      {
        _logger.LogDebug("deleting media");

        var mediaModel = (await _unitOfWork.Media.SelectAsync(e => e.id == mediaId)).First();

        string blobName = mediaModel.Uri.Substring(mediaModel.Uri.LastIndexOf('/') + 1);

        BlobServiceClient blobServiceClient = new BlobServiceClient(_configuration.GetConnectionString("storage"));
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(mediaModel.Group);
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
    [RequestSizeLimit(100 * 1024 * 1024)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromForm] IFormFileCollection files, string group, string groupidentifier)
    {
      Regex FileNameRegex = new Regex(@"([a-zA-Z0-9\s_\.-:])+\.+.*$");
      Regex PictureRegexExtension = new Regex(@"^.*\.(jpg|JPG|gif|GIF|png|PNG|jpeg|JPEG)$");
      Regex AudioRegexExtension = new Regex(@"^.*\.(mp3|wav|WAV|MP3|flac|FLAC)$");
      Regex VideoRegexExtension = new Regex(@"^.*\.(mp4|MP4|MOV|mov|WMV|wmv|AVI|avi|WEBM|webm)$");
      Regex Extensions = new Regex(@"\.(mp3|wav|WAV|MP3|jpg|JPG|gif|GIF|png|PNG|flac|FLAC|jpeg|JPEG|mp4|MP4|MOV|mov|WMV|wmv|AVI|avi|WEBM|webm)$");

      if (!files.Any())
      {
        return BadRequest("No files given");
      }

      foreach (var file in files)
      {
        if (!FileNameRegex.IsMatch(file.FileName))
        {
          return BadRequest("Invalid file name");
        }
        if ((!PictureRegexExtension.IsMatch(file.FileName)) && (!AudioRegexExtension.IsMatch(file.FileName)) && (!VideoRegexExtension.IsMatch(file.FileName)))
        {
          return BadRequest("Invalid file extention");
        }
        if (PictureRegexExtension.IsMatch(file.FileName))
        {
          if (file.Length > (5 * 1024 * 1024))
          {
            return BadRequest("File too large (5mb Max)");
          }
        }
        if (AudioRegexExtension.IsMatch(file.FileName))
        {
          if (file.Length > (12 * 1024 * 1024))
          {
            return BadRequest("File too large (12mb Max)");
          }
        }
        if (VideoRegexExtension.IsMatch(file.FileName))
        {
          if (file.Length > (100 * 1024 * 1024))
          {
            return BadRequest("File too large (100mb Max)");
          }
        }

      }
      BlobServiceClient blobServiceClient = new BlobServiceClient(_configuration.GetConnectionString("storage"));
      foreach (var file in files)
      {
        Match extensionmatch = Extensions.Match(file.FileName);
        string extension = extensionmatch.ToString();
        MediaModel model = new MediaModel();
        model.Group = group;
        model.GroupIdentifier = groupidentifier;

        switch (group)
        {
          case "audio":
          case "video":
          case "profiles":
          case "campgrounds":
          case "campsites":
            {
              _logger.LogDebug("uploading media");

              BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(model.Group);
              BlobClient blobClient = containerClient.GetBlobClient(model.GroupIdentifier + System.Guid.NewGuid().ToString() + extension);

              await blobClient.UploadAsync(file.OpenReadStream());

              _logger.LogDebug("uploaded media");

              model.Uri = blobClient.Uri.ToString();
              if (PictureRegexExtension.IsMatch(file.FileName))
              {
                model.AltText = "Picture of " + model.GroupIdentifier;
              }
              else if (AudioRegexExtension.IsMatch(file.FileName))
              {
                model.AltText = "Audio of " + model.GroupIdentifier;
              }
              else if (VideoRegexExtension.IsMatch(file.FileName))
              {
                model.AltText = "Video of " + model.GroupIdentifier;
              }
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
