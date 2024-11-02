using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Persistence.Configurations;

/// <summary>
/// Represents the configuration for the <see cref="PersonalEvent"/> entity.
/// </summary>
internal sealed class PersonalEventConfiguration : IEntityTypeConfiguration<PersonalEvent>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<PersonalEvent> builder) =>
        builder.Property(personalEvent => personalEvent.Processed)
            .IsRequired()
            .HasDefaultValue(false);
}
