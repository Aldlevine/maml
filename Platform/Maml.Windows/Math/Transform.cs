using Windows.Win32.Graphics.Direct2D.Common;

namespace Maml.Math;

public partial struct Transform
{
	unsafe internal D2D_MATRIX_3X2_F ToD2DMatrix3X2F()
	{
		var asFloat = new float[] { (float)X.X, (float)X.Y, (float)Y.X, (float)Y.Y, (float)Origin.X, (float)Origin.Y };
		fixed (void* pAsFloat = asFloat)
		{
			return *(D2D_MATRIX_3X2_F*)pAsFloat;
		}
	}

	internal Transform(D2D_MATRIX_3X2_F d2DMatrix3X2F)
	{
		X = new(d2DMatrix3X2F.Anonymous.m._0, d2DMatrix3X2F.Anonymous.m._1);
		Y = new(d2DMatrix3X2F.Anonymous.m._2, d2DMatrix3X2F.Anonymous.m._3);
		Origin = new(d2DMatrix3X2F.Anonymous.m._4, d2DMatrix3X2F.Anonymous.m._5);
	}
}
