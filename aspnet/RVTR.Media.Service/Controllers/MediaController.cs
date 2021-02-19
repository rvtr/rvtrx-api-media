using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    /// <param name="file"></param>
    /// <param name="group"></param>
    /// <param name="groupidentifier"></param>
    /// <returns></returns>
    [HttpPost("{group}/{groupidentifier}")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> Post([FromForm] IFormFileCollection file, string group, string groupidentifier)
    {
      
      
      _logger.LogDebug("adding media");

      await _unitOfWork.Media.InsertAsync(media);
      await _unitOfWork.CommitAsync();

      _logger.LogInformation($"added media");

      return Accepted(media);

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
