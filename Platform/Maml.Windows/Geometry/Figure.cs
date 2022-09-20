using Windows.Win32.Graphics.Direct2D;
using Windows.Win32.Graphics.Direct2D.Common;

namespace Maml.Geometry;

public abstract partial record Figure
{
	public partial record Rect : Figure
	{
		internal D2D_RECT_F ToD2DRectF() => new()
		{
			left = (float)Origin.X,
			top = (float)Origin.Y,
			right = (float)(Origin.X + Size.X),
			bottom = (float)(Origin.Y + Size.Y),
		};
	}

	public partial record Ellipse : Figure
	{
		internal D2D1_ELLIPSE ToD2DEllipse() => new()
		{
			point = Origin.ToD2DPoint2F(),
			radiusX = (float)Radii.X,
			radiusY = (float)Radii.Y,
		};
	}
}
