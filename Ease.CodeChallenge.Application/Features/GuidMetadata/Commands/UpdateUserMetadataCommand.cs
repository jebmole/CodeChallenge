using Ease.CodeChallenge.Application.Exceptions;
using Ease.CodeChallenge.Application.Features.GuidMetadata.Responses;
using Ease.CodeChallenge.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ease.CodeChallenge.Application.Features.GuidMetadata.Commands
{
    public class UpdateUserMetadataCommand : IRequest<UpdateUserMetadataResponse>
    {
        public string Guid { get; set; }
        public DateTime Expires { get; set; }
    }

    public class UpdateUserMetadataCommandHandler(IApplicationContext context) : IRequestHandler<UpdateUserMetadataCommand, UpdateUserMetadataResponse>
    {
        private readonly IApplicationContext _context = context;
        public async Task<UpdateUserMetadataResponse> Handle(UpdateUserMetadataCommand request, CancellationToken cancellationToken)
        {
            var existingMetadata = await _context
                                        .UserMetadata
                                        .FirstOrDefaultAsync(x => x.Guid == request.Guid, cancellationToken);

            if (existingMetadata is null)
                throw new InvalidGuidException();

            existingMetadata.Expires = request.Expires;
            await _context.SaveChangesAsync(cancellationToken);

            return new UpdateUserMetadataResponse
            {
                Guid = existingMetadata.Guid,
                Expires = existingMetadata.Expires,
                User = existingMetadata.User,
            };
        }
    }
}
