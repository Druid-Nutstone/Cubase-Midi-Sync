using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.WindowManager.Services.Win
{
    [Obsolete]
    public enum SnapPosition
    {
        Left,
        Right,
        Top,
        Bottom,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        Center,
        Custom
    }

    [Obsolete()]
    public static class SnapHelper
    {
        public static void SnapWindow(IntPtr hWnd, SnapPosition position,
            double customWidthRatio = 1.0, double customHeightRatio = 1.0)
        {
            var mon = WindowManagerService.GetMonitorForWindow(hWnd);
            var rect = mon.Work;

            int x = rect.Left;
            int y = rect.Top;
            int width = rect.Width;
            int height = rect.Height;

            switch (position)
            {
                case SnapPosition.Left:
                    width /= 2;
                    break;

                case SnapPosition.Right:
                    width /= 2;
                    x += width;
                    break;

                case SnapPosition.Top:
                    height /= 2;
                    break;

                case SnapPosition.Bottom:
                    height /= 2;
                    y += height;
                    break;

                case SnapPosition.TopLeft:
                    width /= 2;
                    height /= 2;
                    break;

                case SnapPosition.TopRight:
                    width /= 2;
                    height /= 2;
                    x += width;
                    break;

                case SnapPosition.BottomLeft:
                    width /= 2;
                    height /= 2;
                    y += height;
                    break;

                case SnapPosition.BottomRight:
                    width /= 2;
                    height /= 2;
                    x += width;
                    y += height;
                    break;

                case SnapPosition.Center:
                    width = (int)(width * 0.6);
                    height = (int)(height * 0.6);
                    x = rect.Left + (rect.Width - width) / 2;
                    y = rect.Top + (rect.Height - height) / 2;
                    break;

                case SnapPosition.Custom:
                    width = (int)(rect.Width * customWidthRatio);
                    height = (int)(rect.Height * customHeightRatio);
                    x = rect.Left + (rect.Width - width) / 2;
                    y = rect.Top + (rect.Height - height) / 2;
                    break;
            }

            WindowManagerService.SetPosition(hWnd, x, y, width, height);
        }

        public static bool SnapWindow(string windowTitlePart, SnapPosition position)
        {
            var hWnd = WindowManagerService.FindWindowByTitle(windowTitlePart);
            if (hWnd == null)
                return false;

            SnapWindow(hWnd.Value, position);
            return true;
        }
    }


}
