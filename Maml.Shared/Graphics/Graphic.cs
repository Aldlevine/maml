using Maml.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maml.Graphics;

public abstract partial class Graphic
{
	public Transform Transform { get; set; } = Transform.Identity;
}

public partial class GeometryGraphic : Graphic
{
	public Geometry? Geometry { get; set; }
	public List<DrawLayer> DrawLayers { get; init; } = new();
}
