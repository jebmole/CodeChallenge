using Ease.CodeChallenge.Application.Exceptions;
using Ease.CodeChallenge.Application.Features.GuidMetadata.Commands;
using Ease.CodeChallenge.Application.Interfaces;
using Ease.CodeChallenge.Domain.Entities;
using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;

namespace Ease.CodeChallenge.Tests.Queries
{
    public class GetUserMetadataQueryTests
    {
        private Mock<IApplicationContext> _applicationContextMock;
        private readonly string ExpiredGuid = Guid.NewGuid().ToString("N").ToUpper();
        private readonly string ValidGuid = Guid.NewGuid().ToString("N").ToUpper();

        public GetUserMetadataQueryTests()
        {
            _applicationContextMock = new Mock<IApplicationContext>();

            var existingData = new List<UserMetadata>()
            {
                new UserMetadata
                {
                    Guid = ExpiredGuid,
                    Expires = DateTime.Now.AddDays(-1),
                    User = "Jonathan Estrada"
                },
                new UserMetadata
                {
                    Guid = ValidGuid,
                    Expires = DateTime.Now.AddDays(5),
                    User = "Jonathan Estrada"
                }
            };

            _applicationContextMock.Setup(x => x.UserMetadata).ReturnsDbSet(existingData);
        }

        [Fact]
        public async Task HandleGetGuid_GuidDoesNotExists_Returns_InvalidGuidException()
        {
            //Arrange
            var query = new GetUserMetadataQuery()
            {
                Guid = Guid.NewGuid().ToString("N").ToUpper(),
            };

            var queryHandler = new GetUserMetadataQueryHandler(_applicationContextMock.Object);

            //Act and Assert
            await queryHandler.Awaiting(x => x.Handle(query, default))
                .Should().ThrowAsync<InvalidGuidException>()
                .WithMessage("Guid does not exist");
        }

        [Fact]
        public async Task HandleGetGuid_GuidIsExpired_Returns_ApiException()
        {
            //Arrange
            var query = new GetUserMetadataQuery()
            {
                Guid = ExpiredGuid,
            };

            var queryHandler = new GetUserMetadataQueryHandler(_applicationContextMock.Object);

            //Act and Assert
            await queryHandler.Awaiting(x => x.Handle(query, default))
                .Should().ThrowAsync<ApiException>()
                .WithMessage("Guid is already expired");
        }

        [Fact]
        public async Task HandleGetGuid_GuidIsValid_Returns_GuidAndMetadata()
        {
            //Arrange
            var query = new GetUserMetadataQuery()
            {
                Guid = ValidGuid,
            };

            var queryHandler = new GetUserMetadataQueryHandler(_applicationContextMock.Object);

            //Act
            var response = await queryHandler.Handle(query, default);

            //Assert
            response.Guid.Should().Be(query.Guid);
        }
    }
}
