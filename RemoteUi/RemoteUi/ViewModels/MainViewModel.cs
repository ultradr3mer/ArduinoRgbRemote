using System.ComponentModel;
using Prism.Mvvm;
using RemoteUi.Enumerations;
using RemoteUi.Services;

namespace RemoteUi.ViewModels
{
  internal class MainViewModel : BindableBase
  {
    #region Fields

    private byte propBlue;
    private byte propGreen;
    private bool propIsCustomColorEnabled;
    private bool propIsD3dAmbientLightEnabled;
    private bool propIsMusicControlled;
    private byte propRed;
    private readonly RgbService service;

    #endregion

    #region Constructors

    public MainViewModel(RgbService service)
    {
      this.service = service;
      this.PropertyChanged += this.MainViewModel_PropertyChanged;
      this.IsCustomColorEnabled = true;
    }

    #endregion

    #region Properties

    public byte Blue
    {
      get { return this.propBlue; }
      set { this.SetProperty(ref this.propBlue, value); }
    }

    public byte Green
    {
      get { return this.propGreen; }
      set { this.SetProperty(ref this.propGreen, value); }
    }

    public bool IsCustomColorEnabled
    {
      get { return this.propIsCustomColorEnabled; }
      set { this.SetProperty(ref this.propIsCustomColorEnabled, value); }
    }

    public bool IsD3dAmbientLightEnabled
    {
      get { return this.propIsD3dAmbientLightEnabled; }
      set { this.SetProperty(ref this.propIsD3dAmbientLightEnabled, value); }
    }

    public bool IsMusicControlled
    {
      get { return this.propIsMusicControlled; }
      set { this.SetProperty(ref this.propIsMusicControlled, value); }
    }

    public byte Red
    {
      get { return this.propRed; }
      set { this.SetProperty(ref this.propRed, value); }
    }

    #endregion

    #region Methods

    private void MainViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(MainViewModel.Red) || e.PropertyName == nameof(MainViewModel.Green) || e.PropertyName == nameof(MainViewModel.Blue))
      {
        this.service.SetColor(this.Red, this.Green, this.Blue);
      }
      else if (e.PropertyName == nameof(MainViewModel.IsMusicControlled) || e.PropertyName == nameof(MainViewModel.IsD3dAmbientLightEnabled) || e.PropertyName == nameof(MainViewModel.IsCustomColorEnabled))
      {
        if (this.IsMusicControlled)
        {
          this.service.Mode = OperationMode.MusicControlled;
        }
        else if (this.IsCustomColorEnabled)
        {
          this.service.Mode = OperationMode.Custom;
        }
        else if (this.IsD3dAmbientLightEnabled)
        {
          this.service.Mode = OperationMode.D3dAmbientLight;
        }
      }
    }

    #endregion
  }
}