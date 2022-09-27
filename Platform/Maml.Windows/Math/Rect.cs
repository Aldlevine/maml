using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Direct2D.Common;

namespace Maml.Math;

public partial struct Rect
{
	internal D2D_RECT_F ToD2DRectF() => new()
	{
		left = (float)Position.X,
		top = (float)Position.Y,
		right = (float)(Position.X + Size.X),
		bottom = (float)(Position.Y + Size.Y),
	};

	internal Rect(RECT rect)
	{
		Position = new(rect.left, rect.top);
		End = new(rect.right, rect.bottom);
	}
}

