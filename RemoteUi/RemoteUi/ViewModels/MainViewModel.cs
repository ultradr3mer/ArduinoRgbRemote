using System;
using System.ComponentModel;
using System.Windows.Media;
using Prism.Events;
using Prism.Mvvm;
using RemoteUi.Daten;
using RemoteUi.Enumerations;
using RemoteUi.Events;
using RemoteUi.Services;

namespace RemoteUi.ViewModels
{
  internal class MainViewModel : BindableBase
  {
    #region Fields

    private readonly RgbService service;

    private byte propBlue;
    private byte propGreen;
    private bool propIsCustomColorEnabled;
    private bool propIsD3dAmbientLightEnabled;
    private bool propIsMusicControlled;

    /// <summary>The <see cref="LedColor" /> property's value.</summary>
    private SolidColorBrush propLedColor;

    private byte propRed;

    #endregion

    #region Constructors

    public MainViewModel(RgbService service, IEventAggregator eventAggretator)
    {
      this.service = service;
      this.PropertyChanged += this.MainViewModel_PropertyChanged;
      this.IsCustomColorEnabled = true;

      eventAggretator.GetEvent<ColorChanged>().Subscribe(this.HandleColorChanged, ThreadOption.UIThread);
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

    /// <summary>Gets or sets the led color.</summary>
    public SolidColorBrush LedColor
    {
      get { return this.propLedColor; }
      set { this.SetProperty(ref this.propLedColor, value); }
    }

    public byte Red
    {
      get { return this.propRed; }
      set { this.SetProperty(ref this.propRed, value); }
    }

    #endregion

    #region Methods

    /// <summary>Handles the ColorChanged event.</summary>
    /// <param name="data">The event data.</param>
    private void HandleColorChanged(ColorChangedDaten data)
    {
      this.LedColor = new SolidColorBrush(Color.FromRgb(data.Color.R, data.Color.G, data.Color.B));
    }

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