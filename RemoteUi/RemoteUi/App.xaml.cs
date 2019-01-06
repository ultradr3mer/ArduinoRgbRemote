using System.Windows;
using Prism.Mvvm;
using Unity;

namespace RemoteUi
{
  /// <summary>Interaktionslogik für "App.xaml"</summary>
  public partial class App : Application
  {
    #region Methods

    private void Application_Startup(object sender, StartupEventArgs e)
    {
      IUnityContainer container = new UnityContainer();
      ViewModelLocationProvider.SetDefaultViewModelFactory(type => { return container.Resolve(type); });
    }

    #endregion
  }
}