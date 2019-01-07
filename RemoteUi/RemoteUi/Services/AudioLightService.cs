using System;
using NAudio.CoreAudioApi;
using RL;

namespace RemoteUi.Services
{
  internal class AudioLightService
  {
    #region Fields

    private double averageVolume = 0.5;
    private readonly MMDevice defaultDevice;

    private double hueShift;
    private double maxVolume = 0.01;
    private double minVolume = 0.01;

    #endregion

    #region Constructors

    public AudioLightService()
    {
      var devEnum = new MMDeviceEnumerator();
      this.defaultDevice = devEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
    }

    #endregion

    #region Methods

    internal Color GetAudioLight()
    {
      var volume = this.defaultDevice.AudioMeterInformation.MasterPeakValue;

      volume = (float) this.GetProperVolume(volume);

      return this.GetColor(volume);
    }

    private Color GetColor(float volume)
    {
      this.hueShift += volume * 0.1;
      if (this.hueShift > 360.0)
      {
        this.hueShift -= 360.0;
      }

      var primary = Color.FromHsl(this.hueShift, 1.0, 0.5);

      var secondaryHue = this.hueShift - 120.0;
      if (secondaryHue < 0.0)
      {
        secondaryHue += 360.0;
      }

      var secondary = Color.FromHsl(secondaryHue, 1.0, 0.05);

      return Color.Mix(secondary, primary, volume) * Color.FromRgb(255, 123, 71);
    }

    private double Clamp(double properVolume)
    {
      return double.IsNaN(properVolume) ? 0 : properVolume > 1 ? 1 : properVolume < 0 ? 0 : properVolume;
    }

    private double GetProperVolume(double volume)
    {
      var preDelta = this.maxVolume - this.minVolume;

      var adjustionRate = 0.001f;

      this.maxVolume -= adjustionRate;
      this.maxVolume = Math.Max(this.maxVolume, volume);

      this.minVolume += adjustionRate;
      this.minVolume = Math.Min(this.minVolume, volume);

      var delta = this.maxVolume - this.minVolume;

      var properVolume = volume - this.minVolume;

      if (delta == 0)
      {
        properVolume = Math.Sign(properVolume);
      }
      else
      {
        properVolume /= delta;
      }

      properVolume = properVolume * 0.5f + volume * 0.5f;

      //properVolume = Math.Pow(properVolume, 2);

      Console.WriteLine("Input Volume: {0}", volume);

      //Console.WriteLine("Normalized Volume: {0} Range: {1}-{2}", properVolume, this.minVolume, this.maxVolume);

      return this.Clamp(properVolume);
    }

    #endregion
  }
}