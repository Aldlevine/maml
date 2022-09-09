using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maml;

internal static class GraphicsExtensions
{
    public static GraphicsPath GetRoundRectanglePath(this Graphics g, Rectangle rect, int r)
    {
        GraphicsPath path = new();

        if (r < 1)
        {
            path.AddRectangle(rect);
            return path;
        }

        int d = r * 2;
        d = Math.Min(Math.Min(rect.Width, rect.Height), d);
        Rectangle corner = new(rect.X, rect.Y, d, d);

        path.StartFigure();

        // top left
        path.AddArc(corner, 180, 90);

        // weird tweak
        // see: https://www.codeproject.com/articles/27228/a-class-for-creating-round-rectangles-in-gdi-with
        if (r == 10)
        {
            corner.Width += 1;
            corner.Height += 1;
            rect.Width -= 1;
            rect.Height -= 1;
        }

        // top right
        corner.X += rect.Width - d;
        path.AddArc(corner, 270, 90);

        // bottom right
        corner.Y += rect.Height - d;
        path.AddArc(corner, 0, 90);

        // bottom left
        corner.X -= rect.Width - d;
        path.AddArc(corner, 90, 90);

        path.CloseFigure();

        return path;
    }

    public static void DrawRoundedRectangle(this Graphics g, Rectangle rect, int r, Pen pen)
    {
        var path = g.GetRoundRectanglePath(rect, r);
        g.DrawPath(pen, path);
    }
}
