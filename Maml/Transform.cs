using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Windows.Win32.Graphics.Direct2D;
using Windows.Win32.Graphics.Direct2D.Common;

namespace Maml;

public struct Transform
{
    public Vector2 X;
    public Vector2 Y;
    public Vector2 Origin;

    public Transform(Vector2 x, Vector2 y, Vector2 origin)
    {
        X = x;
        Y = y;
        Origin = origin;
    }

    public static readonly Transform Identity = new(
        new(1, 0),
        new(0, 1),
        new(0, 0));

    public static readonly Transform PixelIdentity = new(
        new(1, 0),
        new(0, 1),
        new(-0.5f, -0.5f));

    unsafe internal D2D_MATRIX_3X2_F ToD2DMatrix3X2F()
    {
		fixed (Transform* pThis = &this)
		{
            return *(D2D_MATRIX_3X2_F*)pThis;
		}
    }

    //    var xform = new D2D_MATRIX_3X2_F()
    //    {
    //        Anonymous = new()
    //        {
    //            m = new()
    //            {
    //                _0 = 1, _2 = 0, _4 = -0.5f,
    //                _1 = 0, _3 = 1, _5 = -0.5f,
    //            }
    //        }
    //    };
}
