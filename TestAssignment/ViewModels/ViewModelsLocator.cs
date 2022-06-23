using Microsoft.Extensions.DependencyInjection;

namespace TestAssignment.ViewModels
{
    internal class ViewModelsLocator
    {
        public MainWindowViewModel MainWindowModel => App.Services.GetRequiredService<MainWindowViewModel>();
    }
}
