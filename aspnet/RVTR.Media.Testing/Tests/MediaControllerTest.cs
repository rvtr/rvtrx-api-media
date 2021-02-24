using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
    private readonly IConfiguration _configuration;

    public static readonly IEnumerable<object[]> Medias = new List<object[]>
    {
      new object[]
      {
        new FormFileCollection
        {

        },
        new string("campgrounds")
        {

        },
        new string("camplazlo")
      }
    };

    public MediaControllerTest()
    {
      var loggerMock = new Mock<ILogger<MediaController>>();
      var repositoryMock = new Mock<IRepository<MediaModel>>();
      var unitOfWorkMock = new Mock<IUnitOfWork>();
      var configurationMock = new Mock<IConfiguration>();

      repositoryMock.Setup(m => m.DeleteAsync("0")).Throws(new Exception());
      repositoryMock.Setup(m => m.DeleteAsync("1")).Returns(Task.CompletedTask);
      repositoryMock.Setup(m => m.InsertAsync(It.IsAny<MediaModel>())).Returns(Task.CompletedTask);
      repositoryMock.Setup(m => m.SelectAsync()).ReturnsAsync((IEnumerable<MediaModel>)null);
      repositoryMock.Setup(m => m.SelectAsync(e => e.EntityId == 0)).Throws(new Exception());
      repositoryMock.Setup(m => m.SelectAsync(e => e.EntityId == 1)).ReturnsAsync((IEnumerable<MediaModel>)null);
      repositoryMock.Setup(m => m.Update(It.IsAny<MediaModel>()));
      unitOfWorkMock.Setup(m => m.Media).Returns(repositoryMock.Object);

      _logger = loggerMock.Object;
      _unitOfWork = unitOfWorkMock.Object;
      _configuration = configurationMock.Object;
      _controller = new MediaController(_logger, _unitOfWork, _configuration);
    }

    [Fact]
    public async void Test_Controller_Delete()
    {
      var resultFail = await _controller.Delete("0");
      var resultPass = await _controller.Delete("1");

      Assert.NotNull(resultFail);
      Assert.NotNull(resultPass);
    }

    [Theory]
    [MemberData(nameof(Medias))]
    public async void Test_Controller_Post_EmptyFile(IFormFileCollection files, string group, string groupidentifier)
    {
      var resultFail = await _controller.Post(files, group, groupidentifier);

      Assert.IsType<BadRequestObjectResult>(resultFail);
    }

    [Fact]
    public async void Test_Contoller_Post_BadFileType()
    {
      var fileMock = new Mock<IFormFile>();
      var content = "Hello World from a Fake File";
      var fileName = "test.pdf";
      var ms = new MemoryStream();
      var writer = new StreamWriter(ms);
      writer.Write(content);
      writer.Flush();
      ms.Position = 0;
      fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
      fileMock.Setup(_ => _.FileName).Returns(fileName);
      fileMock.Setup(_ => _.Length).Returns(ms.Length);

      var resultFail = await _controller.Post(new FormFileCollection { fileMock.Object }, "campgrounds", "camplazlo");
      Assert.IsType<BadRequestObjectResult>(resultFail);
    }

    [Fact]
    public async void Test_Contoller_Post_BadFileSize()
    {
      var fileMock = new Mock<IFormFile>();
      var content = "Hello World from a Fake File";
      var fileName = "test.png";
      var ms = new MemoryStream();
      var writer = new StreamWriter(ms);
      writer.Write(content);
      writer.Flush();
      ms.Position = 0;
      fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
      fileMock.Setup(_ => _.FileName).Returns(fileName);
      fileMock.Setup(_ => _.Length).Returns(ms.Length + 1000000000000);

      var resultFail = await _controller.Post(new FormFileCollection { fileMock.Object }, "campgrounds", "camplazlo");
      Assert.IsType<BadRequestObjectResult>(resultFail);
    }
  }
}
