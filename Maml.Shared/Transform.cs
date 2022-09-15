using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Maml;

public partial struct Transform
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
}
