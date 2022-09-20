using Windows.Win32.Graphics.Direct2D.Common;

namespace Maml.Math;

public partial struct Vector2
{
	internal D2D_POINT_2F ToD2DPoint2F()
	{
		return new D2D_POINT_2F { x = (float)X, y = (float)Y };
	}

	internal D2D_SIZE_U ToD2DSizeU()
	{
		return new D2D_SIZE_U { width = (uint)X, height = (uint)Y };
	}
}
