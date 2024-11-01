using Ease.CodeChallenge.Application.Exceptions;
using Ease.CodeChallenge.Application.Features.GuidMetadata.Responses;
using Ease.CodeChallenge.Application.Interfaces;
using Ease.CodeChallenge.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ease.CodeChallenge.Application.Features.GuidMetadata.Commands
{
    public class CreateUserMetadataCommand : IRequest<CreateUserMetadataResponse>
    {
        public string Guid { get; set; }
        public DateTime? Expires { get; set; }
        public string User { get; set; }
    }

    public class CreateUserMetadataCommandHandler(IApplicationContext context) : IRequestHandler<CreateUserMetadataCommand, CreateUserMetadataResponse>
    {
        private readonly IApplicationContext _context = context;
        public async Task<CreateUserMetadataResponse> Handle(CreateUserMetadataCommand request, CancellationToken cancellationToken)
        {
            bool isNewGuid = string.IsNullOrEmpty(request.Guid);

            if (!isNewGuid)
            {
                var guidInUse = await _context
                                       .UserMetadata
                                       .AnyAsync(x => x.Guid == request.Guid, cancellationToken);


                if (guidInUse)
                    throw new ApiException("Guid identifier is already in use");
            }
            
            var meta = new UserMetadata
            {
                Guid = isNewGuid ? Guid.NewGuid().ToString("N").ToUpper() : request.Guid,
                Expires = request.Expires ?? DateTime.Now.AddDays(30),
                User = request.User,
            };

            await _context.UserMetadata.AddAsync(meta, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new CreateUserMetadataResponse
            {
                Guid = meta.Guid,
                Expires = meta.Expires,
                User = meta.User,
            };
        }
    }
}
