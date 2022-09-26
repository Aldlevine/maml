using Windows.Win32.Graphics.Direct2D.Common;

namespace Maml.Math;

public partial struct Transform
{
	unsafe internal D2D_MATRIX_3X2_F ToD2DMatrix3X2F()
	{
		fixed (void* pMatrix = &matrix)
		{
			return *(D2D_MATRIX_3X2_F*)pMatrix;
		}
	}

	unsafe internal Transform(D2D_MATRIX_3X2_F d2DMatrix3X2F)
	{
		matrix = *(System.Numerics.Matrix3x2*)&d2DMatrix3X2F;
	}
}
