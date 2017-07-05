using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ErrorTimer.Model
{
    public static class ScreenCapture
    {
        public static Bitmap CaptureScreens()
        {
            var allScreens = Screen.AllScreens.OrderBy(s => s.Bounds.Left).ToArray();
            var totalWidth = allScreens.Sum(s => s.Bounds.Width);
            var maxHeight = allScreens.Max(s => s.Bounds.Top + s.Bounds.Height);

            Bitmap bmp = new Bitmap(totalWidth, maxHeight);
            int offset = 0;

            using (var g = Graphics.FromImage(bmp))
            {
                foreach (var screen in allScreens)
                {
                    g.CopyFromScreen(screen.Bounds.Left, screen.Bounds.Top, offset,
                        screen.Bounds.Top, screen.Bounds.Size);
                    offset += screen.Bounds.Width;
                }
            }
            return bmp;
        }
    }
}
