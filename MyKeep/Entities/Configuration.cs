using Marten;
using Marten.Events.Projections;
using MyKeep.Entities.TodoList;

namespace MyKeep.Entities;

public static class Configuration
{
    public static IServiceCollection AddEntities(this IServiceCollection services)
        => services.ConfigureMarten(opts =>
        {
            opts.Projections.Add<CheckListProjection>(ProjectionLifecycle.Inline);
        });
}