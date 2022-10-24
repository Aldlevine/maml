namespace Maml.Math;

public partial struct Transform
{
	unsafe internal Transform(double[] matrixArray)
	{
		fixed (double* pMatrixArray = matrixArray)
		{
			matrix = *(System.Numerics.Matrix3x2*)&pMatrixArray;
		}
	}

	internal double[] ToDoubleArray() => new[] {
		X.X, X.Y, Y.X, Y.Y, Origin.X, Origin.Y,
	};
}
