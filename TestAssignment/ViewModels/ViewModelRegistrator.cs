using Microsoft.Extensions.DependencyInjection;

namespace TestAssignment.ViewModels
{
    internal static class ViewModelRegistrator
    {
        public static IServiceCollection AddViews(this IServiceCollection services) => services
            .AddSingleton<MainWindowViewModel>();
    }
}
