using Windows.Win32.Graphics.Direct2D.Common;

namespace Maml.Graphics;

public partial struct Color
{
	unsafe internal D2D1_COLOR_F ToD2DColorF()
	{
		fixed (Color* pThis = &this)
		{
			return *(D2D1_COLOR_F*)pThis;
		}
	}
}
