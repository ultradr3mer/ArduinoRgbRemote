using System.Windows;

namespace RemoteUi.DxHelper
{
  internal class DxPoint
  {
    #region Fields

    private static readonly int ScreenPixel = (int) SystemParameters.PrimaryScreenWidth * (int) SystemParameters.PrimaryScreenHeight;
    private static readonly int ScreenWidth = (int) SystemParameters.PrimaryScreenWidth;

    #endregion

    #region Constructors

    public DxPoint(int x, int y)
    {
      this.X = x;
      this.Y = y;
      this.DxPos = DxPoint.getPos(x, y);
    }

    #endregion

    #region Properties

    public long DxPos { get; }

    public int X { get; }
    public int Y { get; }

    #endregion

    #region Methods

    private static long getPos(int x, int y)
    {
      long pos = y * DxPoint.ScreenWidth + x;
      return pos <= DxPoint.ScreenPixel ? pos * 4 : -1;
    }

    #endregion
  }
}