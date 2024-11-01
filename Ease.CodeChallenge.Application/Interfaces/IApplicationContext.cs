using Ease.CodeChallenge.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ease.CodeChallenge.Application.Interfaces
{
    public interface IApplicationContext 
    {
        DbSet<UserMetadata> UserMetadata { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
