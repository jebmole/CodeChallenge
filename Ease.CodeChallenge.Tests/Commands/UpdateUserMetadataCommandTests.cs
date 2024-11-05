using Ease.CodeChallenge.Application.Exceptions;
using Ease.CodeChallenge.Application.Features.GuidMetadata.Commands;
using Ease.CodeChallenge.Application.Interfaces;
using Ease.CodeChallenge.Domain.Entities;
using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;

namespace Ease.CodeChallenge.Tests.Commands
{
    public class UpdateUserMetadataCommandTests
    {
        private Mock<IApplicationContext> _applicationContextMock;
        private readonly string ExistingGuid = Guid.NewGuid().ToString("N").ToUpper();
        public UpdateUserMetadataCommandTests()
        {
            _applicationContextMock = new Mock<IApplicationContext>();

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
        public async Task HandleUpdateGuid_GuidDoesNotExists_Returns_InvalidGuidException()
        {
            //Arrange
            var command = new UpdateUserMetadataCommand()
            {
                Guid = Guid.NewGuid().ToString("N").ToUpper(),
            };

            var commandHandler = new UpdateUserMetadataCommandHandler(_applicationContextMock.Object);

            //Act and Assert
            await commandHandler.Awaiting(x => x.Handle(command, default))
                .Should().ThrowAsync<InvalidGuidException>()
                .WithMessage("Guid does not exist");
        }

        [Fact]
        public async Task HandleUpdateGuid_GuidAndExpireDateAreValid_Returns_GuidWithUpdatedDate()
        {
            //Arrange
            var command = new UpdateUserMetadataCommand()
            {
                Guid = ExistingGuid,
                Expires = DateTime.Now.AddDays(30),
            };

            var commandHandler = new UpdateUserMetadataCommandHandler(_applicationContextMock.Object);

            //Act
            var response = await commandHandler.Handle(command, default);

            //Assert
            response.Expires.Should().Be(command.Expires);
        }
    }
}
