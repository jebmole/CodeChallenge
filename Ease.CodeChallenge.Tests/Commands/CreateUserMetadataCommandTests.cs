using Ease.CodeChallenge.Application.Exceptions;
using Ease.CodeChallenge.Application.Features.GuidMetadata.Commands;
using Ease.CodeChallenge.Application.Interfaces;
using Ease.CodeChallenge.Domain.Entities;
using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;

namespace Ease.CodeChallenge.Tests.Commands
{
    public class CreateUserMetadataCommandTests
    {
        private Mock<IApplicationContext> _applicationContextMock;
        private readonly string ExistingGuid = Guid.NewGuid().ToString("N").ToUpper();
        public CreateUserMetadataCommandTests()
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
        public async Task HandleCreateGuid_GuidIsProvidedAndAlreadyExists_Returns_ApiException()
        {
            //Arrange
            var command = new CreateUserMetadataCommand()
            {
                Guid = ExistingGuid,
                Expires = DateTime.Now,
                User = "Test"
            };

            var commandHandler = new CreateUserMetadataCommandHandler(_applicationContextMock.Object);

            //Act and Assert
            await commandHandler.Awaiting(x => x.Handle(command, default))
                .Should().ThrowAsync<ApiException>()
                .WithMessage("Guid identifier is already in use");
        }

        [Fact]
        public async Task HandleCreateGuid_GuidIsProvidedAndNotExists_Returns_GuidCreated()
        {
            //Arrange
            var command = new CreateUserMetadataCommand()
            {
                Guid = Guid.NewGuid().ToString("N").ToUpper(),
                Expires = DateTime.Now,
                User = "Test"
            };

            var commandHandler = new CreateUserMetadataCommandHandler(_applicationContextMock.Object);

            //Act
            var response = await commandHandler.Handle(command, default);

            //Assert
            response.Guid.Should().Be(command.Guid);
        }

        [Fact]
        public async Task HandleCreateGuid_GuidIsNotProvided_Returns_GuidCreatedWithGuidGenerated()
        {
            //Arrange
            var command = new CreateUserMetadataCommand()
            {
                Guid = null,
                Expires = DateTime.Now,
                User = "Test"
            };

            var commandHandler = new CreateUserMetadataCommandHandler(_applicationContextMock.Object);

            //Act
            var response = await commandHandler.Handle(command, default);

            //Assert
            response.Guid.Should().NotBeNull();
            response.Guid.Should().HaveLength(32);
        }

        [Fact]
        public async Task HandleCreateGuid_ExpiresDateIsProvided_Returns_GuidCreated()
        {
            //Arrange
            var command = new CreateUserMetadataCommand()
            {
                Guid = null,
                Expires = DateTime.Now,
                User = "Test"
            };

            var commandHandler = new CreateUserMetadataCommandHandler(_applicationContextMock.Object);

            //Act
            var response = await commandHandler.Handle(command, default);

            //Assert
            response.Expires.Should().Be(command.Expires);
        }

        [Fact]
        public async Task HandleCreateGuid_ExpiresDateIsNotProvided_Returns_GuidCreatedWithExpiresDate_30Days()
        {
            //Arrange
            var command = new CreateUserMetadataCommand()
            {
                Guid = null,
                Expires = null,
                User = "Test"
            };

            var commandHandler = new CreateUserMetadataCommandHandler(_applicationContextMock.Object);

            //Act
            var response = await commandHandler.Handle(command, default);

            //Assert
            response.Expires.Date.Should().Be(DateTime.Now.AddDays(30).Date);
        }
    }
}
