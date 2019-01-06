using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using RemoteUi.DxHelper;
using SlimDX.Direct3D9;

namespace RemoteUi.Services
{
  internal class AmbientLightService : IDisposable
  {
    #region Fields

    private const int DATASTREAM_BYTES_PER_PIXEL = 4;
    private const int resolutionX = 20;
    private const int resolutionY = 10;
    private static Color smoothAmbientColor;

    private readonly byte[] colorBuffer = new byte[AmbientLightService.DATASTREAM_BYTES_PER_PIXEL];

    private readonly Device device;
    private readonly Direct3D direct3D;
    private bool running = true;
    private readonly Task task;

    #endregion

    #region Constructors

    public AmbientLightService()
    {
      this.direct3D = new Direct3D();

      var present_params = new PresentParameters();
      present_params.Windowed = true;
      present_params.SwapEffect = SwapEffect.Discard;
      this.device = new Device(this.direct3D, 0, DeviceType.Hardware, IntPtr.Zero, CreateFlags.SoftwareVertexProcessing, present_params);

      this.task = Task.Factory.StartNew(this.UpdateLoop);
    }

    #endregion

    #region Properties

    public Color AmbientColor { get; private set; }

    #endregion

    #region Methods

    public void Dispose()
    {
      this.running = false;
      this.task.Wait(1000);
      this.device.Dispose();
      this.direct3D.Dispose();
    }

    public Color GetProperAmbientColor()
    {
      var screenColor = this.AmbientColor;
      var correctedScreenColor = AmbientLightService.Mult(screenColor, Color.FromRgb(255, 123, 71));
      AmbientLightService.smoothAmbientColor = AmbientLightService.Mix(AmbientLightService.smoothAmbientColor, correctedScreenColor, 0.2);
      return AmbientLightService.smoothAmbientColor;
    }

    private static Color Mix(Color smoothAmbientColor, Color newColor, double v)
    {
      var r = smoothAmbientColor.R * (1.0 - v) + newColor.R * v;
      var g = smoothAmbientColor.G * (1.0 - v) + newColor.G * v;
      var b = smoothAmbientColor.B * (1.0 - v) + newColor.B * v;
      return Color.FromRgb((byte) Math.Min(r, 255), (byte) Math.Min(g, 255), (byte) Math.Min(b, 255));
    }

    private static Color Mult(Color newColor, Color color)
    {
      var r = newColor.R * color.R / 255.0;
      var g = newColor.G * color.G / 255.0;
      var b = newColor.B * color.B / 255.0;
      return Color.FromRgb((byte) Math.Min(r, 255), (byte) Math.Min(g, 255), (byte) Math.Min(b, 255));
    }

    private Color GetColor()
    {
      var sw = new Stopwatch();
      sw.Start();

      var primaryScreenWidth = (int) SystemParameters.PrimaryScreenWidth;
      var primaryScreenHeight = (int) SystemParameters.PrimaryScreenHeight;

      long red = 0;
      long green = 0;
      long blue = 0;

      var rnd = new Random(5498675);

      using (var surface = Surface.CreateOffscreenPlain(this.device,
        primaryScreenWidth,
        primaryScreenHeight,
        Format.A8R8G8B8,
        Pool.Scratch))
      {
        this.device.GetFrontBufferData(0, surface);

        var dataRectangle = surface.LockRectangle(LockFlags.None);
        var dataStream = dataRectangle.Data;

        for (var x = 0; x < AmbientLightService.resolutionX; x++)
        {
          var xShift = rnd.NextDouble();
          var xPixelPos = (int) ((x + xShift) * primaryScreenWidth / AmbientLightService.resolutionX);

          for (var y = 0; y < AmbientLightService.resolutionY; y++)
          {
            var yShift = rnd.NextDouble();
            var yPixelPos = (int) ((y + yShift) * primaryScreenHeight / AmbientLightService.resolutionY);

            var dxPoint = new DxPoint(xPixelPos, yPixelPos);

            dataStream.Position = dxPoint.DxPos;
            dataStream.Read(this.colorBuffer, 0, 4);

            red += this.colorBuffer[2];
            green += this.colorBuffer[1];
            blue += this.colorBuffer[0];
          }
        }

        surface.UnlockRectangle();
      }

      var sampleCount = AmbientLightService.resolutionX * AmbientLightService.resolutionY;
      var redAverage = (byte) (red / sampleCount);
      var greenAverage = (byte) (green / sampleCount);
      var blueAverage = (byte) (blue / sampleCount);

      sw.Stop();
      //Debug.WriteLine($"Elapsed: {sw.ElapsedMilliseconds}ms");

      return Color.FromRgb(redAverage, greenAverage, blueAverage);
    }

    private void UpdateLoop()
    {
      while (this.running)
      {
        this.AmbientColor = this.GetColor();
        Thread.Sleep(200);
      }
    }

    #endregion
  }
}