using System.Windows;
using Prism.Events;
using Prism.Mvvm;
using Unity;
using Unity.Lifetime;

namespace RemoteUi
{
  /// <summary>Interaktionslogik für "App.xaml"</summary>
  public partial class App : Application
  {
    #region Methods

    private void Application_Startup(object sender, StartupEventArgs e)
    {
      IUnityContainer container = new UnityContainer();
      container.RegisterType<IEventAggregator, EventAggregator>(new ContainerControlledLifetimeManager());
      ViewModelLocationProvider.SetDefaultViewModelFactory(type => { return container.Resolve(type); });
    }

    #endregion
  }
}