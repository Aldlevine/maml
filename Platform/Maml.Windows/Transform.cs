using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Win32.Graphics.Direct2D;
using Windows.Win32.Graphics.Direct2D.Common;

namespace Maml;

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
