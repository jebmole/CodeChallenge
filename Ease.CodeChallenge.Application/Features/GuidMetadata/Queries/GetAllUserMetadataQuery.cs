using Ease.CodeChallenge.Application.Features.GuidMetadata.Responses;
using Ease.CodeChallenge.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ease.CodeChallenge.Application.Features.GuidMetadata.Commands
{
    public class GetAllUserMetadataQuery : IRequest<List<GetUserMetadataResponse>>
    {
    }

    public class GetAllUserMetadataQueryHandler(IApplicationContext context, ICacheService cacheService) : IRequestHandler<GetAllUserMetadataQuery, List<GetUserMetadataResponse>>
    {
        private readonly IApplicationContext _context = context;
        private readonly ICacheService _cacheService = cacheService;
        private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(1);

        public async Task<List<GetUserMetadataResponse>> Handle(GetAllUserMetadataQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"AllGuids";
            var cachedGuids = await _cacheService.GetCacheValueAsync<List<GetUserMetadataResponse>>(cacheKey);
            if (cachedGuids != null)
            {
                return cachedGuids;
            }


            var metadatas = await _context.UserMetadata
                                    .AsNoTracking()
                                    .ToListAsync(cancellationToken);

            var allGuids = metadatas.Select(x => new GetUserMetadataResponse
            {
                Guid = x.Guid,
                Expires = x.Expires,
                User = x.User,
            }).ToList();

            await _cacheService.SetCacheValueAsync(cacheKey, allGuids, CacheExpiration);

            return allGuids;
        }
    }
}
