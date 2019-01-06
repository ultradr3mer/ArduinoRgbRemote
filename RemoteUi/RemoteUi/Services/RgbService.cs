using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using RemoteUi.Enumerations;
using Unity;

namespace RemoteUi.Services
{
  internal class RgbService : IDisposable
  {
    #region Fields

    private AmbientLightService ambientLightService;
    private AudioLightService audioLightService;
    private readonly TcpClient client;
    private Color colorToSet;
    private readonly IUnityContainer container;
    private Color currentColor;
    private readonly bool running = true;
    private readonly NetworkStream stream;

    #endregion

    #region Constructors

    public RgbService(IUnityContainer container)
    {
      this.client = new TcpClient("192.168.0.242", 80); //new TcpClient("192.168.0.17", 80);
      this.stream = this.client.GetStream();
      this.container = container;

      Task.Factory.StartNew(this.UpdateLoop);
    }

    #endregion

    #region Properties

    public OperationMode Mode { get; set; }

    #endregion

    #region Methods

    public void Dispose()
    {
      this.client.Dispose();
      this.stream.Dispose();
    }

    internal void SetColor(byte red, byte green, byte blue)
    {
      this.colorToSet = Color.FromRgb(red, green, blue);
    }

    private static string HexConverter(Color c)
    {
      return c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
    }

    private bool SetColorViaTcp(Color colorToSet)
    {
      var message = $"COLOR#{RgbService.HexConverter(colorToSet)}*";

      var data = Encoding.ASCII.GetBytes(message);

      this.stream.Write(data, 0, data.Length);

      Console.WriteLine("Sent: {0}", message);

      return true;
    }

    private void UpdateLoop()
    {
      while (this.running)
      {
        switch (this.Mode)
        {
          case OperationMode.D3dAmbientLight:
            this.ambientLightService = this.ambientLightService ?? this.container.Resolve<AmbientLightService>();
            this.colorToSet = this.ambientLightService.GetProperAmbientColor();
            break;

          case OperationMode.MusicControlled:
            this.audioLightService = this.audioLightService ?? this.container.Resolve<AudioLightService>();
            this.colorToSet = this.audioLightService.GetAudioLight();
            break;
        }

        if (this.currentColor != this.colorToSet)
        {
          var newColor = this.colorToSet;
          if (this.SetColorViaTcp(newColor))
          {
            this.currentColor = newColor;
          }

          Thread.Sleep(10);
        }
      }
    }

    #endregion
  }
}