using Ease.CodeChallenge.Application.Exceptions;
using Ease.CodeChallenge.Application.Features.GuidMetadata.Commands;
using Ease.CodeChallenge.Application.Features.GuidMetadata.Responses;
using Ease.CodeChallenge.Application.Interfaces;
using Ease.CodeChallenge.Domain.Entities;
using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;

namespace Ease.CodeChallenge.Tests.Queries
{
    public class GetAllUserMetadataQueryTests
    {
        private Mock<IApplicationContext> _applicationContextMock;
        private Mock<ICacheService> _cacheServiceMock;
        private readonly string ExpiredGuid = Guid.NewGuid().ToString("N").ToUpper();
        private readonly string ValidGuid = Guid.NewGuid().ToString("N").ToUpper();

        public GetAllUserMetadataQueryTests()
        {
            _applicationContextMock = new Mock<IApplicationContext>();
            _cacheServiceMock = new Mock<ICacheService>();

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
        public async Task HandleGetAllGuid_RetrieveAll_Returns_AllRecordsFromDB()
        {
            //Arrange
            var query = new GetAllUserMetadataQuery();

            var queryHandler = new GetAllUserMetadataQueryHandler(_applicationContextMock.Object, _cacheServiceMock.Object);

            //Act
            var response = await queryHandler.Handle(query, default);

            //Assert
            response.Should().HaveCount(2);
        }

        [Fact]
        public async Task HandleGetAllGuid_RetrieveAll_Returns_AllRecordsFromCache()
        {
            //Arrange
            var query = new GetAllUserMetadataQuery();
            var cacheData = new List<GetUserMetadataResponse>()
            {
                new GetUserMetadataResponse
                {
                    Guid = ValidGuid,
                    Expires = DateTime.Now.AddDays(5),
                    User = "Jonathan Estrada"
                }
            };

            _cacheServiceMock.Setup(x => x.GetCacheValueAsync<List<GetUserMetadataResponse>>(It.IsAny<string>())).Returns(Task.FromResult(cacheData));

            var queryHandler = new GetAllUserMetadataQueryHandler(_applicationContextMock.Object, _cacheServiceMock.Object);

            //Act
            var response = await queryHandler.Handle(query, default);

            //Assert
            response.Should().HaveCount(1);
        }
    }
}
