using Windows.Win32.Graphics.Direct2D.Common;

namespace Maml.Geometry;

public abstract partial record Figure
{
	public partial record Rect : Figure
	{
		internal D2D_RECT_F ToD2DRectF()
		{
			return new()
			{
				left = (float)Origin.X,
				top = (float)Origin.Y,
				right = (float)(Origin.X + Size.X),
				bottom = (float)(Origin.Y + Size.Y),
			};
		}
	}
}
