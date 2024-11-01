using Ease.CodeChallenge.Application.Exceptions;
using Ease.CodeChallenge.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ease.CodeChallenge.Application.Features.GuidMetadata.Commands
{
    public class DeleteUserMetadataCommand : IRequest<bool>
    {
        public string Guid { get; set; }
    }

    public class DeleteUserMetadataCommandHandler(IApplicationContext context) : IRequestHandler<DeleteUserMetadataCommand, bool>
    {
        private readonly IApplicationContext _context = context;
        public async Task<bool> Handle(DeleteUserMetadataCommand request, CancellationToken cancellationToken)
        {
            var existingMetadata = await _context
                                        .UserMetadata
                                        .FirstOrDefaultAsync(x => x.Guid == request.Guid, cancellationToken);

            if (existingMetadata is null)
                throw new InvalidGuidException();

            _context.UserMetadata.Remove(existingMetadata);
            var affectedRows = await _context.SaveChangesAsync(cancellationToken);
            return affectedRows > 0;
        }
    }
}
