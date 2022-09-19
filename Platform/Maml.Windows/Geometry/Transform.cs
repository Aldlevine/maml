using Windows.Win32.Graphics.Direct2D.Common;

namespace Maml.Geometry;

public partial struct Transform
{
	unsafe internal D2D_MATRIX_3X2_F ToD2DMatrix3X2F()
	{
		var asFloat = new float[] { (float)X.X, (float)X.Y, (float)Y.X, (float)Y.Y, (float)Origin.X, (float)Origin.Y };
		fixed(void* pAsFloat = asFloat)
		{
			return *(D2D_MATRIX_3X2_F*)pAsFloat;
		}
	}
}
