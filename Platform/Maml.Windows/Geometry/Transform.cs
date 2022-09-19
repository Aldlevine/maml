using Windows.Win32.Graphics.Direct2D.Common;

namespace Maml.Geometry;

public partial struct Transform
{
	unsafe internal D2D_MATRIX_3X2_F ToD2DMatrix3X2F()
	{
		fixed (Transform* pThis = &this)
		{
			return *(D2D_MATRIX_3X2_F*)pThis;
		}
	}
}
