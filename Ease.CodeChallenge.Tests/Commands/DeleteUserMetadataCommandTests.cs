using Ease.CodeChallenge.Application.Exceptions;
using Ease.CodeChallenge.Application.Features.GuidMetadata.Commands;
using Ease.CodeChallenge.Application.Interfaces;
using Ease.CodeChallenge.Domain.Entities;
using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;

namespace Ease.CodeChallenge.Tests.Commands
{
    public class DeleteUserMetadataCommandTests
    {
        private Mock<IApplicationContext> _applicationContextMock;
        private readonly string ExistingGuid = Guid.NewGuid().ToString("N").ToUpper();
        public DeleteUserMetadataCommandTests()
        {
            _applicationContextMock = new Mock<IApplicationContext>();
            _applicationContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

            var existingData = new List<UserMetadata>()
            {
                new UserMetadata
                {
                    Guid = ExistingGuid,
                    Expires = DateTime.Now,
                    User = "Jonathan Estrada"
                }
            };

            _applicationContextMock.Setup(x => x.UserMetadata).ReturnsDbSet(existingData);
        }

        [Fact]
        public async Task HandleDeleteGuid_GuidDoesNotExists_Returns_InvalidGuidException()
        {
            //Arrange
            var command = new DeleteUserMetadataCommand()
            {
                Guid = Guid.NewGuid().ToString("N").ToUpper(),
            };

            var commandHandler = new DeleteUserMetadataCommandHandler(_applicationContextMock.Object);

            //Act and Assert
            await commandHandler.Awaiting(x => x.Handle(command, default))
                .Should().ThrowAsync<InvalidGuidException>()
                .WithMessage("Guid does not exist");
        }

        [Fact]
        public async Task HandleDeleteGuid_GuidIsValid_Returns_True()
        {
            //Arrange
            var command = new DeleteUserMetadataCommand()
            {
                Guid = ExistingGuid,
            };

            var commandHandler = new DeleteUserMetadataCommandHandler(_applicationContextMock.Object);

            //Act
            var response = await commandHandler.Handle(command, default);

            //Assert
            response.Should().BeTrue();
        }
    }
}
