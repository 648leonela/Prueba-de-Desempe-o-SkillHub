using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillHub.Domain.Entities;

namespace SkillHub.Infrastructure.Persistence.Configurations;

public class ParticipantConfiguration : IEntityTypeConfiguration<Participant>
{
    public void Configure(EntityTypeBuilder<Participant> builder)
    {
        builder.ToTable("Participants");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.FullName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasIndex(p => p.Email)
            .IsUnique();

        builder.Property(p => p.IdentityUserId)
            .HasMaxLength(450);

        // Índice sobre IdentityUserId para búsquedas eficientes por usuario autenticado
        builder.HasIndex(p => p.IdentityUserId)
            .IsUnique()
            .HasFilter("\"IdentityUserId\" IS NOT NULL");

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        // Relación con Inscripciones (Restrict para conservar historiales)
        builder.HasMany(p => p.Enrollments)
            .WithOne(e => e.Participant)
            .HasForeignKey(e => e.ParticipantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
