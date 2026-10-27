using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkillHub.Application.Interfaces.Repositories;
using SkillHub.Application.Interfaces.Services;
using SkillHub.Infrastructure.Persistence;
using SkillHub.Infrastructure.Repositories;
using SkillHub.Infrastructure.Services;

namespace SkillHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
                               ?? configuration["DATABASE_URL"];

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                options.UseNpgsql(connectionString);
            }
        });

        // Unit of Work
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

        // Repositorios
        services.AddScoped<IInstructorRepository, InstructorRepository>();
        services.AddScoped<IWorkshopRepository, WorkshopRepository>();
        services.AddScoped<IParticipantRepository, ParticipantRepository>();
        services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();

        // Servicios de infraestructura
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        return services;
    }
}
