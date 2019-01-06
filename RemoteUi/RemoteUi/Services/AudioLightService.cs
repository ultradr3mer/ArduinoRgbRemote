using System;
using System.Windows.Media;
using NAudio.CoreAudioApi;

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

      var red = (byte) ((1 - volume) * 255);
      var green = (byte) (volume * 128);
      var blue = (byte) (volume * 255);

      return Color.FromRgb(red, green, blue);
    }

    private double Clamp(double properVolume)
    {
      return double.IsNaN(properVolume) ? 0 : properVolume > 1 ? 1 : properVolume < 0 ? 0 : properVolume;
    }

    private double GetProperVolume(double volume)
    {
      var preDelta = this.maxVolume - this.minVolume;

      var adjustionRate = 0.00004f * preDelta;

      this.maxVolume -= adjustionRate;
      this.maxVolume = Math.Max(this.maxVolume, volume);

      this.minVolume += adjustionRate;
      this.minVolume = Math.Min(this.minVolume, volume);

      var delta = this.maxVolume - this.minVolume;

      var properVolume = volume - this.minVolume;
      properVolume /= delta;

      properVolume = properVolume * 0.5f + volume * 0.5f;

      return this.Clamp(properVolume);
    }

    #endregion
  }
}