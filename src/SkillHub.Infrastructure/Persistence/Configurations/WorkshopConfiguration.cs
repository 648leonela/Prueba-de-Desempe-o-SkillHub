using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillHub.Domain.Entities;

namespace SkillHub.Infrastructure.Persistence.Configurations;

public class WorkshopConfiguration : IEntityTypeConfiguration<Workshop>
{
    public void Configure(EntityTypeBuilder<Workshop> builder)
    {
        builder.ToTable("Workshops");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Code)
            .IsRequired()
            .HasMaxLength(30);

        // Restricción e índice único para evitar códigos de taller duplicados
        builder.HasIndex(w => w.Code)
            .IsUnique();

        builder.Property(w => w.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(w => w.Description)
            .HasMaxLength(1000);

        builder.Property(w => w.StartDate)
            .IsRequired();

        builder.Property(w => w.EndDate)
            .IsRequired();

        builder.Property(w => w.Capacity)
            .IsRequired();

        // Relación con Instructor (Restrict para evitar eliminación accidental)
        builder.HasOne(w => w.Instructor)
            .WithMany(i => i.Workshops)
            .HasForeignKey(w => w.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación con Inscripciones (Restrict para conservar historiales)
        builder.HasMany(w => w.Enrollments)
            .WithOne(e => e.Workshop)
            .HasForeignKey(e => e.WorkshopId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
