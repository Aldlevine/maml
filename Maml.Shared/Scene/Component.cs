using Maml.Graphics;
using Maml.Math;

namespace Maml.Scene;

public abstract class Component
{
	public Node? Node;
}

public class GraphicComponent : Component
{
	public Transform Transform { get; set; } = Transform.Identity;
	public Graphic? Graphic { get; set; } = default;

	public GraphicComponent() : base() { }
	public GraphicComponent(GraphicComponent graphicComponent)
	{
		Transform = graphicComponent.Transform;
		Graphic = graphicComponent.Graphic;
	}

	public static implicit operator GraphicComponent(Graphic graphic) => new GraphicComponent { Graphic = graphic, };
}
