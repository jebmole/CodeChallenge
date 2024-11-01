using Ease.CodeChallenge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ease.CodeChallenge.Infrastructure.Persistence.Configurations
{
    internal class UserMetadataConfiguration : IEntityTypeConfiguration<UserMetadata>
    {
        public void Configure(EntityTypeBuilder<UserMetadata> builder)
        {
            builder.HasKey(e => e.Guid).HasName("PK_Guid");

            builder.Property(e => e.Guid).ValueGeneratedNever();
            builder.Property(e => e.Expires).HasColumnType("datetime");
            builder.Property(e => e.User)
                .HasMaxLength(50)
                .IsUnicode(false);
        }
    }
}
