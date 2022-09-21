using Maml.Math;
using System.Collections.Generic;

namespace Maml.Graphics;

public abstract partial class Graphic
{
	public Transform Transform { get; set; } = Transform.Identity;
}

public partial class GeometryGraphic : Graphic
{
	public Geometry? Geometry { get; set; }
	public List<DrawLayer> DrawLayers { get; set; } = new();
}
