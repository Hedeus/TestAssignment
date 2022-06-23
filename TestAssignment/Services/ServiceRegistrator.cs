using Microsoft.Extensions.DependencyInjection;
using TestAssignment.Services.Interfaces;

namespace TestAssignment.Services
{
    static class ServiceRegistrator
    {
        public static IServiceCollection AddServices(this IServiceCollection services) => services
            .AddTransient<IUserDialog, UserDialog>()
            ;
    }
}
