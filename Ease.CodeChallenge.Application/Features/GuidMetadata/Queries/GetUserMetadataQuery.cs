using Ease.CodeChallenge.Application.Exceptions;
using Ease.CodeChallenge.Application.Features.GuidMetadata.Responses;
using Ease.CodeChallenge.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ease.CodeChallenge.Application.Features.GuidMetadata.Commands
{
    public class GetUserMetadataQuery : IRequest<GetUserMetadataResponse>
    {
        public string Guid { get; set; }
    }

    public class GetUserMetadataQueryHandler(IApplicationContext context) : IRequestHandler<GetUserMetadataQuery, GetUserMetadataResponse>
    {
        private readonly IApplicationContext _context = context;

        public async Task<GetUserMetadataResponse> Handle(GetUserMetadataQuery request, CancellationToken cancellationToken)
        {
            var metadata = await _context.UserMetadata
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(x=> x.Guid == request.Guid, cancellationToken);
            
            if (metadata is null)
                throw new InvalidGuidException();

            if (metadata.Expires < DateTime.Now)
                throw new ApiException("Guid is already expired");

            return new GetUserMetadataResponse
            {
                Guid = metadata.Guid,
                Expires = metadata.Expires,
                User = metadata.User,
            };
        }
    }
}
