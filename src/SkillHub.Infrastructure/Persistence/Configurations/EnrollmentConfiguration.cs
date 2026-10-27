using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillHub.Domain.Entities;

namespace SkillHub.Infrastructure.Persistence.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("Enrollments");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.EnrolledAt)
            .IsRequired();

        // Conversión del Enum EnrollmentStatus almacenando el valor numérico
        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(e => e.CancelledAt);

        builder.Property(e => e.CancelledBy)
            .HasMaxLength(50);

        builder.Property(e => e.CancellationReason)
            .HasMaxLength(500);

        // Relaciones foráneas sin borrado en cascada para preservar historiales
        builder.HasOne(e => e.Participant)
            .WithMany(p => p.Enrollments)
            .HasForeignKey(e => e.ParticipantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Workshop)
            .WithMany(w => w.Enrollments)
            .HasForeignKey(e => e.WorkshopId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índice compuesto para validación rápida de inscripciones activas duplicadas
        builder.HasIndex(e => new { e.ParticipantId, e.WorkshopId, e.Status })
            .HasDatabaseName("IX_Enrollments_Participant_Workshop_Status");

        // Índice para validación de cruces de horario y consulta de historial propio
        builder.HasIndex(e => new { e.ParticipantId, e.Status })
            .HasDatabaseName("IX_Enrollments_Participant_Status");
    }
}
