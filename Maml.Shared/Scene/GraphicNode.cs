using Maml.Graphics;
using Maml.Math;
using Maml.Observable;
using System;

namespace Maml.Scene;

public class GraphicNode : Node
{
	public bool NeedsRedraw { get; internal set; } = true;
	public static BasicProperty<GraphicNode, Graphic?> GraphicProperty { get; } = new(null);
	public Graphic? Graphic
	{
		get => GraphicProperty[this].Get();
		set => GraphicProperty[this].Set(value);
	}

	public virtual void Draw(RenderTarget renderTarget) => Graphic?.Draw(renderTarget, GlobalTransform);

	internal Rect PreviousBoundingRect = new();
	public virtual Rect GetBoundingRect() => GlobalTransform * Graphic?.GetBoundingRect() ?? new Rect();

	public GraphicNode()
	{
		Changed += (s, p) =>
		{
			NeedsRedraw = (p != Node.VisibleInTreeProperty) && VisibleInTree;
		};
	}
}
