using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using RVTR.Media.Domain.Interfaces;
using RVTR.Media.Domain.Models;
using RVTR.Media.Service.Controllers;
using Xunit;

namespace RVTR.Media.Testing.Tests
{
  public class MediaControllerTest
  {
    private readonly MediaController _controller;
    private readonly ILogger<MediaController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public MediaControllerTest()
    {
      var loggerMock = new Mock<ILogger<MediaController>>();
      var repositoryMock = new Mock<IRepository<MediaModel>>();
      var unitOfWorkMock = new Mock<IUnitOfWork>();

      repositoryMock.Setup(m => m.DeleteAsync(0)).Throws(new Exception());
      repositoryMock.Setup(m => m.DeleteAsync(1)).Returns(Task.CompletedTask);
      repositoryMock.Setup(m => m.InsertAsync(It.IsAny<MediaModel>())).Returns(Task.CompletedTask);
      repositoryMock.Setup(m => m.SelectAsync()).ReturnsAsync((IEnumerable<MediaModel>)null);
      repositoryMock.Setup(m => m.SelectAsync(e => e.EntityId == 0)).Throws(new Exception());
      repositoryMock.Setup(m => m.SelectAsync(e => e.EntityId == 1)).ReturnsAsync((IEnumerable<MediaModel>)null);
      repositoryMock.Setup(m => m.Update(It.IsAny<MediaModel>()));
      unitOfWorkMock.Setup(m => m.Media).Returns(repositoryMock.Object);

      _logger = loggerMock.Object;
      _unitOfWork = unitOfWorkMock.Object;
      _controller = new MediaController(_logger, _unitOfWork);
    }

    [Fact]
    public async void Test_Controller_Delete()
    {
      var resultFail = await _controller.Delete(0);
      var resultPass = await _controller.Delete(1);

      Assert.NotNull(resultFail);
      Assert.NotNull(resultPass);
    }

    [Fact]
    public async void Test_Controller_Post()
    {
      var resultPass = await _controller.Post(new MediaModel());

      Assert.NotNull(resultPass);
    }

    [Fact]
    public async void Test_Controller_Put()
    {
      var resultPass = await _controller.Put(new MediaModel());

      Assert.NotNull(resultPass);
    }
  }
}
