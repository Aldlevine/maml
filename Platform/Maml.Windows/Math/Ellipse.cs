using System;
using System.Collections.Generic;
using System.Text;
using Windows.Win32.Graphics.Direct2D;

namespace Maml.Math;

public partial struct Ellipse
{
	internal D2D1_ELLIPSE ToD2DEllipse() => new()
	{
		point = Center.ToD2DPoint2F(),
		radiusX = (float)Radius.X,
		radiusY = (float)Radius.Y,
	};
}
