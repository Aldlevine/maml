﻿using System;
using System.Collections.Generic;
using System.Text;
using Windows.Win32.Graphics.Direct2D;
using Windows.Win32.Graphics.Direct2D.Common;

namespace Maml.Geometry;

public partial struct Rect
{
	internal D2D_RECT_F ToD2DRectF() => new()
	{
		left = (float)Position.X,
		top = (float)Position.Y,
		right = (float)(Position.X + Size.X),
		bottom = (float)(Position.Y + Size.Y),
	};
}

